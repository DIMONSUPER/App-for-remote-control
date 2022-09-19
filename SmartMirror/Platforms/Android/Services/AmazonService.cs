using System;
using System.Diagnostics;
using Com.Amazon.Identity.Auth.Device;
using Com.Amazon.Identity.Auth.Device.Api.Authorization;
using Com.Amazon.Identity.Auth.Device.Api.Workflow;
using SmartMirror.Services.Amazon;

namespace SmartMirror.Platforms.Services
{
    public class AmazonService : IAmazonService
    {
        public AmazonService()
        {
        }

        #region -- IAmazonService implementation --

        public event EventHandler<AuthorizeResult> AuthorizationFinished;

        public Task<AOResult> StartAuthorizationAsync()
        {
            var result = new AOResult();

            try
            {
                if (Platform.CurrentActivity is MainActivity mainActivity)
                {
                    mainActivity.StartAmazonAuthorization(AuthorizationFinished);
                }
                else
                {
                    result.SetFailure("Current activity is not main activity");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(StartAuthorizationAsync)}: {ex.Message}");
                result.SetError(nameof(StartAuthorizationAsync), ex.Message, ex);
            }

            return Task.FromResult(result);
        }

        #endregion
    }

    internal class CustomAuthorizeListener : AuthorizeListener
    {
        private event EventHandler<AuthorizeResult> _currentEventHandler;

        public CustomAuthorizeListener(EventHandler<AuthorizeResult> eventHandler)
        {
            _currentEventHandler = eventHandler;
        }

        #region -- AuthorizeListener implementation --

        public override void OnCancel(AuthCancellation authCancellation)
        {
            _currentEventHandler?.Invoke(this, null);
        }

        public override void OnError(AuthError authError)
        {
            _currentEventHandler?.Invoke(this, null);
        }

        public override void OnSuccess(AuthorizeResult authorizeResult)
        {
            _currentEventHandler?.Invoke(this, authorizeResult);
        }

        #endregion
    }
}

