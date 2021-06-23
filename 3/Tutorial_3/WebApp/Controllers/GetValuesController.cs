using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BizGUI;
using WebApp.Models;
using System.Drawing;

namespace WebApp.Controllers
{
    public class GetValuesController : ApiController
    {
        [HttpPost]
        [Route("api/GetValuesForEntry/{idx}")]
        public DataIntermediate GetValuesForEntry(int idx)
        {
            DataModel model = new DataModel();
            DataIntermediate data = new DataIntermediate();
            try
            {
                model.GetValuesForEntry(idx, out uint acct, out uint pin, out int bal, out string fname, out string lname, out Bitmap icon);
                data.acct = acct;
                data.bal = bal;
                data.pin = pin;
                data.fname = fname;
                data.lname = lname;
                //data.icon = icon;

                return data;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
            
        }
    }
}