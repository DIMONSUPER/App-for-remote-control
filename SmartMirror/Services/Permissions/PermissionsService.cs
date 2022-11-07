using System;

namespace SmartMirror.Services.Permissions
{
    public class PermissionsService : IPermissionsService
    {
        public Task<PermissionStatus> CheckPermissionAsync<T>()
            where T : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new()
        {
            return Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<T>();
        }

        public async Task<PermissionStatus> RequestPermissionAsync<T>()
            where T : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new()
        {
            var status = await CheckPermissionAsync<T>();

            if (status != PermissionStatus.Granted)
            {
                status = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<T>();
            }

            return status;
        }

        public void OpenApplicationSettingsPage()
        {
            AppInfo.Current.ShowSettingsUI();
        }

        public bool ShouldShowRationale<T>()
            where T : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new()
        {
            return Microsoft.Maui.ApplicationModel.Permissions.ShouldShowRationale<T>();
        }
    }
}

