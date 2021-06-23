using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace DBServer
{
    class Program
    {
        // present end point for the client to communicate with
        static void Main(string[] args)
        {
            var tcp = new NetTcpBinding();
            var host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DBInterface.DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            host.Open();
            Console.WriteLine("System is online");
            Console.ReadLine();
            host.Close();
        }
    }
}
