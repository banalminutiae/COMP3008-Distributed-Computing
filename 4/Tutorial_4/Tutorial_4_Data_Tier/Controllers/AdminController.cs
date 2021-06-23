using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    public class AdminController : ApiController
    {
        [HttpPost]
        [Route("api/SaveToDisk")]
        public void SaveToDisk()
        {
            Bank.bankDB.SaveToDisk();
        }

        [HttpPost]
        [Route("api/ProcessAllTransactions")]
        public bool ProcessAllTransactions()
        {
            try
            {
                Bank.bankDB.ProcessAllTransactions();
                return true;
            }
            catch(Exception)
            {
                return false;
            }

        }
    }
}