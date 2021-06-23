using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class Client
    {
        public string IPAddress { get; set; }
        public uint ports { get; set; }

        public Client() { }

        public Client(string newIPAddress, uint newPorts)
        {
            IPAddress = newIPAddress;
            ports = newPorts;
        }

        public string ConstructClientURL()
        {
            return IPAddress + ":" + ports.ToString();
        }
    }
}
