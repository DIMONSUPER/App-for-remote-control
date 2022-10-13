using AutoMapper;
using SmartMirror.Models;
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
                cfg.CreateMap<NotificationModel, NotificationGroupItemBindableModel>().ReverseMap();
            });

            return mapperConfiguration.CreateMapper();
        }

        #endregion
    }
}
