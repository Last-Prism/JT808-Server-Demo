using System.Net;
using System.Net.Sockets;

namespace JT808Server
{
    internal class Program
    {
        internal static WebSocketServerManager wsManager;

        static void Main(string[] args)
        {
            IpData ipData = CustomIpConfig.ReadFromFile();

            Task.Run(() =>
            {
                wsManager = new WebSocketServerManager();
                wsManager.Start(ipData.Ws_Port);
            });

            Task.Run(() =>
            {
                int port = ipData.GPS_Port;
                string ip = ipData.IP;
                var listener = new TcpListener(IPAddress.Parse(ip), port);
                listener.Start();
                Console.WriteLine($"JT808 Server started on {ip}:{port}. Waiting for connections...");

                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    Console.WriteLine($"New {DateTime.Now}: {client.Client.RemoteEndPoint}");
                    Task.Run(() => HandleClient(client));
                }
            });

            Thread.Sleep(Timeout.Infinite); // 保证主线程不退出
        }

        static void HandleClient(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[4096];
            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;

                    // Print received JT808 message as hex
                    string hex = BitConverter.ToString(buffer, 0, bytesRead).Replace("-", " ");
                    Console.WriteLine($"[Received {DateTime.Now}] {hex}");
                    RequestHandler.HandleMsg(stream, buffer.Take(bytesRead).ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"Connection closed: {client.Client.RemoteEndPoint}");
                client.Close();
            }
        }
    }
}
