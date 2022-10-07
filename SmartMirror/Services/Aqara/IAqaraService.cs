using System;
using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Aqara;

public interface IAqaraService
{
    Task<AOResult<BaseAqaraResponse>> SendLoginCodeAsync(string email);
    Task<AOResult<BaseAqaraResponse>> LoginWithCodeAsync(string email, string code);
    bool IsAuthorized { get; }
}

