using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server
{
    internal class FileLoader
    {
        internal static IpData ReadFromFile()
        {
            string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IpSetting.json");

            if (File.Exists(root) == false)
            {
                IpData ori = new IpData();
                string con = JsonConvert.SerializeObject(new IpData());

                File.Create(root).Dispose();
                File.WriteAllText(root, con);
            }

            IpData data = JsonConvert.DeserializeObject<IpData>(File.ReadAllText(root));
            return data;
        }
    }
}
