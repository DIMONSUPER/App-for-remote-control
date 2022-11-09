using System;

namespace SmartMirror.Services.Permissions
{
    public interface IPermissionsService
    {
        Task<PermissionStatus> CheckPermissionAsync<T>()
            where T : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new();

        Task<PermissionStatus> RequestPermissionAsync<T>()
            where T : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new();

        bool ShouldShowRationale<T>()
            where T : Microsoft.Maui.ApplicationModel.Permissions.BasePermission, new();

        void OpenApplicationSettingsPage();
    }
}

