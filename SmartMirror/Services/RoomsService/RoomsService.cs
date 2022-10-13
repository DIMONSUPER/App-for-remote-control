using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mock;

namespace SmartMirror.Services.RoomsService
{
    public class RoomsService : IRoomsService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;
        private readonly IAqaraService _aqaraService;

        public RoomsService(
            IAqaraService aqaraService, 
            ISmartHomeMockService smartHomeMockService)
        {
            _smartHomeMockService = smartHomeMockService;
            _aqaraService = aqaraService;
        }

        #region -- IRoomsService implementation --

        public Task<AOResult<IEnumerable<RoomModel>>> GetAllRoomsAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var rooms = Enumerable.Empty<RoomModel>();

                var resultOfGettingRooms = _smartHomeMockService.GetRooms();

                if (resultOfGettingRooms is not null)
                {
                    var resultOfGettingRoomsAqara = await GetAllRoomsOfHousesAsync();

                    if (resultOfGettingRoomsAqara.IsSuccess)
                    {
                        rooms = resultOfGettingRoomsAqara.Result.Concat(resultOfGettingRooms);
                    }
                    else
                    {
                        rooms = resultOfGettingRooms; 
                    }
                }
                else
                {
                    onFailure("rooms is null");
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
                    var resultOfGettingHouses = await _aqaraService.GetPositionsAsync(String.Empty, 1, 100);

                    if (resultOfGettingHouses.IsSuccess)
                    {
                        if (resultOfGettingHouses.Result.TotalCount > 0)
                        {
                            var houses = resultOfGettingHouses.Result.Data;

                            foreach (var house in houses)
                            {
                                var resultOfGettingRoomsHouse = await GetRoomsHouseAsync(house.PositionId, 1, 100);

                                if (resultOfGettingRoomsHouse.IsSuccess)
                                {
                                    rooms = resultOfGettingRoomsHouse.Result.Concat(rooms);
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

        private Task<AOResult<IEnumerable<RoomModel>>> GetRoomsHouseAsync(string positionId, int pageNum, int pageSize)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var rooms = new List<RoomModel>();

                var resultOfGettingHouseRooms = await _aqaraService.GetPositionsAsync(positionId, pageNum, pageSize);

                if (resultOfGettingHouseRooms.IsSuccess)
                {
                    foreach(var room in resultOfGettingHouseRooms.Result.Data)
                    {
                        var resultOfGettingDevices = await _aqaraService.GetDevicesPositionsync(room.PositionId, 1, 100);

                        if (!resultOfGettingDevices.IsSuccess)
                        {
                            onFailure("devices: Request failed");
                        }

                        rooms.Add(new RoomModel()
                        {
                            Id = room.PositionId,
                            Name = room.PositionName,
                            CreateTime = DateTime.FromBinary(room.CreateTime),
                            Description = $"{resultOfGettingDevices.Result.TotalCount} {Strings.Accessories}",
                            Devices = resultOfGettingDevices.Result.Data.Select(device => new DeviceModel()
                            {
                                Id = device.Did,
                                Name = device.DeviceName,
                                Status = (Enums.EDeviceStatus)device.State,
                                Type = device.ModelType.ToString(),
                                RoomName = room.PositionName,
                            }),
                            DevicesCount = resultOfGettingDevices.Result.TotalCount,
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
