using System;
namespace Com.Amazon.Identity.Auth.Device.Workflow
{
    public abstract partial class BaseWorkflowListener
    {
        public void OnCancel(Java.Lang.Object? p0)
        {
            System.Diagnostics.Debug.WriteLine("BaseWorkflowListener: OnCancel");
        }

        public void OnError(Java.Lang.Object? p0)
        {
            System.Diagnostics.Debug.WriteLine("BaseWorkflowListener: OnError");
        }

        public void OnSuccess(Java.Lang.Object? p0)
        {
            System.Diagnostics.Debug.WriteLine("BaseWorkflowListener: OnSuccess");
        }
    }
}

