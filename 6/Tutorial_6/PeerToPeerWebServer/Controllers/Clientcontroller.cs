using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PeerToPeerWebServer.Models;
using ClientLibrary;

namespace PeerToPeerWebServer.Controllers
{
    public class Clientcontroller : ApiController
    {
         [HttpPost]
         [Route("api/Register")]
         public void RegisterClient(Client client)
        {
            ClientList.clientList.Add(client);
        }

        [HttpGet]
        [Route("api/Get")]
        public List<Client> GetClientList()
        {
            return ClientList.clientList;
        }
    }
}
