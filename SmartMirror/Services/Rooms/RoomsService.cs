using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
namespace SmartMirror.Services.Rooms
{
    public class RoomsService : IRoomsService
    {
        private readonly IAqaraService _aqaraService;
        private readonly IDevicesService _devicesService;
        private readonly IMapperService _mapperService;
        private readonly IAqaraMessanger _aqaraMessanger;

        private TaskCompletionSource<object> _roomsTaskCompletionSource = new();
        private List<RoomBindableModel> _allRooms = new();

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

        public event EventHandler AllRoomsChanged;

        public async Task<IEnumerable<RoomBindableModel>> GetAllRoomsAsync()
        {
            await _roomsTaskCompletionSource.Task;

            return _allRooms;
        }

        public async Task<AOResult> DownloadAllRoomsAsync()
        {
            var result = new AOResult();
            result.SetFailure("Task is already running");

            if (_roomsTaskCompletionSource.Task.Status is not TaskStatus.RanToCompletion and not TaskStatus.WaitingForActivation and not TaskStatus.Canceled and not TaskStatus.Faulted)
            {
                return result;
            }

            System.Diagnostics.Debug.WriteLine($"{nameof(DownloadAllRoomsAsync)} STARTED");

            result = await AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var rooms = Enumerable.Empty<RoomBindableModel>();

                var resultOfGettingRoomsAqara = await GetAllRoomsOfHousesAsync();

                if (resultOfGettingRoomsAqara.IsSuccess)
                {
                    rooms = resultOfGettingRoomsAqara.Result;
                    _allRooms = new(rooms);
                }
                else
                {
                    onFailure($"Can't download all rooms: {resultOfGettingRoomsAqara.Message}");
                }
            });

            if (!result.IsSuccess)
            {
                _allRooms = new();
            }

            AllRoomsChanged?.Invoke(this, EventArgs.Empty);

            System.Diagnostics.Debug.WriteLine($"{nameof(DownloadAllRoomsAsync)} FINISHED");

            _roomsTaskCompletionSource.TrySetResult(null);

            return result;
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
                    var devices = await _devicesService.GetAllSupportedDevicesAsync();

                    foreach (var room in simpleRooms)
                    {
                        var roomDevices = devices.Where(x => x.PositionId == room.PositionId && x.IsShownInRooms);
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

        private async void OnAllDevicesChanged(object sender, DeviceBindableModel device)
        {
            var devices = await _devicesService.GetAllSupportedDevicesAsync();

            foreach (var room in _allRooms)
            {
                var count = devices.Count(x => x.PositionId == room.Id && x.IsShownInRooms);
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
            var devices = await _devicesService.GetAllSupportedDevicesAsync();
            var device = devices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId && x.IsShownInRooms);
            var newRoom = _allRooms.FirstOrDefault(x => x.Id == aqaraMessage.Value);
            var oldRoom = _allRooms.FirstOrDefault(x => x.Id == device.PositionId);

            if (newRoom is null)
            {
                await DownloadAllRoomsAsync();

                newRoom = _allRooms.FirstOrDefault(x => x.Id == aqaraMessage.Value);
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
