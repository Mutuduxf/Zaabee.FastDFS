using System.Collections.Generic;

namespace Zaabee.FastDfsClient.Config
{
    public class FastDfsConfig
    {
        public FastDfsConfig()
        {
            FastDfsServer = new List<FastDfsServer>();
        }

        public string GroupName { get; set; }

        public List<FastDfsServer> FastDfsServer { get; set; }
    }
}