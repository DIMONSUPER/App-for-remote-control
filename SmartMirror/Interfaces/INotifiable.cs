namespace SmartMirror.Interfaces
{
    public interface INotifiable
    {
        int Id { get; set; }

        string Name { get; set; }

        bool IsReceiveNotifications { get; set; }
    }
}
