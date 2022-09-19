using System;
namespace Com.Amazon.Identity.Auth.Device.Thread
{
    public partial class DefaultAuthorizationListener
    {
        public void OnCancel(Java.Lang.Object? p0)
        {
            OnCancel(p0 as Bundle);
        }

        public void OnError(Java.Lang.Object? p0)
        {
            OnError(p0 as object as AuthError);
        }

        public void OnSuccess(Java.Lang.Object? p0)
        {
            OnSuccess(p0 as Bundle);
        }
    }
}

