using System;
using System.Diagnostics;

namespace SmartMirror.Services.Permissions
{
    public class PermissionsService : IPermissionsService
    {
        #region -- IPermissionsService implementation --

        public async Task<PermissionStatus> CheckPermissionAsync<T>()
            where T : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new()
        {
            var permissionStatus = PermissionStatus.Unknown;

            try
            {
                permissionStatus = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<T>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return permissionStatus;
        }

        public async Task<PermissionStatus> RequestPermissionAsync<T>()
            where T : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new()
        {
            var status = PermissionStatus.Unknown;

            try
            {
                status = await CheckPermissionAsync<T>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<T>();
                }

                return status;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return status;
        }

        public void OpenApplicationSettingsPage()
        {
            try
            {
                AppInfo.Current.ShowSettingsUI();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public bool ShouldShowRationale<T>()
            where T : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new()
        {
            var shouldShowRationale = false;

            try
            {
                shouldShowRationale = Microsoft.Maui.ApplicationModel.Permissions.ShouldShowRationale<T>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return shouldShowRationale;
        }

        #endregion
    }
}

