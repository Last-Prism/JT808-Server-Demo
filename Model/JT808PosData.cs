using JT808.Protocol.Enums;
using JT808.Protocol.MessageBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server.Model
{
    public class JT808PosData : ICustomHeader
    {
        public ushort MsgId { get; set; } = (ushort)JT808MsgId._0x0200;
        public string GpsTime { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public float Speed { get; set; }
        public ushort Direction { get; set; }
        public uint AlarmFlag { get; set; }
        public uint StatusFlag { get; set; }

        // ...补全字段

        public static JT808PosData Serialize(JT808_0x0200 data)
        {
            JT808PosData res = new JT808PosData()
            {
                GpsTime = data.GPSTime.ToString(),
                Lat = data.Lat,
                Lng = data.Lng,
                Speed = data.Speed,
                Direction = data.Direction,
                AlarmFlag = data.AlarmFlag,
                StatusFlag = data.StatusFlag,
            };

            return res;
        }
    }

    public interface ICustomHeader
    {
        public ushort MsgId { get; set; }
    }
}
