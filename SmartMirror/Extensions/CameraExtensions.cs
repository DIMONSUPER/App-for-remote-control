using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using System.Windows.Input;

namespace SmartMirror.Extensions
{
    public static class CameraExtensions
    {
        public static CameraBindableModel ToCameraBindableModel(this CameraModel cameraModel, ICommand? tapCommand = null) =>
            new CameraBindableModel
            {
                Id = cameraModel.Id,
                Name = cameraModel.Name,
                IsConnected = cameraModel.IsConnected,
                CreateTime = cameraModel.CreateTime,
                VideoUrl = cameraModel.VideoUrl,
                TapCommand = tapCommand,
            };
    }
}
