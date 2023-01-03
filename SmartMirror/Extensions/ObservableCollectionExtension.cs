using System;
using System.Collections.ObjectModel;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Extensions;

public static class ObservableCollectionExtension
{
    public static void Update(this ObservableCollection<DeviceBindableModel> collection, DeviceBindableModel device)
    {
        var deviceId = device.Id;

        var updatedDivice = collection.FirstOrDefault(row => row.Id == deviceId);

        if (updatedDivice is not null)
        {
            if (!device.IsShownInRooms)
            {
                collection.Remove(updatedDivice);
            }
        }
        else
        {
            var nextItem = collection.FirstOrDefault(row => row.Id > deviceId);

            if (nextItem is null)
            {
                collection.Add(device);
            }
            else
            {
                var indexNextItem = collection.IndexOf(nextItem);

                collection.Insert(indexNextItem, device);
            }
        }
    }
}