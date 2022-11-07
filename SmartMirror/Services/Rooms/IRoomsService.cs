using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Rooms;

public interface IRoomsService
{
    event EventHandler AllRoomsChanged;

    Task<AOResult> DownloadAllRoomsAsync();

    Task<IEnumerable<RoomBindableModel>> GetAllRoomsAsync();
}
