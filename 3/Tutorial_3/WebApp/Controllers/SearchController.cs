using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApp.Models;
using BizGUI;

namespace WebApp.Controllers
{
    public class SearchController : ApiController
    {
        [HttpPost]
        // returns index of matching database entry
        // TODO: transform data tier access to a singleton
        public SearchResponse Search(SearchData payload)
        {
            SearchResponse resp = new SearchResponse();
            int matchingIdx = -1;
            DataModel model = new DataModel();
            DataIntermediate data = new DataIntermediate();

            try
            {
                for (int i = 0; i < model.GetNumEntries(); i++)
                {
                    // only interested in the index and last name, everything else can be discarded
                    model.GetValuesForEntry(i, out _, out _, out _, out _, out string lname, out _);
                    if (lname.Equals(payload.searchString))
                    {
                        matchingIdx = i;
                        break;
                    }
                }
                resp.matchingIndex = matchingIdx;
                return resp;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
            
        }
    }
}