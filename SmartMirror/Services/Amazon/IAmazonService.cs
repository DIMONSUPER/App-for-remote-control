using System;
using Com.Amazon.Identity.Auth.Device.Api.Authorization;

namespace SmartMirror.Services.Amazon;

public interface IAmazonService
{
    event EventHandler<AuthorizeResult> AuthorizationFinished;

    Task<AOResult> StartAuthorizationAsync();
}

