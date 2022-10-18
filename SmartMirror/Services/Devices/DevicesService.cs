using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;

namespace SmartMirror.Services.Devices
{
    public class DevicesService : BaseAqaraService, IDevicesService
    {
        private const string DEFAULT_IMAGE = "grey_question_mark";
        private const int UDP_PORT = 7834;

        private readonly ConcurrentDictionary<string, IEnumerable<AttributeAqaraResponse>> _devicesAttributes = new();
        //Contains only unique device ids
        private readonly ConcurrentDictionary<string, DeviceBindableModel> _allDevices = new();

        private readonly IMapperService _mapperService;

        public DevicesService(
            IMapperService mapperService,
            IRestService restService,
            ISettingsManager settingsManager)
            : base(restService, settingsManager)
        {
            _mapperService = mapperService;
            Task.Run(ReceiveMessageAsync);
        }

        private async Task ReceiveMessageAsync()
        {
            var broadcastIPAddress = IPAddress.Broadcast.ToString();
            var receiver = new UdpClient(UDP_PORT); // UdpClient для получения данных

            try
            {
                while (true)
                {
                    var data = await receiver.ReceiveAsync(); // получаем данные
                    var message = Encoding.UTF8.GetString(data.Buffer);
                    Console.WriteLine("Собеседник: {0}", message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }

        #region -- IDevicesService implementation --

        //Can contain repeatable device ids
        public List<DeviceBindableModel> AllObservableDevicesCollection { get; private set; } = new();

        public Task<AOResult> DownloadAllDevicesWithSubInfoAsync(string positionId = null, int pageNum = 1, int pageSize = 100)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var resultOfGettingDevices = await GetDevicesAsync();

                if (resultOfGettingDevices.IsSuccess)
                {
                    var bindableModels = _mapperService.MapRange<DeviceBindableModel>(resultOfGettingDevices.Result.Data);

                    foreach (var bindableDevice in bindableModels)
                    {
                        _allDevices[bindableDevice.DeviceId] = bindableDevice;

                        if (bindableDevice.Model.Contains("switch"))
                        {
                            await GetSubdevicesForDeviceAsync(bindableDevice.DeviceId);
                        }
                    }

                    await SetAttributesForDevicesAsync(bindableModels);
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

                return response.Result;
            });
        }

        #endregion

        #region -- Private helpers --

        private string GetIconSourceForDevice(DeviceBindableModel device)
        {
            var attributesId = _devicesAttributes[device.DeviceId].Select(x => x.ResourceId);

            return device switch
            {
                _ when device.Model.Contains("switch") => GetImageSourceForSwitch(device),
                _ when device.Model.Contains("weather") => GetImageSourceForSwitch(device),
                _ => DEFAULT_IMAGE,
            };
        }

        private string GetImageSourceForSwitch(DeviceBindableModel device)
        {
            string result = DEFAULT_IMAGE;

            var attributesId = _devicesAttributes[device.DeviceId].Select(x => x.ResourceId);

            if (attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS)
                && attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS)
                && attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_2_STATUS))
            {
                result = device.EditableResourceId switch
                {
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS => "pic_wall_switch_three_left",
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS => "pic_wall_switch_three_center",
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_2_STATUS => "pic_wall_switch_three_right",
                    _ => DEFAULT_IMAGE,
                };
            }
            else if (attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS)
                && attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS))
            {
                result = device.EditableResourceId switch
                {
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS => "pic_wall_switch_double_left",
                    Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS => "pic_wall_switch_double_right",
                    _ => DEFAULT_IMAGE,
                };
            }
            else if (attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS))
            {
                result = "pic_wall_switch_single";
            }

            return result;
        }

        private string GetImageSourceForWeather(DeviceBindableModel device)
        {
            string result = DEFAULT_IMAGE;

            result = device.EditableResourceId switch
            {
                Constants.Aqara.AttibutesId.HUMIDITY_STATUS => "pic_humidity",
                Constants.Aqara.AttibutesId.TEMPERATURE_STATUS => "pic_pressure",
                Constants.Aqara.AttibutesId.AIR_PRESSURE_STATUS => "pic_temperature",
                _ => DEFAULT_IMAGE,
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
        private Task GetTaskForDevice(DeviceBindableModel device)
        {
            var availableAttributes = _devicesAttributes[device.DeviceId];

            return device switch
            {
                _ when availableAttributes.Any(x => x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS) => AddSwitchAsync(device),
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

        private async Task AddSwitchAsync(DeviceBindableModel device)
        {
            device.DeviceType = EDeviceType.Switcher;

            var attributesId = _devicesAttributes[device.DeviceId].Select(x => x.ResourceId);

            if (device.State > 0)
            {
                if (attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS)
                    && attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS)
                    && attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_2_STATUS))
                {
                    var deviceAttributeResponse = await GetDeviceAttributeValueAsync(device.DeviceId,
                        Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS,
                        Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS,
                        Constants.Aqara.AttibutesId.SWITCH_CHANNEL_2_STATUS);

                    if (deviceAttributeResponse.IsSuccess)
                    {
                        var d1 = _mapperService.Map<DeviceBindableModel>(device);
                        d1.EditableResourceId = Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS;
                        d1.Status = deviceAttributeResponse.Result?.FirstOrDefault(x => x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS)?.Value == "0" ? EDeviceStatus.Off : EDeviceStatus.On;
                        d1.IconSource = GetIconSourceForDevice(device);
                    }
                }
                else if (attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS)
                    && attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS))
                {
                    var deviceAttributeResponse = await GetDeviceAttributeValueAsync(device.DeviceId,
                        Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS,
                        Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS);

                    if (deviceAttributeResponse.IsSuccess)
                    {
                        device.Status = deviceAttributeResponse.Result?.FirstOrDefault()?.Value == "0" ? EDeviceStatus.Off : EDeviceStatus.On;
                    }
                }
                else if (attributesId.Contains(Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS))
                {
                    var deviceAttributeResponse = await GetDeviceAttributeValueAsync(device.DeviceId, Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS);

                    if (deviceAttributeResponse.IsSuccess)
                    {
                        device.Status = deviceAttributeResponse.Result?.FirstOrDefault()?.Value == "0" ? EDeviceStatus.Off : EDeviceStatus.On;
                    }
                }
            }
        }

        private void AddSwitchToCollection(DeviceBindableModel device, string resourceId)
        {

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
                        EditableResourceId = "4.1.85",
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
                        EditableResourceId = "4.2.85",
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
