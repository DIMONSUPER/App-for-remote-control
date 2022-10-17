using SmartMirror.Helpers;
using SmartMirror.Models;

namespace SmartMirror.Services.Rooms
{
    public interface IRoomsService
    {
        Task<AOResult<IEnumerable<RoomModel>>> GetAllRoomsAsync();
    }
}
