using Newtonsoft.Json;
using SmartMirror.Models.Aqara;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using SmartMirror.Models;

namespace SmartMirror.Services.Aqara;

public class AqaraMessanger : IAqaraMessanger
{
    private HubConnection _connection;

    public AqaraMessanger()
    {
    }

    #region -- IAqaraMessanger implementation --

    public event EventHandler<AqaraMessageEventArgs> MessageReceived;

    public event EventHandler StoppedListenning;

    public async Task StartListeningAsync()
    {
        try
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{Constants.OurCloudServer.API_URL}/chat")
                .WithAutomaticReconnect()
                .Build();

            _connection.On<AqaraMessageEventArgs>(Constants.OurCloudServer.RECEIVE_MESSAGE, (message) =>
            {
                MessageReceived?.Invoke(this, message);
            });

            await _connection.StartAsync();
        }
        catch (Exception ex)
        {
        }
    }

    public async Task StopListeningAsync()
    {
        await _connection?.StopAsync();

        StoppedListenning?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}

