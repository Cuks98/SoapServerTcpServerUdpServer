using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TcpServer;

namespace UdpServer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            UdpClient udpServer = new UdpClient(11000);

            while (!stoppingToken.IsCancellationRequested)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, 11000);
                var data = udpServer.Receive(ref remoteEP);

                var recivedMessage = Encoding.UTF8.GetString(data, 0, data.Length);

                var recivedData = JsonSerializer.Deserialize<UdpServerRequest>(recivedMessage);

                var reply = "test nazad";

                if (recivedData.SportHistory=="1")
                {
                    reply = "condition";
                }
                else if(recivedData.SportHistory== "2")
                {
                    reply = "weightLos";
                }
                else if(recivedData.SportHistory == "3")
                {
                    reply = "muscleBuilding";
                }
                else
                {
                    reply = "multipleSpecialitys";
                }

                
                byte[] replyBytes = Encoding.UTF8.GetBytes(reply);
                

                udpServer.Send(replyBytes, replyBytes.Length, remoteEP);
            }
        }
    }
}