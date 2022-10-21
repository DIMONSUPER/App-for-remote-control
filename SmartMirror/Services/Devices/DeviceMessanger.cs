using System;
using Newtonsoft.Json;
using SmartMirror.Models.Aqara;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SmartMirror.Models;
using static Android.Provider.CalendarContract;

namespace SmartMirror.Services.Devices
{
    public class DeviceMessanger
    {
        private const int UDP_PORT = 7834;
        private readonly JsonSerializer _serializer = new();
        private CancellationTokenSource _cancellationTokenSource;

        public DeviceMessanger()
        {
        }

        #region -- Public properties --

        public event EventHandler<AqaraMessageEventArgs> MessageReceived;

        public event EventHandler StoppedListenning;

        #endregion

        #region -- Public helpers --

        public void StartListening()
        {
            _cancellationTokenSource = new();

            var cancellationToken = _cancellationTokenSource.Token;

            var receiver = new UdpClient(UDP_PORT);

            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        var data = await receiver.ReceiveAsync(cancellationToken);

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            var message = Encoding.UTF8.GetString(data.Buffer);

                            DeserealizeAndInvokeHandler(message);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                finally
                {
                    StoppedListenning?.Invoke(this, EventArgs.Empty);
                    receiver?.Close();
                    receiver?.Dispose();
                    receiver = null;
                    _cancellationTokenSource = null;
                }
            }, cancellationToken);
        }

        public void StopListening()
        {
            _cancellationTokenSource?.Cancel();
        }

        #endregion

        #region -- Private helpers --

        private void DeserealizeAndInvokeHandler(string message)
        {
            if (TryDeserealize(message, out MessageEventResponse messageEventResponse))
            {
                foreach (var eventData in messageEventResponse.Data)
                {
                    MessageReceived?.Invoke(this, new()
                    {
                        EventType = messageEventResponse.MsgType,
                        DeviceId = eventData.SubjectId,
                        Time = eventData.Time,
                        ResourceId = eventData.ResourceId,
                        Value = eventData.Value,
                    });
                }
            }
            else if (TryDeserealize(message, out MessageChangeResponse messageChangeResponse))
            {
                if (messageChangeResponse.Data?.ChangeValues is not null && messageChangeResponse.Data.ChangeValues.Any())
                {
                    foreach (var changeValue in messageChangeResponse.Data?.ChangeValues)
                    {
                        MessageReceived?.Invoke(this, new()
                        {
                            EventType = messageChangeResponse.EventType,
                            DeviceId = messageChangeResponse.Data.Did,
                            Time = messageChangeResponse.Time,
                            ResourceId = changeValue.ResourceId,
                            Value = changeValue.Name,
                        });
                    }
                }
                else
                {
                    MessageReceived?.Invoke(this, new()
                    {
                        EventType = messageChangeResponse.EventType,
                        DeviceId = messageChangeResponse.Data.Did,
                        Time = messageChangeResponse.Data.Time,
                        Value = messageChangeResponse.Data.DeviceName,
                    });
                }
            }
            else if (TryDeserealize(message, out MessageDicsonnectReponse messageDisconnectResponse))
            {
                foreach (var subDid in messageDisconnectResponse.Data.SubDids)
                {
                    MessageReceived?.Invoke(this, new()
                    {
                        EventType = messageDisconnectResponse.EventType,
                        DeviceId = messageDisconnectResponse.Data.Did,
                        Time = messageDisconnectResponse.Data.Time,
                        ResourceId = subDid,
                        Value = messageDisconnectResponse.Data.Cause.ToString(),
                    });
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Failed to serealize {message}");
            }
        }

        private bool TryDeserealize<T>(string json, out T target)
        {
            var result = false;

            try
            {
                using var stringReader = new StringReader(json);
                using var reader = new JsonTextReader(stringReader);

                target = _serializer.Deserialize<T>(reader);

                result = true;
            }
            catch (Exception)
            {
                target = default;
            }

            return result;
        }

        #endregion
    }
}

