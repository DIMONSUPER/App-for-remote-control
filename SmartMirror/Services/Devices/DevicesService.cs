using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;

namespace SmartMirror.Services.Devices
{
    public class DevicesService : BaseAqaraService, IDevicesService
    {
        private const int UDP_PORT = 7834;

        private readonly ConcurrentDictionary<string, IEnumerable<AttributeAqaraResponse>> _devicesAttributes = new();
        private readonly ConcurrentDictionary<string, DeviceBindableModel> _cachedDevices = new();
        private readonly DeviceMessanger _deviceMessanger = new();

        private readonly IMapperService _mapperService;

        public DevicesService(
            IMapperService mapperService,
            IRestService restService,
            ISettingsManager settingsManager)
            : base(restService, settingsManager)
        {
            _mapperService = mapperService;

            _deviceMessanger.StartListening();
            _deviceMessanger.MessageChangeReceived += OnMessageChangeReceived;
            _deviceMessanger.MessageDicsonnectReceived += OnMessageDicsonnectReceived;
            _deviceMessanger.MessageEventReceived += OnMessageEventReceived;
            _deviceMessanger.StoppedListenning += OnStoppedListenning;
        }

        #region -- IDevicesService implementation --

        //Can contain repeatable device ids
        public List<DeviceBindableModel> AllSupportedDevices { get; private set; } = new();

        public Task<AOResult> DownloadAllDevicesWithSubInfoAsync(string positionId = null, int pageNum = 1, int pageSize = 100)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var resultOfGettingDevices = await GetDevicesAsync();

                if (resultOfGettingDevices.IsSuccess)
                {
                    var bindableModels = _mapperService.MapRange<DeviceBindableModel>(resultOfGettingDevices.Result.Data);

                    await SetAttributesForDevicesAsync(bindableModels);

                    var result = Enumerable.Empty<DeviceBindableModel>();

                    foreach (var device in bindableModels)
                    {
                        _cachedDevices[device.DeviceId] = device;

                        var devices = await GetTaskForDevice(device);

                        result = result.Concat(devices);
                    }

                    AllSupportedDevices = new(result);
                }
                else
                {

                    onFailure("Request failed");
                }
            });
        }

        public Task<AOResult<DataAqaraResponse<DeviceAqaraModel>>> GetDevicesAsync(string positionId = null, int pageNum = 1, int pageSize = 100, params string[] deviceIds)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var data = new
                {
                    dids = deviceIds,
                    positionId = positionId,
                    pageNum = pageNum,
                    pageSize = pageSize,
                };

                var response = await MakeRequestAsync<DataAqaraResponse<DeviceAqaraModel>>("query.device.info", data, onFailure);

                return response?.Result;
            });
        }

        public Task<AOResult<BaseAqaraResponse>> UpdateAttributeValueAsync(string deviceId, params (string resourceId, string value)[] resources)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var array = resources.Select(x => new { resourceId = x.resourceId, value = x.value }).ToArray();

                var data = new[]
                {
                    new
                    {
                        subjectId = deviceId,
                        resources = array,
                    }
                };

                var response = await MakeRequestAsync("write.resource.device", data, onFailure);

                if (response is null)
                {
                    onFailure("response is null");
                }

                return response;
            });
        }

        public Task<AOResult<IEnumerable<ResourceResponse>>> GetDeviceAttributeValueAsync(string deviceId, params string[] resourceIds)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var data = new
                {
                    resources = new[]
                    {
                        new
                        {
                            subjectId = deviceId,
                            resourceIds = resourceIds,
                        }
                    }
                };

                var response = await MakeRequestAsync<IEnumerable<ResourceResponse>>("query.resource.value", data, onFailure);

                return response?.Result;
            });
        }

        public Task<AOResult<IEnumerable<DeviceAqaraModel>>> GetSubdevicesForDeviceAsync(string deviceId)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var data = new
                {
                    did = deviceId,
                };

                var response = await MakeRequestAsync<IEnumerable<DeviceAqaraModel>>("query.device.subInfo", data, onFailure);

                var a = response.JsonResult.ToString();

                return response.Result;
            });
        }

        public Task<AOResult<IEnumerable<AttributeAqaraResponse>>> GetAttributesForDeviceModel(string model, string resourceId = null)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var data = new
                {
                    model = model,
                    resourceId = resourceId,
                };

                var response = await MakeRequestAsync<IEnumerable<AttributeAqaraResponse>>("query.resource.info", data, onFailure);

                return response?.Result;
            });
        }

        public Task<AOResult<IEnumerable<AttributeNameAqaraResponse>>> GetAttributeNamesForDevices(params string[] devicesId)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var data = new
                {
                    subjectIds = devicesId,
                };

                var response = await MakeRequestAsync<IEnumerable<AttributeNameAqaraResponse>>("query.resource.name", data, onFailure);

                return response.Result;
            });
        }

        #endregion

        #region -- Private helpers --

        private bool HasDeviceNSwitches(DeviceBindableModel device, int n)
        {
            var attributes = _devicesAttributes[device.Model];

            var result = attributes.Count(x => x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS);
            result += attributes.Count(x => x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS);
            result += attributes.Count(x => x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_2_STATUS);

            return result == n;
        }

        private bool IsDeviceSwitch(DeviceBindableModel device)
        {
            return _devicesAttributes[device.Model].Any(x =>
            x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS ||
            x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS ||
            x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_2_STATUS);
        }

        private bool IsDeviceWeather(DeviceBindableModel device)
        {
            return _devicesAttributes[device.Model].Any(x => x.ResourceId == Constants.Aqara.AttibutesId.AIR_PRESSURE_STATUS)
                && _devicesAttributes[device.Model].Any(x => x.ResourceId == Constants.Aqara.AttibutesId.HUMIDITY_STATUS)
                && _devicesAttributes[device.Model].Any(x => x.ResourceId == Constants.Aqara.AttibutesId.TEMPERATURE_STATUS);
        }

        private string GetIconSourceForDevice(DeviceBindableModel device)
        {
            var attributesId = _devicesAttributes[device.Model].Select(x => x.ResourceId);

            return device switch
            {
                _ when IsDeviceSwitch(device) => GetImageSourceForSwitch(device),
                _ when IsDeviceWeather(device) => GetImageSourceForWeather(device),
                _ => IconsNames.grey_question_mark,
            };
        }

        private string GetImageSourceForSwitch(DeviceBindableModel device)
        {
            string result = IconsNames.grey_question_mark;

            var attributesId = _devicesAttributes[device.Model].Select(x => x.ResourceId);

            if (HasDeviceNSwitches(device, 3))
            {
                result = device.EditableResourceId switch
                {
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS => IconsNames.pic_wall_switch_three_left,
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS => IconsNames.pic_wall_switch_three_center,
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_2_STATUS => IconsNames.pic_wall_switch_three_right,
                    _ => IconsNames.grey_question_mark,
                };
            }
            else if (HasDeviceNSwitches(device, 2))
            {
                result = device.EditableResourceId switch
                {
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS => IconsNames.pic_wall_switch_double_left,
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS => IconsNames.pic_wall_switch_double_right,
                    _ => IconsNames.grey_question_mark,
                };
            }
            else if (HasDeviceNSwitches(device, 1))
            {
                result = IconsNames.pic_wall_switch_single;
            }

            return result;
        }

        private string GetImageSourceForWeather(DeviceBindableModel device)
        {
            var result = device.EditableResourceId switch
            {
                Constants.Aqara.AttibutesId.HUMIDITY_STATUS => IconsNames.pic_humidity,
                Constants.Aqara.AttibutesId.TEMPERATURE_STATUS => IconsNames.pic_temperature,
                Constants.Aqara.AttibutesId.AIR_PRESSURE_STATUS => IconsNames.pic_pressure,
                _ => IconsNames.grey_question_mark,
            };

            return result;
        }

        private async Task SetAttributesForDevicesAsync(IEnumerable<DeviceBindableModel> devices)
        {
            foreach (var bindableModel in devices)
            {
                if (!_devicesAttributes.ContainsKey(bindableModel.Model))
                {
                    var res = await GetAttributesForDeviceModel(bindableModel.Model);

                    if (res.IsSuccess)
                    {
                        _devicesAttributes[bindableModel.Model] = res.Result;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to get attributes for {bindableModel.Model}: {res.Message}");
                    }
                }
                else
                {
                    //Attributes for this model are already in dictionary
                }
            }
        }

        //TODO: add support for all devices
        private Task<IEnumerable<DeviceBindableModel>> GetTaskForDevice(DeviceBindableModel device)
        {
            var availableAttributes = _devicesAttributes[device.Model];

            if (IsDeviceSwitch(device))
            {
                return GetSwitchesAsync(device);
            }
            else if (IsDeviceWeather(device))
            {
                return GetWeatherAccessoriesAsync(device);
            }
            else
            {
                return Task.FromResult(Enumerable.Empty<DeviceBindableModel>());
            }
        }

        private Task<IEnumerable<DeviceBindableModel>> GetWeatherAccessoriesAsync(DeviceBindableModel device)
        {
            System.Diagnostics.Debug.WriteLine($"GetWeatherAccessoriesAsync for {device.Model}, {device.State}");

            device.DeviceType = EDeviceType.Sensor;

            return GetDevicesForAttributesAsync(device,
                Constants.Aqara.AttibutesId.HUMIDITY_STATUS,
                Constants.Aqara.AttibutesId.TEMPERATURE_STATUS,
                Constants.Aqara.AttibutesId.AIR_PRESSURE_STATUS);
        }

        private async Task<IEnumerable<DeviceBindableModel>> GetSwitchesAsync(DeviceBindableModel device)
        {
            System.Diagnostics.Debug.WriteLine($"GetSwitchesAsync for {device.Model}");

            var result = Enumerable.Empty<DeviceBindableModel>();

            device.DeviceType = EDeviceType.Switcher;

            var attributesId = _devicesAttributes[device.Model].Select(x => x.ResourceId);

            if (HasDeviceNSwitches(device, 3))
            {
                result = await GetDevicesForAttributesAsync(device,
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS,
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS,
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_2_STATUS);
            }
            else if (HasDeviceNSwitches(device, 2))
            {
                result = await GetDevicesForAttributesAsync(device,
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS,
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS);
            }
            else if (HasDeviceNSwitches(device, 1))
            {
                result = await GetDevicesForAttributesAsync(device, Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS);
            }

            return result;
        }

        private async Task<IEnumerable<DeviceBindableModel>> GetDevicesForAttributesAsync(DeviceBindableModel device, params string[] resouceIds)
        {
            var result = new List<DeviceBindableModel>();

            var namesResponse = await GetAttributeNamesForDevices(device.DeviceId);

            if (namesResponse.IsSuccess)
            {
                var names = namesResponse.Result.ToDictionary(x => x.ResourceId);
                var deviceAttributeResponse = await GetDeviceAttributeValueAsync(device.DeviceId, resouceIds);

                if (deviceAttributeResponse.IsSuccess)
                {
                    var attributes = deviceAttributeResponse.Result.ToDictionary(x => x.ResourceId);

                    foreach (var id in resouceIds)
                    {
                        result.Add(CreateNewDevice(device, attributes[id].ResourceId, names[id].Name, attributes[id].Value));
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Can't get attributes for {device.Model}, {device.DeviceId}");

                    foreach (var id in resouceIds)
                    {
                        result.Add(CreateNewDevice(device, id, names[id].Name, "Fail"));
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Can't get names for {device.DeviceId}");
            }

            return result;
        }

        private DeviceBindableModel CreateNewDevice(DeviceBindableModel device, string resourceId, string name, string info = null)
        {
            var newDevice = new DeviceBindableModel()
            {
                DeviceId = device.DeviceId,
                DeviceType = device.DeviceType,
                PositionId = device.PositionId,
                TappedCommand = device.TappedCommand,
                State = device.State,
                Model = device.Model,
                ModelType = device.ModelType,
                Id = device.Id,
                IconSource = device.IconSource,
                Name = name,
                EditableResourceId = resourceId,
                AdditionalInfo = info,
            };

            newDevice.IconSource = GetIconSourceForDevice(newDevice);

            return newDevice;
        }

        private void OnStoppedListenning(object sender, EventArgs e)
        {
        }

        private void OnMessageEventReceived(object sender, MessageEventResponse messageEventResponse)
        {
            foreach (var messageEvent in messageEventResponse.Data)
            {
                System.Diagnostics.Debug.WriteLine("SubjectId: {0}, ResourceId: {1}, Value: {2}", messageEvent.SubjectId, messageEvent.ResourceId, messageEvent.Value);

                var devicesWithId = AllSupportedDevices.Where(x => x.DeviceId == messageEvent.SubjectId);

                foreach (var device in devicesWithId)
                {
                    if (device.EditableResourceId == messageEvent.ResourceId)
                    {
                        device.AdditionalInfo = messageEvent.Value;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Found: {devicesWithId.Count()}");
            }
        }

        private void OnMessageDicsonnectReceived(object sender, MessageDicsonnectReponse e)
        {
        }

        private void OnMessageChangeReceived(object sender, MessageChangeResponse messageChangeResponse)
        {
            var devicesWithId = AllSupportedDevices.Where(x => x.DeviceId == messageChangeResponse.Data.Did);

            foreach (var changeValue in messageChangeResponse.Data.ChangeValues)
            {
                var device = devicesWithId.FirstOrDefault(x => x.EditableResourceId == changeValue.ResourceId);

                if (device is not null)
                {
                    device.Name = changeValue.Name;
                }

                System.Diagnostics.Debug.WriteLine("Name: {0}, ResourceId: {1}", changeValue.Name, changeValue.ResourceId);
            }
        }

        #endregion
    }
}
