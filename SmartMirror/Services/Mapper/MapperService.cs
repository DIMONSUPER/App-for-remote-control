using AutoMapper;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;
using SmartMirror.Models;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.DTO;

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
                ConfigureDeviceMapping(cfg);

                cfg.CreateMap<RoomModel, RoomBindableModel>().ReverseMap();
                cfg.CreateMap<CameraBindableModel, CameraDTO>().ReverseMap();
                cfg.CreateMap<ScenarioActionModel, ScenarioActionBindableModel>().ReverseMap();
                cfg.CreateMap<DeviceModel, DeviceBindableModel>().ReverseMap();
                cfg.CreateMap<FanDevice, DeviceBindableModel>().ReverseMap();
                cfg.CreateMap<DeviceDTO, DeviceBindableModel>().ReverseMap();
                cfg.CreateMap<DeviceBindableModel, DeviceDTO>().ReverseMap();
                cfg.CreateMap<DeviceBindableModel, DeviceBindableModel>().ReverseMap();
                cfg.CreateMap<AttributeAqaraResponse, AttributeAqaraDTO>().ReverseMap();
                cfg.CreateMap<DetailSceneAqaraModel, ScenarioBindableModel>().ReverseMap();
                cfg.CreateMap<ActionAqaraModel, ScenarioActionBindableModel>().ReverseMap();
                cfg.CreateMap<ScenarioBindableModel, ImageAndTitleBindableModel>().ReverseMap();
                cfg.CreateMap<CameraBindableModel, ImageAndTitleBindableModel>().ReverseMap();
                cfg.CreateMap<AutomationBindableModel, ImageAndTitleBindableModel>().ReverseMap();
                cfg.CreateMap<INotifiable, ImageAndTitleBindableModel>().ReverseMap();
                cfg.CreateMap<ScenarioBindableModel, ScenarioDTO>().ReverseMap();
                cfg.CreateMap<RoomBindableModel, NotificationSourceBindableModel>().ReverseMap();
                cfg.CreateMap<LinkageAqaraModel, AutomationBindableModel>().ReverseMap();
                cfg.CreateMap<AutomationBindableModel, AutomationDTO>().ReverseMap();
                cfg.CreateMap<AutomationBindableModel, ImageAndTitleBindableModel>().ReverseMap();
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

            cfg.CreateMap<DeviceBindableModel, ImageAndTitleBindableModel>()
                .ForMember(nameof(ImageAndTitleBindableModel.ImageSource), opt => opt.MapFrom(scr => scr.IconSource))
                .ReverseMap();

            cfg.CreateMap<ScenarioModel, ScenarioBindableModel>()
                .ForMember(nameof(ScenarioBindableModel.Id), opt => opt.MapFrom(src => 0))
                .ForMember(nameof(ScenarioBindableModel.SceneId), opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            cfg.CreateMap<DeviceBindableModel, NotificationSourceBindableModel>()
                .ForMember(nameof(NotificationSourceBindableModel.Id), opt => opt.MapFrom(src => src.FullDeviceId))
                .ReverseMap();
        }

        #endregion
    }
}
