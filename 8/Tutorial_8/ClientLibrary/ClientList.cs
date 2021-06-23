using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public static class ClientList
    {
        public static List<Client> clientList = new List<Client>();

        public static List<Client> GetClients()
        {
            return clientList;
        }
    }
}
