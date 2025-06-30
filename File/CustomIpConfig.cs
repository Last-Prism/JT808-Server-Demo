using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server
{
    internal class CustomIpConfig
    {
        internal static IpData Data { get; private set; }

        internal static IpData ReadFromFile()
        {
            string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IpSetting.json");

            if (File.Exists(root) == false)
            {
                string con = JsonConvert.SerializeObject(new IpData());

                File.Create(root).Dispose();
                File.WriteAllText(root, con);
            }

            Data = JsonConvert.DeserializeObject<IpData>(File.ReadAllText(root));
            return Data;
        }
    }
}
