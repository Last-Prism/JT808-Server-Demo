using JT808.Protocol;
using JT808.Protocol.Enums;
using JT808.Protocol.MessageBody;
using JT808Server.Model;
using JT808Server.Msg;
using System.Net.Sockets;

namespace JT808Server
{
    internal class RequestHandler
    {
        static JT808Serializer jT808Serializer = new JT808Serializer();

        internal static void HandleMsg(NetworkStream stream, byte[] data)
        {
            if (data == null || data.Length < 1)
            {
                Console.WriteLine("[Error] 数据为空或长度不合法");
                return;
            }

            // 判断起始和结束标志
            if (data[0] != 0x7E || data[data.Length - 1] != 0x7E)
            {
                Console.WriteLine("[Error] 不是合法的JT808数据包（缺少7E头尾）");
                return;
            }

            // 解析消息头
            if (data.Length < 13)
            {
                Console.WriteLine("[Error] 数据包长度不足，无法解析头部");
                return;
            }

            var package = jT808Serializer.Deserialize(data);
            JT808MsgId id =  (JT808MsgId)Enum.Parse(typeof(JT808MsgId), package.Header.MsgId.ToString());

            //Console.WriteLine(id);

            try
            {
                // 终端注册
                if (id == JT808MsgId._0x0100)
                {
                    // 处理注册包
                    var msg = PackageFactory.PlatformRegisterReply(package);
                    SendMsg(stream, msg);
                }
                // 终端鉴权
                else if (id == JT808MsgId._0x0102)
                {
                    var msg = PackageFactory.ClientRootReply(package);
                    SendMsg(stream, msg);
                }
                // 终端主动发送定位数据
                else if (id == JT808MsgId._0x0200)
                {
                    JT808_0x0200 posData = (JT808_0x0200)package.Bodies;
                    
                    Program.wsManager.BroadcastJT808PosData(JT808PosData.Serialize(posData));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            #region Old
            //    ushort msgId = (ushort)((data[1] << 8) | data[2]);
            //    ushort bodyLen = (ushort)(((data[3] & 0x03) << 8) | data[4]);
            //    string phone = BcdToString(data, 5, 6);
            //    ushort flowId = (ushort)((data[11] << 8) | data[12]);

            //    // 消息体
            //    int bodyStart = 13;
            //    int bodyEnd = data.Length - 2; // 去掉校验和和0x7E
            //    int realBodyLen = bodyEnd - bodyStart;
            //    byte[] body = new byte[realBodyLen > 0 ? realBodyLen : 0];
            //    if (realBodyLen > 0)
            //    {
            //        Array.Copy(data, bodyStart, body, 0, realBodyLen);
            //    }

            //    // 根据消息ID分类处理
            //    switch (msgId)
            //    {
            //        case 0x0100: // 注册
            //            Console.WriteLine($"[注册] 手机号:{phone}, 流水号:{flowId}");
            //            // 可进一步解析body内容
            //            break;
            //        case 0x0102: // 鉴权
            //            string authCode = System.Text.Encoding.ASCII.GetString(body);
            //            Console.WriteLine($"[鉴权] 手机号:{phone}, 流水号:{flowId}, 鉴权码:{authCode}");
            //            break;
            //        case 0x0200: // 位置信息
            //            if (body.Length >= 28)
            //            {
            //                uint lat = BitConverter.ToUInt32(body, 8);
            //                uint lng = BitConverter.ToUInt32(body, 12);
            //                double latitude = lat / 1e6;
            //                double longitude = lng / 1e6;
            //                Console.WriteLine($"[位置] 手机号:{phone}, 纬度:{latitude}, 经度:{longitude}");
            //            }
            //            else
            //            {
            //                Console.WriteLine("[位置] 消息体长度不足");
            //            }
            //            break;
            //        default:
            //            Console.WriteLine($"[未知消息] 消息ID:0x{msgId:X4}, 手机号:{phone}, 流水号:{flowId}");
            //            break;
            //    }
            #endregion
        }

        //// BCD转字符串
        //static string BcdToString(byte[] data, int index, int len)
        //{
        //    var sb = new System.Text.StringBuilder();
        //    for (int i = index; i < index + len; i++)
        //    {
        //        sb.Append((data[i] >> 4) & 0x0F);
        //        sb.Append(data[i] & 0x0F);
        //    }
        //    return sb.ToString().TrimStart('0');
        //}

        static void SendMsg(NetworkStream stream, JT808Package data)
        {
            var msg = jT808Serializer.Serialize(data);
            stream.Write(msg);

            //Console.WriteLine("返回消息：" + BitConverter.ToString(msg, 0, msg.Length).Replace("-", " "));
        }
    }
}
