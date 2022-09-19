using System.Security.Cryptography;
using Com.Amazon.Identity.Auth.Device.Interactive;

namespace Com.Amazon.Identity.Auth.Device.Api.Authorization
{
    public partial class AuthorizeListener
    {
        void ICancellableListener.OnCancel(Java.Lang.Object? p0)
        {
            OnCancel(p0 as AuthCancellation);
        }

        void ICancellableListener.OnError(Java.Lang.Object? p0)
        {
            OnError(p0 as object as AuthError);
        }

        void IListener.OnError(Java.Lang.Object? p0)
        {
            OnError(p0 as object as AuthError);
        }

        void ICancellableListener.OnSuccess(Java.Lang.Object? p0)
        {
            OnSuccess(p0 as AuthorizeResult);
        }

        void IListener.OnSuccess(Java.Lang.Object? p0)
        {
            OnSuccess(p0 as AuthorizeResult);
        }
    }
}
