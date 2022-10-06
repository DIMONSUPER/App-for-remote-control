using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace SmartMirror.Services.Mapper
{
    public interface IMapperService
    {
        Task<IMapper> GetMapperAsync();
        Task<T> MapAsync<T>(object source);
        Task<OutT> MapAsync<InT, OutT>(InT source);
        Task<T> MapAsync<T>(object source, Action<object, T> afterMap);
        Task<OutT> MapAsync<InT, OutT>(InT source, Action<InT, OutT> afterMap);
        Task<IEnumerable<T>> MapRangeAsync<T>(IEnumerable<object> items);
        Task<IEnumerable<OutT>> MapRangeAsync<InT, OutT>(IEnumerable<InT> items);
        Task<IEnumerable<T>> MapRangeAsync<T>(IEnumerable<object> items, Action<object, T> afterMap);
        Task<IEnumerable<OutT>> MapRangeAsync<InT, OutT>(IEnumerable<InT> items, Action<InT, OutT> afterMap);
    }
}
