using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class MiscApiController : ApiController
    {
        private DataModel model = new DataModel();

        [HttpGet]
        [Route("api/GetNumEntries")]
        public int GetNumEntries()
        {
            return model.GetNumEntries();
        }
    }
}