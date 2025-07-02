using JT808.Protocol;
using JT808.Protocol.Enums;
using JT808.Protocol.MessageBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server.Msg
{
    internal class PackageFactory
    {
        /// <summary>
        /// 注册用
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        internal static JT808Package PlatformRegisterReply(JT808Package package)
        {
            var package0100 = (JT808_0x0100)package.Bodies;

            var replyPackage = new JT808Package
            {
                Header = new JT808Header
                {
                    MsgId = (ushort)JT808MsgId._0x8100,
                    ManualMsgNum = 0,
                    TerminalPhoneNo = package.Header.TerminalPhoneNo
                },
                Bodies = new JT808_0x8100
                {
                    AckMsgNum = package.Header.MsgNum,
                    JT808TerminalRegisterResult = JT808TerminalRegisterResult.success,
                    Code = package0100.TerminalId + "," + package0100.PlateNo
                }
            };

            return replyPackage;
        }

        internal static JT808Package PlatformCommonReply(JT808Package package, string code)
        {
            var replyPackage = new JT808Package
            {
                Header = new JT808Header
                {
                    MsgId = (ushort)JT808MsgId._0x8001,
                    ManualMsgNum = 0,
                    TerminalPhoneNo = package.Header.TerminalPhoneNo
                },
                Bodies = new JT808_0x8001
                {
                    MsgNum = package.Header.MsgNum,
                    AckMsgId = package.Header.MsgId,
                    JT808PlatformResult = JT808PlatformResult.succeed
                }
            };

            return replyPackage;
        }

        internal static JT808Package ClientRootReply(JT808Package package)
        {
            var package0102 = (JT808_0x0102)package.Bodies;
            var code = package0102.Code;

            return PlatformCommonReply(package, code);
        }
    }
}
