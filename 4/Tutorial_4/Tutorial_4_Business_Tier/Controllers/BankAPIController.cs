using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;
using RestSharp;
using Newtonsoft.Json;

using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Tutorial_4_Business_Tier.Controllers
{
    public class BankAPIController : ApiController
    {
        private readonly string url = "https://Localhost:44396/";// data tier web service url
        private RestClient client;
        private Regex numericalRegex = new Regex("/^[0-9]*$/");

        // If a user profile is created for a bank account, then the user should not then have to make an account separately
        [HttpPost]
        [Route("api/Bank/CreateUserAndAccount/{fname}/{lname}")] 
        public Account CreateUserAndAccount(string fname, string lname)
        {
           if ((!string.IsNullOrEmpty(fname)) && (!string.IsNullOrEmpty(lname)))
            {
                // first the user
                client = new RestClient(url);
                IRestRequest userRequest = new RestRequest("api/User/CreateUser/" + fname + "/" + lname);
                IRestResponse userCreationResp = client.Post(userRequest);

                if (!userCreationResp.IsSuccessful)
                {
                    Debug.WriteLine("No response from the rest request");
                    return null;
                }

                if (userCreationResp.StatusCode == HttpStatusCode.BadRequest)
                {
                    Debug.Write(userCreationResp.StatusCode);
                    return null;
                }
                
                
                Users user = JsonConvert.DeserializeObject<Users>(userCreationResp.Content);

                // then the account
                IRestRequest accountRequest = new RestRequest("api/Account/CreateAccount/" + user.UserID.ToString());
                var accountResp = client.Post(accountRequest);

                Account account = JsonConvert.DeserializeObject<Account>(accountResp.Content);

                //save
                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);

                return account;
            }
           else
            {
                return null;
            }
        }

        // Creates a transaction i.e. transfer of value from one account to another
        [HttpPost]
        [Route("api/Bank/CreateTransaction/{receiverID}/{senderID}/{amount}")]
        public int CreateTransaction(uint receiverID, uint senderID, uint amount)
        {
            client = new RestClient(url);
            RestRequest request = new RestRequest("api/Transaction/CreateTransaction/" + receiverID + "/" + senderID + "/" + amount.ToString());
            IRestResponse resp = client.Post(request);

            if (!resp.IsSuccessful || resp.StatusCode == HttpStatusCode.BadRequest)
            {
                return -1;
            } 
            else
            {
                TransactionDetails transaction = JsonConvert.DeserializeObject<TransactionDetails>(resp.Content);

                RestRequest processRequest = new RestRequest("api/ProcessAllTransactions");
                client.Post(processRequest);

                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);

                return 0;
            }
        }

        [HttpPost]
        [Route("api/Bank/Withdraw/{accountID}/{amount}")]
        public void Withdraw(uint accountID, uint amount)
        {
            client = new RestClient(url);
            RestRequest request = new RestRequest("api/Account/Withdraw/" + accountID + "/" + amount);
            var resp = client.Post(request);
            if (resp.StatusCode == HttpStatusCode.BadRequest)
            {
                Debug.WriteLine("Bad request for account withdrawal");
            }
            else
            {
                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);
            }
        }

        [HttpPost]
        [Route("api/Bank/Deposit/{accountID}/{amount}")]
        public void Deposit(uint accountID, uint amount)
        {
            client = new RestClient(url);
            RestRequest request = new RestRequest("api/Account/Deposit/" + accountID + "/" + amount);
            var resp = client.Post(request);
            if (resp.StatusCode == HttpStatusCode.BadRequest)
            {
                Debug.WriteLine("Bad request for account deposit");
            }
            else
            {
                RestRequest saveRequest = new RestRequest("api/Save");
                client.Post(saveRequest);
            }
        }

        // Retrieve to details of an account denoted by ID as a given user can have multiple accounts 
        [HttpGet]
        [Route("api/Bank/GetAccount/{accountID}")]
        public Account GetAccount(uint accountID)
        {
            client = new RestClient(url);
            RestRequest request = new RestRequest("api/Account/GetAccount/" + accountID);
            IRestResponse resp = client.Get(request);

            if (resp.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<Account>(resp.Content);
            }
        }

        [HttpGet]
        [Route("api/Bank/GetBalance/{accountID}")]
        public int GetBalance(uint accountID)
        {
            client = new RestClient(url);
            RestRequest request = new RestRequest("api/Account/GetBalance/" + accountID);
            IRestResponse resp = client.Get(request);
            if (resp.StatusCode == HttpStatusCode.BadRequest)
            {
                return -1;
            }
            else
            {
                if (int.TryParse(resp.Content.ToString(), out int bal))
                {
                    return bal;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
