
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.DataTemplates;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;

namespace SmartMirror.Services.Devices
{
    public class DevicesService : BaseAqaraService, IDevicesService
    {
        private readonly ConcurrentDictionary<string, IEnumerable<AttributeAqaraResponse>> _attributes = new();
        private readonly ConcurrentDictionary<string, DeviceBindableModel> _allDevices = new();

        private readonly IMapperService _mapperService;

        public DevicesService(
            IMapperService mapperService,
            IRestService restService,
            ISettingsManager settingsManager)
            : base(restService, settingsManager)
        {
            _mapperService = mapperService;
        }

        #region -- IDevicesService implementation --

        public List<DeviceBindableModel> AllObservableDevicesCollection { get; private set; }

        public Task<AOResult> DownloadAllDevicesWithSubInfoAsync(string positionId = null, int pageNum = 1, int pageSize = 100)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var resultOfGettingDevices = await GetDevicesAsync();

                if (resultOfGettingDevices.IsSuccess)
                {
                    var bindableModels = _mapperService.MapRange<DeviceBindableModel>(resultOfGettingDevices.Result.Data);

                    await SetAttributesForDevicesAsync(bindableModels);

                    foreach (var bindableDevice in bindableModels)
                    {
                        _allDevices[bindableDevice.DeviceId] = bindableDevice;
                    }
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

                return response.Result;
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

                return response.Result;
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
                var a = response.JsonResult.ToString();
                return response.Result;
            });
        }

        #endregion

        #region -- Private helpers --

        private async Task SetAttributesForDevicesAsync(IEnumerable<DeviceBindableModel> devices)
        {
            foreach (var bindableModel in devices)
            {
                if (!_attributes.ContainsKey(bindableModel.Model))
                {
                    var res = await GetAttributesForDeviceModel(bindableModel.Model);

                    if (res.IsSuccess)
                    {
                        _attributes[bindableModel.Model] = res.Result;
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
        private Task GetTaskForDevice(DeviceBindableModel device)
        {
            return device switch
            {
                _ when device.Model.Contains("switch.l") => AddLampAsync(device),
                _ when device.Model.Contains("switch.b") => AddDoubleLampAsync(device),
                _ when device.Model.Contains("weather") => AddWeatherAccessoriesAsync(device),
                _ when device.Model.Contains("gateway") || device.Model.Contains("sensor_motion") => Task.CompletedTask,
            };
        }

        private async Task AddWeatherAccessoriesAsync(DeviceBindableModel device)
        {
            if (device.State > 0)
            {
                var deviceAttributeResponse = await GetDeviceAttributeValueAsync(device.DeviceId, "0.2.85", "0.1.85", "0.3.85");

                if (deviceAttributeResponse.IsSuccess)
                {
                    device.Status = EDeviceStatus.Connected;

                    var temperature = double.Parse(deviceAttributeResponse.Result.FirstOrDefault(x => x.ResourceId == "0.1.85").Value) / 100 + "℃";
                    var humididty = double.Parse(deviceAttributeResponse.Result.FirstOrDefault(x => x.ResourceId == "0.2.85").Value) / 100 + "%";
                    var pressure = double.Parse(deviceAttributeResponse.Result.FirstOrDefault(x => x.ResourceId == "0.3.85").Value) / 1000 + "kPa";

                    AddWeatherDevice(device.DeviceId, humididty, "Humidity", "pic_humidity");
                    AddWeatherDevice(device.DeviceId, temperature, "Temperature", "pic_temperature");
                    AddWeatherDevice(device.DeviceId, pressure, "Pressure", "pic_pressure");
                }
            }
        }

        private void AddWeatherDevice(string did, string additionalInfo, string name, string iconSource)
        {
            _allDevices[did] = new DeviceBindableModel()
            {
                AdditionalInfo = additionalInfo,
                Status = EDeviceStatus.Connected,
                DeviceType = EDeviceType.Sensor,
                Name = name,
                IconSource = iconSource,
            };
        }

        private async Task AddLampAsync(DeviceBindableModel device)
        {
            device.DeviceType = EDeviceType.Switcher;
            device.IconSource = "lamp";
            device.EditableResource = "4.1.85";

            if (device.State > 0)
            {
                var deviceAttributeResponse = await GetDeviceAttributeValueAsync(device.DeviceId, "4.1.85");

                if (deviceAttributeResponse.IsSuccess)
                {
                    device.Status = deviceAttributeResponse.Result?.FirstOrDefault()?.Value == "0" ? EDeviceStatus.Off : EDeviceStatus.On;
                }

                _allDevices[device.DeviceId] = device;
            }
        }

        private async Task AddDoubleLampAsync(DeviceBindableModel device)
        {
            if (device.State > 0)
            {
                var leftDeviceAttributeResponse = await GetDeviceAttributeValueAsync(device.DeviceId, "4.1.85");

                DeviceBindableModel leftDevice = null;
                DeviceBindableModel rightDevice = null;

                if (leftDeviceAttributeResponse.IsSuccess)
                {
                    leftDevice = new DeviceBindableModel()
                    {
                        DeviceId = device.DeviceId,
                        Status = leftDeviceAttributeResponse.Result?.FirstOrDefault()?.Value == "0" ? EDeviceStatus.Off : EDeviceStatus.On,
                        DeviceType = EDeviceType.Switcher,
                        Name = "Left Lamp",
                        IconSource = "lamp",
                        EditableResource = "4.1.85",
                    };

                }

                var rightDeviceAttributeResponse = await GetDeviceAttributeValueAsync(device.DeviceId, "4.2.85");

                if (rightDeviceAttributeResponse.IsSuccess)
                {
                    rightDevice = new DeviceBindableModel()
                    {
                        DeviceId = device.DeviceId,
                        Status = rightDeviceAttributeResponse.Result?.FirstOrDefault()?.Value == "0" ? EDeviceStatus.Off : EDeviceStatus.On,
                        DeviceType = EDeviceType.Switcher,
                        Name = "Right Lamp",
                        IconSource = "lamp",
                        EditableResource = "4.2.85",
                    };
                }

                if (leftDevice is not null)
                {
                    _allDevices[leftDevice.DeviceId] = leftDevice;
                }

                if (rightDevice is not null)
                {
                    _allDevices[rightDevice.DeviceId] = rightDevice;
                }
            }
        }

        #endregion
    }
}
