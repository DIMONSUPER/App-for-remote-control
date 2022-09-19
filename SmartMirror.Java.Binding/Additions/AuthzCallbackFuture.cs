using System;
using Com.Amazon.Identity.Auth.Device.Api;

namespace Com.Amazon.Identity.Auth.Device.Thread
{
    public partial class AuthzCallbackFuture
    {
        void ICancellableListener.OnCancel(Java.Lang.Object? p0)
        {
            OnCancel(p0 as Bundle);
        }

        void ICancellableListener.OnSuccess(Java.Lang.Object? p0)
        {
            OnSuccess(p0 as Bundle);
        }

        void ICancellableListener.OnError(Java.Lang.Object? p0)
        {
            OnError(p0 as object as AuthError);
        }
    }
}