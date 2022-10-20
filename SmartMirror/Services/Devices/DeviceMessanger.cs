using System;
using Newtonsoft.Json;
using SmartMirror.Models.Aqara;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

        public event EventHandler<MessageChangeResponse> MessageChangeReceived;

        public event EventHandler<MessageEventResponse> MessageEventReceived;

        public event EventHandler<MessageDicsonnectReponse> MessageDicsonnectReceived;

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

                            if (TryDeserealize(message, out MessageEventResponse messageEventResponse))
                            {
                                MessageEventReceived?.Invoke(this, messageEventResponse);
                            }
                            else if (TryDeserealize(message, out MessageChangeResponse messageChangeResponse))
                            {
                                MessageChangeReceived?.Invoke(this, messageChangeResponse);
                            }
                            else if (TryDeserealize(message, out MessageDicsonnectReponse messageDisconnectResponse))
                            {
                                MessageDicsonnectReceived?.Invoke(this, messageDisconnectResponse);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to serealize {message}");
                            }
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

