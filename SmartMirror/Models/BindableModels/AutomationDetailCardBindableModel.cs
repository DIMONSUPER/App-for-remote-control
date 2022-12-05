using System;

namespace SmartMirror.Models.BindableModels;

public class AutomationDetailCardBindableModel : BindableBase
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _iconSource = "pic_gears";
    public string IconSource
    {
        get => _iconSource;
        set => SetProperty(ref _iconSource, value);
    }

    // TODO temporally
    private string _roomName = "Main room";
    public string RoomName
    {
        get => _roomName;
        set => SetProperty(ref _roomName, value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    // TODO temporally
    private string _status = "Turn Off";
    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
}