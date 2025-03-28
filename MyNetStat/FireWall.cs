using System.Collections.Generic;

namespace MyNetStat
{
    public class FireWall
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int Action { get; set; }
        public int Direction { get; set; }
        public string LocalAddresses { get; set; }
        public int Protocol { get; set; }
        public Localport[] LocalPorts { get; set; }
        public string InterfaceTypes { get; set; }
        public List<Remoteaddress> RemoteAddresses { get; set; }
        public string RemotePorts { get; set; }


        public class Localport
        {
            public int PortNumber { get; set; }
            public string Description { get; set; }
        }

        public class Remoteaddress
        {
            public string IpAddress { get; set; }
            public string Description { get; set; }
        }
    }
}
