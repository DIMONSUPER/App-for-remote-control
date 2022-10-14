using SmartMirror.Helpers;
using SmartMirror.Models;

namespace SmartMirror.Services.RoomsService
{
    public interface IRoomsService
    {
        Task<AOResult<IEnumerable<RoomModel>>> GetAllRoomsAsync();
    }
}
