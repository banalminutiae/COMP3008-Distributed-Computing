using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using DBInterface;

namespace DBServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //presents endpoint for the business tier to connect to 
            var tcp = new NetTcpBinding();
            var host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            host.Open();
            Console.WriteLine("System is online");
            Console.ReadLine();
            host.Close();
        }
    }
}
