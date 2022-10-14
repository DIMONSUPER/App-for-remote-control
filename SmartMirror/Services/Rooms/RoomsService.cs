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
                var rooms = Enumerable.Empty<RoomModel>();

                var resultOfGettingMockRooms = _smartHomeMockService.GetRooms();

                var resultOfGettingRoomsAqara = await GetAllRoomsOfHousesAsync();

                if (resultOfGettingRoomsAqara.IsSuccess)
                {
                    rooms = resultOfGettingRoomsAqara.Result.Concat(resultOfGettingMockRooms);
                }
                else
                {
                    rooms = resultOfGettingMockRooms;
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

                if (_aqaraService.IsAuthorized)
                {
                    var resultOfGettingHouses = await _aqaraService.GetPositionsAsync(string.Empty);

                    if (resultOfGettingHouses.IsSuccess)
                    {
                        if (resultOfGettingHouses.Result.TotalCount > 0)
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
                    }
                    else
                    {
                        onFailure("houses: Request failed");
                    }
                }
                else
                {
                    onFailure("unauthorized");
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
                        var resultOfGettingDevices = await _devicesService.GetDevicesAsync(room.PositionId);

                        if (!resultOfGettingDevices.IsSuccess)
                        {
                            onFailure("devices: Request failed");
                        }

                        var devicesCount = Enumerable.Count(resultOfGettingDevices.Result);

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
