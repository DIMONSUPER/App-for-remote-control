using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Rooms;

public interface IRoomsService
{
    List<RoomBindableModel> AllRooms { get; }

    event EventHandler AllRoomsChanged;

    Task<AOResult> DownloadAllRoomsAsync();
}
