using SmartMirror.Helpers;

namespace SmartMirror.Services.Google;

public interface IGoogleService
{
    public Task<AOResult> AutorizeAsync();
}

