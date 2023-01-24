using DataBaseAPI.Models;
using FileAPI;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TcpServer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private TcpListener _tcpListener;

        public Worker(
                ILogger<Worker> logger,
                IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
            _tcpListener.Start();

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker is stopping");
            _tcpListener.Stop();

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("ExecuteAsync method called");


            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork(stoppingToken);
            }

            _logger.LogDebug("ExecuteAsync method finished");
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Waiting for a connection...");
            TcpClient client = await _tcpListener.AcceptTcpClientAsync();
            _logger.LogInformation("a new client connected");
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = 4000;

            try
            {
                byte[] byteMessage = new byte[107520];
                byte[] readArray = new byte[256];
                int byteCounter = 0;
                int i;
                _logger.LogDebug("Reading stream started");

                _logger.LogDebug("Reading stream");


                i = stream.Read(readArray, 0, readArray.Length);
                System.Buffer.BlockCopy(readArray, 0, byteMessage, byteCounter, i);
                byteCounter = byteCounter + i;


                var recivedMessage = Encoding.UTF8.GetString(byteMessage, 0, byteCounter);

                var recivedData = JsonSerializer.Deserialize<TcpServerRequest>(recivedMessage);

                _logger.LogInformation("Got edifact message: {0}", recivedMessage);

                var reply = "";

                if(int.Parse(recivedData.Weight) >= int.Parse(recivedData.Height)-100 && recivedData.Gender == "M")
                {
                    reply = "Mrsavljenje";
                }
                else if(int.Parse(recivedData.Weight) >= int.Parse(recivedData.Height) - 110 && recivedData.Gender == "Z")
                {
                    reply = "Mrsavljenje";
                }
                else if (int.Parse(recivedData.Weight) <= int.Parse(recivedData.Height) - 100 && recivedData.Gender == "M")
                {
                    reply = "Body building";
                }
                else if (int.Parse(recivedData.Weight) <= int.Parse(recivedData.Height) - 110 && recivedData.Gender == "Z")
                {
                    reply = "Body building";
                }
                else
                {
                    reply = "Durability";
                }

                try
                {
                    stream.Write(Encoding.UTF8.GetBytes(reply), 0, reply.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
            }
            finally
            {
                stream.Close();
                _logger.LogInformation("Closing tcp client");
                client.Close();
                client.Dispose();
            }
        }
    }
}
