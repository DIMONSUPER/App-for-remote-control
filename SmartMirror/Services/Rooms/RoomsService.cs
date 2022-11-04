using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;

namespace SmartMirror.Services.Rooms
{
    public class RoomsService : IRoomsService
    {
        private readonly IAqaraService _aqaraService;
        private readonly IDevicesService _devicesService;
        private readonly IMapperService _mapperService;
        private readonly IAqaraMessanger _aqaraMessanger;

        public RoomsService(
            IAqaraService aqaraService,
            IDevicesService devicesService,
            IMapperService mapperService,
            IAqaraMessanger aqaraMessanger)
        {
            _aqaraService = aqaraService;
            _devicesService = devicesService;
            _mapperService = mapperService;
            _aqaraMessanger = aqaraMessanger;

            _aqaraMessanger.MessageReceived += OnMessageReceived;
            _aqaraMessanger.StoppedListenning += OnStoppedListenning;
            _devicesService.AllDevicesChanged += OnAllDevicesChanged;
        }

        #region -- IRoomsService implementation --

        public List<RoomBindableModel> AllRooms { get; private set; } = new();

        public event EventHandler AllRoomsChanged;

        public Task<AOResult> DownloadAllRoomsAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var rooms = Enumerable.Empty<RoomBindableModel>();

                var resultOfGettingRoomsAqara = await GetAllRoomsOfHousesAsync();

                if (resultOfGettingRoomsAqara.IsSuccess)
                {
                    rooms = resultOfGettingRoomsAqara.Result;
                }
                else
                {
                    onFailure($"Can't download all rooms: {resultOfGettingRoomsAqara.Message}");
                }

                AllRooms = new(rooms);
            });
        }

        #endregion

        #region -- Private helpers --

        private Task<AOResult<IEnumerable<RoomBindableModel>>> GetAllRoomsOfHousesAsync()
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

                return _mapperService.MapRange<RoomBindableModel>(rooms);
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
                        var roomDevices = _devicesService.AllSupportedDevices.Where(x => x.PositionId == room.PositionId && x.IsShowInRooms);
                        var count = roomDevices.Count();

                        foreach (var device in roomDevices)
                        {
                            device.RoomName = room.PositionName;
                        }

                        rooms.Add(new RoomModel()
                        {
                            Id = room.PositionId,
                            Name = room.PositionName,
                            CreateTime = DateTime.FromBinary(room.CreateTime),
                            DevicesCount = count,
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

        private void OnAllDevicesChanged(object sender, EventArgs e)
        {
            foreach (var room in AllRooms)
            {
                var count = _devicesService.AllSupportedDevices.Count(x => x.PositionId == room.Id && x.IsShowInRooms);
                room.DevicesCount = count;
            }
        }

        private void OnMessageReceived(object sender, AqaraMessageEventArgs e)
        {
            Action<AqaraMessageEventArgs> action = e.EventType switch
            {
                Constants.Aqara.EventTypes.dev_position_assign => OnDevPositionAssigned,
                _ => x => { }
                ,
            };

            action(e);
        }

        private async void OnDevPositionAssigned(AqaraMessageEventArgs aqaraMessage)
        {
            var device = _devicesService.AllSupportedDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId && x.IsShowInRooms);
            var newRoom = AllRooms.FirstOrDefault(x => x.Id == aqaraMessage.Value);
            var oldRoom = AllRooms.FirstOrDefault(x => x.Id == device.PositionId);

            if (newRoom is null)
            {
                await DownloadAllRoomsAsync();

                newRoom = AllRooms.FirstOrDefault(x => x.Id == aqaraMessage.Value);
            }

            oldRoom.DevicesCount--;
            newRoom.DevicesCount++;
            device.PositionId = newRoom.Id;
            device.RoomName = newRoom.Name;

            AllRoomsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnStoppedListenning(object sender, EventArgs e)
        {
        }

        #endregion
    }
}
