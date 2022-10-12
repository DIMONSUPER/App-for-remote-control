using AutoMapper;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Mapper
{
    public class MapperService : IMapperService
    {
        private readonly Lazy<IMapper> _lazyMapper;

        public MapperService()
        {
            _lazyMapper = new Lazy<IMapper>(ConfigMapper);
        }

        #region -- IMapperService Implementation --

        public Task<IMapper> GetMapperAsync() => Task.Run(() => _lazyMapper.Value);

        public Task<OutT> MapAsync<InT, OutT>(InT source)
        {
            return Task.Run(() => _lazyMapper.Value.Map<InT, OutT>(source));
        }

        public Task<OutT> MapAsync<InT, OutT>(InT source, Action<InT, OutT> afterMap)
        {
            return Task.Run(() => _lazyMapper.Value.Map<InT, OutT>(source, opt => opt.AfterMap(afterMap)));
        }

        public Task<IEnumerable<OutT>> MapRangeAsync<InT, OutT>(IEnumerable<InT> items)
        {
            return Task.Run(() => items.Select(x => _lazyMapper.Value.Map<InT, OutT>(x)));
        }

        public Task<IEnumerable<OutT>> MapRangeAsync<InT, OutT>(IEnumerable<InT> items, Action<InT, OutT> afterMap)
        {
            return Task.Run(() => items.Select(x => _lazyMapper.Value.Map<InT, OutT>(x, opt => opt.AfterMap(afterMap))));
        }

        public Task<T> MapAsync<T>(object source)
        {
            return Task.Run(() => _lazyMapper.Value.Map<T>(source));
        }

        public Task<T> MapAsync<T>(object source, Action<object, T> afterMap)
        {
            return Task.Run(() => _lazyMapper.Value.Map<T>(source, opt => opt.AfterMap(afterMap)));
        }

        public Task<IEnumerable<T>> MapRangeAsync<T>(IEnumerable<object> items)
        {
            return Task.Run(() => items.Select(x => _lazyMapper.Value.Map<T>(x)));
        }

        public Task<IEnumerable<T>> MapRangeAsync<T>(IEnumerable<object> items, Action<object, T> afterMap)
        {
            return Task.Run(() => items.Select(x => _lazyMapper.Value.Map<T>(x, opt => opt.AfterMap(afterMap))));
        }

        #endregion

        #region -- Private Helpers --

        private IMapper ConfigMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Room, RoomBindableModel>().ReverseMap();
                cfg.CreateMap<CameraModel, CameraBindableModel>().ReverseMap();
                cfg.CreateMap<ScenarioModel, ScenarioBindableModel>().ReverseMap();
                cfg.CreateMap<ScenarioActionModel, ScenarioActionBindableModel>().ReverseMap();
                cfg.CreateMap<Models.Device, DeviceBindableModel>().ReverseMap();
                cfg.CreateMap<FanDevice, DeviceBindableModel>().ReverseMap();
                ConfigureDeviceMapping(cfg);
            });

            return mapperConfiguration.CreateMapper();
        }

        private void ConfigureDeviceMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<DeviceResponse, DeviceBindableModel>()
                .ForMember(nameof(DeviceBindableModel.CreateTime), opt => opt.MapFrom(src => DateTimeHelper.ConvertFromMilliseconds(src.CreateTime)))
                .ForMember(nameof(DeviceBindableModel.UpdateTime), opt => opt.MapFrom(src => DateTimeHelper.ConvertFromMilliseconds(src.UpdateTime)))
                .ForMember(nameof(DeviceBindableModel.DeviceId), opt => opt.MapFrom(src => src.Did))
                .ForMember(nameof(DeviceBindableModel.Name), opt => opt.MapFrom(src => src.DeviceName));

            cfg.CreateMap<DeviceBindableModel, DeviceResponse>()
                .ForMember(nameof(DeviceResponse.CreateTime), opt => opt.MapFrom(src => DateTimeHelper.ConvertToMilliseconds(src.CreateTime)))
                .ForMember(nameof(DeviceResponse.UpdateTime), opt => opt.MapFrom(src => DateTimeHelper.ConvertToMilliseconds(src.UpdateTime)))
                .ForMember(nameof(DeviceResponse.Did), opt => opt.MapFrom(src => src.DeviceId))
                .ForMember(nameof(DeviceResponse.DeviceName), opt => opt.MapFrom(src => src.Name));
        }

        #endregion
    }
}
