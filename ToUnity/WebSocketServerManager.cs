using JT808Server.Model;
using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

public class GpsWebSocketBehavior : WebSocketBehavior
{
    // 线程安全的客户端列表
    private static readonly List<GpsWebSocketBehavior> _clients = new List<GpsWebSocketBehavior>();

    protected override void OnOpen()
    {
        lock (_clients)
        {
            _clients.Add(this);

            var clientIp = this.Context.UserEndPoint?.ToString();
            Console.WriteLine(clientIp);
        }
    }

    protected override void OnClose(CloseEventArgs e)
    {
        lock (_clients)
        {
            _clients.Remove(this);
        }
    }

    public static void BroadcastJT808PosData(JT808PosData posData)
    {
        var json = JsonSerializer.Serialize(posData);
        lock (_clients)
        {
            foreach (var client in _clients.ToArray())
            {
                if (client.State == WebSocketState.Open)
                {
                    try 
                    {
                        client.Send(json); 
                    } 
                    catch(Exception ex)
                    {
                        /* 忽略异常 */
                        Console.WriteLine(ex);
                    }
                }
            }
        }
    }
}

public class WebSocketServerManager
{
    private WebSocketServer _wssv;

    public void Start(int port = 8080)
    {
        _wssv = new WebSocketServer(port);
        _wssv.AddWebSocketService<GpsWebSocketBehavior>("/ws");
        _wssv.Start();
        Console.WriteLine($"WebSocket server started at ws://0.0.0.0:{port}/ws");
    }

    public void Stop() => _wssv?.Stop();

    public void BroadcastJT808PosData(JT808PosData posData)
        => GpsWebSocketBehavior.BroadcastJT808PosData(posData);
}