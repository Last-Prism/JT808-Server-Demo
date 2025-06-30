namespace JT808Server
{
    internal class IpData
    {

        public string IP { get; set; }
        public int GPS_Port { get; set; }
        public int Ws_Port { get; set; }

        public IpData()
        {
            IP = "127.0.0.1";
            GPS_Port = 6565;
            Ws_Port = 8080;
        }
    }
}
