using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using BusinessTierInterface;

namespace BusinessServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // presents endpoint for the client to connect to
            var tcp = new NetTcpBinding();
            var host = new ServiceHost(typeof(BusinessServer));
            host.AddServiceEndpoint(typeof(BusinessServerInterface), tcp, "net.tcp://0.0.0.0:8000/BusinessService");
            host.Open();
            Console.WriteLine("System is online");
            Console.ReadLine();
            host.Close();
        }
    }
}
