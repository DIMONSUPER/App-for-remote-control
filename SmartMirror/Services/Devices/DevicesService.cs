using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.DTO;
using SmartMirror.Resources;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Repository;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;

namespace SmartMirror.Services.Devices
{
    public class DevicesService : BaseAqaraService, IDevicesService
    {
        private readonly Dictionary<string, IEnumerable<AttributeAqaraDTO>> _devicesAttributes = new();
        private readonly Dictionary<string, DeviceBindableModel> _cachedDevices = new();

        private readonly IMapperService _mapperService;
        private readonly IRepositoryService _repositoryService;
        private readonly IAqaraMessanger _aqaraMessanger;

        private List<AttributeNameAqaraResponse> _attributeNames = new();

        public DevicesService(
            IMapperService mapperService,
            IRestService restService,
            ISettingsManager settingsManager,
            IRepositoryService repositoryService,
            IAqaraMessanger aqaraMessanger)
            : base(restService, settingsManager)
        {
            _mapperService = mapperService;
            _repositoryService = repositoryService;
            _aqaraMessanger = aqaraMessanger;

            _aqaraMessanger.MessageReceived += OnMessageReceived;
            _aqaraMessanger.StoppedListenning += OnStoppedListenning;
        }

        #region -- IDevicesService implementation --

        //Can't contain repeatable device ids
        public List<DeviceBindableModel> AllDevices { get; private set; } = new();

        //Can contain repeatable device ids
        public List<DeviceBindableModel> AllSupportedDevices { get; private set; } = new();

        public event EventHandler AllDevicesChanged;

        public Task<AOResult> DownloadAllDevicesWithSubInfoAsync(string positionId = null, int pageNum = 1, int pageSize = 100)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var resultOfGettingDevices = await GetDevicesAsync();

                if (resultOfGettingDevices.IsSuccess)
                {
                    var bindableModels = _mapperService.MapRange<DeviceBindableModel>(resultOfGettingDevices.Result.Data);

                    await SetAttributesForDevicesAsync(bindableModels);

                    await SetAttributeNamesForDevicesAsync(bindableModels);

                    var result = Enumerable.Empty<DeviceBindableModel>();

                    foreach (var device in bindableModels)
                    {
                        _cachedDevices[device.DeviceId] = device;

                        var devices = await GetTaskForDevice(device);

                        await GetSettingsDevicesAsync(devices);

                        result = result.Concat(devices);
                    }

                    AllDevices = new(_cachedDevices.Select(x => x.Value));
                    AllSupportedDevices = new(result);

                    var dbModels = _mapperService.MapRange<DeviceDTO>(AllSupportedDevices);

                    await _repositoryService.SaveOrUpdateRangeAsync(dbModels);

                    AllDevicesChanged?.Invoke(this, EventArgs.Empty);
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

        public Task<AOResult> UpdateDeviceAsync(DeviceBindableModel bindableDevice)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var updateDevice = _mapperService.Map<DeviceDTO>(bindableDevice);

                var response = await _repositoryService.SaveOrUpdateAsync(updateDevice);

                if (response == -1)
                {
                    onFailure("Update failed");
                }
                else
                {
                    var bindableDeviceId = bindableDevice.Id;

                    var device = AllSupportedDevices.FirstOrDefault(row => row.Id == bindableDeviceId);

                    device = bindableDevice;

                    AllDevicesChanged?.Invoke(this, EventArgs.Empty);
                }
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

        private async Task GetSettingsDevicesAsync(IEnumerable<DeviceBindableModel> devices)
        {
            foreach (var device in devices)
            {
                var deviceId = device.DeviceId;
                var resourceId = device.EditableResourceId;

                var dbDevice = await _repositoryService.GetSingleAsync<DeviceDTO>(row => row.DeviceId == deviceId && row.EditableResourceId == resourceId);

                if (dbDevice is not null)
                {
                    device.Id = dbDevice.Id;
                    device.UnitMeasure = dbDevice.UnitMeasure;
                    device.IsShowInRooms = dbDevice.IsShowInRooms;
                    device.IsReceiveNotifications = dbDevice.IsReceiveNotifications;
                    device.IsFavorite = dbDevice.IsFavorite;
                }
            }
        }

        private bool HasDeviceNSwitches(DeviceBindableModel device, int n)
        {
            var attributes = _devicesAttributes[device.Model];

            var result = attributes.Count(x => x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_0_STATUS);
            result += attributes.Count(x => x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_1_STATUS);
            result += attributes.Count(x => x.ResourceId == Constants.Aqara.AttibutesId.SWITCH_CHANNEL_2_STATUS);

            return result == n;
        }

        private string GetIconSourceForDevice(DeviceBindableModel device)
        {
            var attributesId = _devicesAttributes[device.Model].Select(x => x.ResourceId);

            return device switch
            {
                _ when DeviceHelper.IsDeviceSwitch(device) => GetImageSourceForSwitch(device),
                _ when DeviceHelper.IsDeviceWeather(device) => DeviceHelper.GetImageSourceForWeather(device),
                _ when DeviceHelper.IsDeviceMotionSensor(device) => DeviceHelper.GetImageSourceForMotionSensor(device),
                _ when DeviceHelper.IsDeviceHub(device) => DeviceHelper.GetImageSourceForHub(device),
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

        private async Task SetAttributeNamesForDevicesAsync(IEnumerable<DeviceBindableModel> devices)
        {
            var attributeNamesResponse = await GetAttributeNamesForDevices(devices.Select(x => x.DeviceId).ToArray());

            if (attributeNamesResponse.IsSuccess)
            {
                _attributeNames = new(attributeNamesResponse.Result);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Can't download attribute names");
            }
        }

        private async Task SetAttributesForDevicesAsync(IEnumerable<DeviceBindableModel> devices)
        {
            var downloadedAttributes = await _repositoryService.GetAllAsync<AttributeAqaraDTO>();

            foreach (var bindableModel in devices)
            {
                if (!_devicesAttributes.ContainsKey(bindableModel.Model))
                {
                    if (downloadedAttributes.Any(x => x.Model == bindableModel.Model))
                    {
                        var modelAttributes = downloadedAttributes.Where(x => x.Model == bindableModel.Model);

                        _devicesAttributes[bindableModel.Model] = modelAttributes;
                    }
                    else
                    {
                        var res = await GetAttributesForDeviceModel(bindableModel.Model);

                        if (res.IsSuccess)
                        {
                            var attributesDTO = _mapperService.MapRange<AttributeAqaraDTO>(res.Result);

                            await _repositoryService.SaveOrUpdateRangeAsync(attributesDTO);

                            _devicesAttributes[bindableModel.Model] = attributesDTO;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to get attributes for {bindableModel.Model}: {res.Message}");
                        }
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
            if (DeviceHelper.IsDeviceSwitch(device))
            {
                return GetSwitchesAsync(device);
            }
            else if (DeviceHelper.IsDeviceWeather(device))
            {
                return GetWeatherAccessoriesAsync(device);
            }
            else if (DeviceHelper.IsDeviceMotionSensor(device))
            {
                return GetMotionSensorsAsync(device);
            }
            else if (DeviceHelper.IsDeviceHub(device))
            {
                return GetHubAsync(device);
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

            var attributeNames = _attributeNames.Where(x => x.SubjectId == device.DeviceId).ToArray();

            return GetDevicesForAttributesAsync(device, attributeNames);
        }

        private Task<IEnumerable<DeviceBindableModel>> GetHubAsync(DeviceBindableModel device)
        {
            System.Diagnostics.Debug.WriteLine($"GetHubAsync for {device.Model}, {device.State}");

            device.DeviceType = EDeviceType.Sensor;

            return GetDevicesForAttributesAsync(device);
        }

        private Task<IEnumerable<DeviceBindableModel>> GetMotionSensorsAsync(DeviceBindableModel device)
        {
            System.Diagnostics.Debug.WriteLine($"GetMotionSensorsAsync for {device.Model}, {device.State}");

            device.DeviceType = EDeviceType.Sensor;

            var attributeNames = _attributeNames.Where(x => x.SubjectId == device.DeviceId).ToArray();//Ignore motion status

            return GetDevicesForAttributesAsync(device, attributeNames);
        }

        private async Task<IEnumerable<DeviceBindableModel>> GetSwitchesAsync(DeviceBindableModel device)
        {
            System.Diagnostics.Debug.WriteLine($"GetSwitchesAsync for {device.Model}");

            var result = Enumerable.Empty<DeviceBindableModel>();

            device.DeviceType = EDeviceType.Switcher;

            var attributesId = _devicesAttributes[device.Model].Select(x => x.ResourceId);

            var attributeNames = _attributeNames.Where(x => x.SubjectId == device.DeviceId).ToArray();

            result = await GetDevicesForAttributesAsync(device, attributeNames);

            return result;
        }

        private async Task<IEnumerable<DeviceBindableModel>> GetDevicesForAttributesAsync(DeviceBindableModel device, params AttributeNameAqaraResponse[] resources)
        {
            var result = new List<DeviceBindableModel>();

            if (resources is null || !resources.Any())
            {
                result.Add(CreateNewDevice(device, null, device.Name));
            }
            else
            {
                var deviceAttributeResponse = await GetDeviceAttributeValueAsync(device.DeviceId, resources.Select(x => x.ResourceId).ToArray());

                if (deviceAttributeResponse.IsSuccess)
                {
                    foreach (var resource in resources)
                    {
                        var value = deviceAttributeResponse.Result?.FirstOrDefault(x => x.ResourceId == resource.ResourceId)?.Value;
                        result.Add(CreateNewDevice(device, resource.ResourceId, resource.Name, value));
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Can't get attribute values for {device.Model}, {device.DeviceId}");

                    foreach (var resource in resources)
                    {
                        result.Add(CreateNewDevice(device, resource.ResourceId, resource.Name, "Fail"));
                    }
                }
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
                ParentDid = device.ParentDid,
                CreateTime = device.CreateTime,
                RoomName = device.RoomName,
                TimeZone = device.TimeZone,
                UpdateTime = device.UpdateTime,
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

            if (newDevice.IconSource == IconsNames.pic_temperature)
            {
                newDevice.UnitMeasure = EUnitMeasure.Fahrenheit;
            }

            return newDevice;
        }

        private void OnMessageReceived(object sender, AqaraMessageEventArgs e)
        {
            Action<AqaraMessageEventArgs> action = e.EventType switch
            {
                Constants.Aqara.EventTypes.resource_alias_changed => OnResourceAliasChanged,
                Constants.Aqara.EventTypes.resource_report => OnReousrceReported,
                Constants.Aqara.EventTypes.dev_name_change => OnDeviceNameChanged,
                Constants.Aqara.EventTypes.gateway_online => OnGatewayOnline,
                Constants.Aqara.EventTypes.gateway_offline => OnGatewayOffline,
                Constants.Aqara.EventTypes.gateway_bind => OnGatewayBind,
                Constants.Aqara.EventTypes.gateway_unbind => OnGatewayUnbind,
                Constants.Aqara.EventTypes.subdevice_offline => OnSubdeviceOffline,
                Constants.Aqara.EventTypes.subdevice_online => OnSubdeviceOnline,
                Constants.Aqara.EventTypes.subdevice_bind => OnSubdeviceBind,
                Constants.Aqara.EventTypes.unbind_sub_gw => OnSubdeviceUnbind,
                _ => x => { }
                ,
            };

            action(e);
        }

        private void OnSubdeviceUnbind(AqaraMessageEventArgs aqaraMessage)
        {
            AllSupportedDevices.RemoveAll(x => x.DeviceId == aqaraMessage.DeviceId);
            AllDevices.RemoveAll(x => x.DeviceId == aqaraMessage.DeviceId);

            AllDevicesChanged?.Invoke(this, EventArgs.Empty);
        }

        private async void OnSubdeviceBind(AqaraMessageEventArgs aqaraMessage)
        {
            await DownloadAllDevicesWithSubInfoAsync();

            AllDevicesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnSubdeviceOffline(AqaraMessageEventArgs aqaraMessage)
        {
            var devices = AllSupportedDevices.Where(x => x.DeviceId == aqaraMessage.DeviceId);

            if (devices.Any())
            {
                foreach (var supportedDevice in devices)
                {
                    supportedDevice.State = 0;
                }
            }

            var device = AllDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId);

            if (device is not null)
            {
                device.State = 0;
            }
        }

        private void OnSubdeviceOnline(AqaraMessageEventArgs aqaraMessage)
        {
            var devices = AllSupportedDevices.Where(x => x.DeviceId == aqaraMessage.DeviceId);

            if (devices.Any())
            {
                foreach (var supportedDevice in devices)
                {
                    supportedDevice.State = 1;
                }
            }

            var device = AllDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId);

            if (device is not null)
            {
                device.State = 1;
            }
        }

        private async void OnGatewayBind(AqaraMessageEventArgs aqaraMessage)
        {
            var gateway = AllSupportedDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId)
                ?? AllDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId);

            if (gateway is null)
            {
                await DownloadAllDevicesWithSubInfoAsync();

                AllDevicesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnGatewayUnbind(AqaraMessageEventArgs aqaraMessage)
        {
            var gateway = AllSupportedDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId)
                ?? AllDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId);

            if (gateway is not null)
            {
                AllSupportedDevices.Remove(gateway);
                AllSupportedDevices.RemoveAll(x => x.ParentDid == gateway.DeviceId);

                AllDevices.Remove(gateway);
                AllDevices.RemoveAll(x => x.ParentDid == gateway.DeviceId);

                AllDevicesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private async void OnGatewayOnline(AqaraMessageEventArgs aqaraMessage)
        {
            var gateway = AllSupportedDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId);

            if (gateway is not null)
            {
                gateway.State = 1;

                var subDevices = AllSupportedDevices.Where(x => x.ParentDid == gateway.DeviceId);
                var updatedSubdevicesRespose = await GetSubdevicesForDeviceAsync(gateway.DeviceId);

                if (updatedSubdevicesRespose.IsSuccess)
                {
                    await UpdateDeviceAsync(subDevices, updatedSubdevicesRespose.Result);
                }
            }
        }

        private async Task UpdateDeviceAsync(IEnumerable<DeviceBindableModel> oldDevices, IEnumerable<DeviceAqaraModel> newDevices)
        {
            foreach (var device in newDevices)
            {
                var existingDevices = oldDevices.Where(x => x.DeviceId == device.Did);
                var deviceResources = existingDevices.Where(x => x.EditableResourceId is not null).Select(x => x.EditableResourceId).ToArray();

                if (existingDevices.Any())
                {
                    foreach (var existingDevice in existingDevices)
                    {
                        existingDevice.State = device.State;
                    }

                    if (deviceResources.Length != 0)
                    {
                        var deviceAttributeResponse = await GetDeviceAttributeValueAsync(device.Did, deviceResources);

                        if (deviceAttributeResponse.IsSuccess)
                        {
                            foreach (var resourceResponse in deviceAttributeResponse.Result)
                            {
                                var resourceDevice = existingDevices.FirstOrDefault(x => x.EditableResourceId == resourceResponse.ResourceId);
                                resourceDevice.AdditionalInfo = resourceResponse.Value;
                            }
                        }
                    }
                }
            }
        }

        private void OnGatewayOffline(AqaraMessageEventArgs aqaraMessage)
        {
            var gateway = AllSupportedDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId);

            if (gateway is not null)
            {
                gateway.State = 0;

                foreach (var device in AllSupportedDevices)
                {
                    if (device.ParentDid == gateway.DeviceId)
                    {
                        device.State = 0;
                    }
                }
            }
        }

        private void OnDeviceNameChanged(AqaraMessageEventArgs aqaraMessage)
        {
            if (AllSupportedDevices.Any(x => x.DeviceId == aqaraMessage.DeviceId))
            {
                if (!string.IsNullOrWhiteSpace(aqaraMessage.ResourceId))
                {
                    var deviceAttribute = AllSupportedDevices.FirstOrDefault(x => x.EditableResourceId == aqaraMessage.ResourceId);

                    if (deviceAttribute is not null)
                    {
                        deviceAttribute.Name = aqaraMessage.Value;
                    }
                }
                else
                {
                    var device = AllSupportedDevices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId);

                    if (device is not null)
                    {
                        device.Name = aqaraMessage.Value;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"{aqaraMessage.DeviceId}: resource {aqaraMessage.ResourceId} was changed to {aqaraMessage.Value}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Device {aqaraMessage.DeviceId} was not found");
            }
        }

        private void OnReousrceReported(AqaraMessageEventArgs aqaraMessage)
        {
            var device = AllSupportedDevices.Where(x => x.DeviceId == aqaraMessage.DeviceId).FirstOrDefault(x => x.EditableResourceId == aqaraMessage.ResourceId);

            if (device is not null)
            {
                device.AdditionalInfo = aqaraMessage.Value;
                System.Diagnostics.Debug.WriteLine($"{device.Model}: resourceName {device.Name} was changed to {aqaraMessage.Value}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Device {aqaraMessage.DeviceId} was not found");
            }
        }

        private void OnResourceAliasChanged(AqaraMessageEventArgs aqaraMessage)
        {
            var device = AllSupportedDevices.Where(x => x.DeviceId == aqaraMessage.DeviceId).FirstOrDefault(x => x.EditableResourceId == aqaraMessage.ResourceId);

            if (device is not null)
            {
                device.Name = aqaraMessage.Value;
                System.Diagnostics.Debug.WriteLine($"{device.Model}: resource {device.EditableResourceId} was changed to {aqaraMessage.Value}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Device {aqaraMessage.DeviceId} was not found");
            }
        }

        private void OnStoppedListenning(object sender, EventArgs e)
        {
        }

        #endregion
    }
}
