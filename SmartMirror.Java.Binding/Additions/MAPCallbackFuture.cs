using System;
using Com.Amazon.Identity.Auth.Device.Api;
using Com.Amazon.Identity.Auth.Device.Shared;
using Java.Util.Concurrent;

namespace Com.Amazon.Identity.Auth.Device.Thread
{
    public partial class MAPCallbackFuture
    {
        void IListener.OnError(Java.Lang.Object? p0)
        {
            OnError(p0 as object as AuthError);
        }

        void IListener.OnSuccess(Java.Lang.Object? p0)
        {
            OnSuccess(p0 as Bundle);
        }

        Java.Lang.Object? IFuture.Get()
        {
            return Get();
        }

        Java.Lang.Object? IFuture.Get(long timeout, TimeUnit? unit)
        {
            return Get(timeout, unit);
        }
    }
}

