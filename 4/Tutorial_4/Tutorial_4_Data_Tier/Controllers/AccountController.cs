using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    public class AccountController : ApiController
    {
        //https://localhost:44396/
        BankDB.AccountAccessInterface accountAccess = Bank.bankDB.GetAccountInterface();

        //SELECT before editing, also create before selecting
        [HttpPost]
        [Route("api/Account/GetAccountIDsByUser/{userID}")]
        public List<uint> GetAccountIDsByUser(uint userID)
        {
            // gets all accounts owned by a user, both denoted by ID
            // return all ID's as a new interim object, or as a lsit of accounts?
            // is a list collection serialisable?
            try
            {
                return accountAccess.GetAccountIDsByUser(userID);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Account/GetAccountDetails/{accountID}")]
        public Account GetAccountDetails(uint accountID)
        {
            try
            {
                accountAccess.SelectAccount(accountID);
                return new Account
                {
                    AccountID = accountAccess.GetOwner(),
                    Balance = accountAccess.GetBalance(),
                    UserID = accountID
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Account/CreateAccount/{userID}")]
        public Account CreateAccount(uint userID)
        {
            try
            {
                uint accountID = accountAccess.CreateAccount(userID);
                accountAccess.SelectAccount(accountID);

                return new Account()
                {
                    AccountID = accountID,
                    UserID = userID,
                    Balance = 0
                };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });  
            }
        }

        [HttpPost]
        [Route("api/Account/Withdraw/{accountID}/{amount}")]
        public void Withdraw(uint accountID, uint value)
        {
            try
            {
                accountAccess.SelectAccount(accountID);
                accountAccess.Withdraw(value);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Account/Deposit/{accountID}/{value}")]
        public void Deposit(uint accountID, uint value)
        {
            try
            {
                accountAccess.SelectAccount(accountID);
                accountAccess.Deposit(value);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpGet]
        [Route("api/Account/GetBalance/{accountID}")]
        public uint GetBalance(uint accountID)
        {
            try
            {
                accountAccess.SelectAccount(accountID);
                return accountAccess.GetBalance();
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Account/GetOwner{accountID}")]
        public Account GetOwner(uint accountID)
        {
            //GetOwner of account interface gives user id for the account
            try
            {
                accountAccess.SelectAccount(accountID);
                return new Account { AccountID = accountID, UserID = accountAccess.GetOwner() };
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }
    }
}
