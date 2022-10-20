using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mock;

namespace SmartMirror.Services.Rooms
{
    public class RoomsService : IRoomsService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;
        private readonly IAqaraService _aqaraService;
        private readonly IDevicesService _devicesService;

        public RoomsService(
            IAqaraService aqaraService,
            IDevicesService devicesService,
            ISmartHomeMockService smartHomeMockService)
        {
            _smartHomeMockService = smartHomeMockService;
            _aqaraService = aqaraService;
            _devicesService = devicesService;
        }

        #region -- IRoomsService implementation --

        public Task<AOResult<IEnumerable<RoomModel>>> GetAllRoomsAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                IEnumerable<RoomModel> rooms = _smartHomeMockService.GetRooms();

                if (_aqaraService.IsAuthorized)
                {
                    var resultOfGettingRoomsAqara = await GetAllRoomsOfHousesAsync();

                    if (resultOfGettingRoomsAqara.IsSuccess)
                    {
                        rooms = resultOfGettingRoomsAqara.Result.Concat(rooms);
                    }
                }

                return rooms;
            });
        }

        #endregion

        #region -- Private helpers --

        private Task<AOResult<IEnumerable<RoomModel>>> GetAllRoomsOfHousesAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var rooms = Enumerable.Empty<RoomModel>();

                var resultOfGettingHouses = await _aqaraService.GetPositionsAsync(string.Empty);

                if (resultOfGettingHouses.IsSuccess && resultOfGettingHouses.Result.TotalCount > 0)
                {
                    var houses = resultOfGettingHouses.Result.Data;

                    foreach (var house in houses)
                    {
                        var resultOfGettingRoomsHouse = await GetRoomsHouseAsync(house.PositionId);

                        if (resultOfGettingRoomsHouse.IsSuccess)
                        {
                            rooms = rooms.Concat(resultOfGettingRoomsHouse.Result);
                        }
                        else
                        {
                            onFailure("rooms: Request failed");
                        }
                    }
                }
                else
                {
                    onFailure("houses: Request failed");
                }

                return rooms;
            });
        }

        private Task<AOResult<IEnumerable<RoomModel>>> GetRoomsHouseAsync(string positionId, int pageNum = 1, int pageSize = 100)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var rooms = new List<RoomModel>();

                var resultOfGettingRoomsHouse = await _aqaraService.GetPositionsAsync(positionId);

                if (resultOfGettingRoomsHouse.IsSuccess)
                {
                    var simpleRooms = resultOfGettingRoomsHouse.Result.Data;

                    foreach (var room in simpleRooms)
                    {
                        var devicesCount = _devicesService.AllSupportedDevices.Count(x => x.PositionId == room.PositionId);

                        rooms.Add(new RoomModel()
                        {
                            Id = room.PositionId,
                            Name = room.PositionName,
                            CreateTime = DateTime.FromBinary(room.CreateTime),
                            Description = $"{devicesCount} {Strings.Accessories}",
                            DevicesCount = devicesCount,
                        });
                    }
                }
                else
                {
                    onFailure("rooms: Request failed");
                }

                return (IEnumerable<RoomModel>)rooms;
            });
        }

        #endregion
    }
}
