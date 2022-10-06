using AutoMapper;
using SmartMirror.Models;
using static Android.Icu.Text.CaseMap;

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
            });

            return mapperConfiguration.CreateMapper();
        }

        #endregion
    }
}
