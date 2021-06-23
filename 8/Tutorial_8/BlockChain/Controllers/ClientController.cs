using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ClientLibrary;

namespace BlockChainApi.Controllers
{
    public class Clientcontroller : ApiController
    {
        [HttpPost]
        [Route("api/Client/Register")]
        public void RegisterClient(Client client)
        {
            try
            {
                ClientList.clientList.Add(client);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpGet]
        [Route("api/Client/GetClientList")]
        public List<Client> GetClientList()
        {
            try
            {
                return ClientList.GetClients();
            }
            catch(Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }
    }
    
}