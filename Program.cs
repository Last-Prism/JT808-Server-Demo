using JT808.Protocol.Enums;
using JT808Server.Msg;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

namespace JT808Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IpData ipData = FileLoader.ReadFromFile();

            int port = ipData.Port; // You can change this port if needed
            string ip = ipData.IP;
            var listener = new TcpListener(IPAddress.Parse(ip), port);
            listener.Start();
            Console.WriteLine($"JT808 Server started on port {port}. Waiting for connections...");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine($"New {DateTime.Now}: {client.Client.RemoteEndPoint}");
                Task.Run(() => HandleClient(client));
            }
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
