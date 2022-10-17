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

        public IMapper GetMapper() => _lazyMapper.Value;

        public OutT Map<InT, OutT>(InT source)
        {
            return _lazyMapper.Value.Map<InT, OutT>(source);
        }

        public OutT Map<InT, OutT>(InT source, Action<InT, OutT> afterMap)
        {
            return _lazyMapper.Value.Map<InT, OutT>(source, opt => opt.AfterMap(afterMap));
        }

        public IEnumerable<OutT> MapRange<InT, OutT>(IEnumerable<InT> items)
        {
            return items.Select(x => _lazyMapper.Value.Map<InT, OutT>(x));
        }

        public IEnumerable<OutT> MapRange<InT, OutT>(IEnumerable<InT> items, Action<InT, OutT> afterMap)
        {
            return items.Select(x => _lazyMapper.Value.Map<InT, OutT>(x, opt => opt.AfterMap(afterMap)));
        }

        public T Map<T>(object source)
        {
            return _lazyMapper.Value.Map<T>(source);
        }

        public T Map<T>(object source, Action<object, T> afterMap)
        {
            return _lazyMapper.Value.Map<T>(source, opt => opt.AfterMap(afterMap));
        }

        public IEnumerable<T> MapRange<T>(IEnumerable<object> items)
        {
            return items.Select(x => _lazyMapper.Value.Map<T>(x));
        }

        public IEnumerable<T> MapRange<T>(IEnumerable<object> items, Action<object, T> afterMap)
        {
            return items.Select(x => _lazyMapper.Value.Map<T>(x, opt => opt.AfterMap(afterMap)));
        }

        #endregion

        #region -- Private Helpers --

        private IMapper ConfigMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RoomModel, RoomBindableModel>().ReverseMap();
                cfg.CreateMap<CameraModel, CameraBindableModel>().ReverseMap();
                cfg.CreateMap<ScenarioModel, ScenarioBindableModel>().ReverseMap();
                cfg.CreateMap<ScenarioActionModel, ScenarioActionBindableModel>().ReverseMap();
                cfg.CreateMap<DeviceModel, DeviceBindableModel>().ReverseMap();
                cfg.CreateMap<FanDevice, DeviceBindableModel>().ReverseMap();
                ConfigureDeviceMapping(cfg);
                cfg.CreateMap<NotificationModel, NotificationGroupItemBindableModel>().ReverseMap();
            });

            return mapperConfiguration.CreateMapper();
        }

        private void ConfigureDeviceMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<DeviceAqaraModel, DeviceBindableModel>()
                .ForMember(nameof(DeviceBindableModel.CreateTime), opt => opt.MapFrom(src => DateTimeHelper.ConvertFromMilliseconds(src.CreateTime)))
                .ForMember(nameof(DeviceBindableModel.UpdateTime), opt => opt.MapFrom(src => DateTimeHelper.ConvertFromMilliseconds(src.UpdateTime)))
                .ForMember(nameof(DeviceBindableModel.DeviceId), opt => opt.MapFrom(src => src.Did))
                .ForMember(nameof(DeviceBindableModel.Name), opt => opt.MapFrom(src => src.DeviceName));

            cfg.CreateMap<DeviceBindableModel, DeviceAqaraModel>()
                .ForMember(nameof(DeviceAqaraModel.CreateTime), opt => opt.MapFrom(src => DateTimeHelper.ConvertToMilliseconds(src.CreateTime)))
                .ForMember(nameof(DeviceAqaraModel.UpdateTime), opt => opt.MapFrom(src => DateTimeHelper.ConvertToMilliseconds(src.UpdateTime)))
                .ForMember(nameof(DeviceAqaraModel.Did), opt => opt.MapFrom(src => src.DeviceId))
                .ForMember(nameof(DeviceAqaraModel.DeviceName), opt => opt.MapFrom(src => src.Name));
        }

        #endregion
    }
}
