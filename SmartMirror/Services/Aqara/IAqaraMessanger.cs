using SmartMirror.Models;

namespace SmartMirror.Services.Aqara;

public interface IAqaraMessanger
{
    event EventHandler<AqaraMessageEventArgs> MessageReceived;

    event EventHandler StoppedListenning;

    void StartListening();

    void StopListening();
}

