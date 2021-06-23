using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    public class UserController : ApiController
    {
        // https://localhost:44396/
        BankDB.UserAccessInterface userAccess = Bank.bankDB.GetUserAccess();

        [HttpPost]
        [Route("api/User/CreateUser/{fname}/{lname}")]
        public Users CreateUser(string fname, string lname)
        {
            try
            {
                Users newUser = new Users();
                newUser.UserID = userAccess.CreateUser();

               newUser.Fname = fname;
               newUser.Lname = lname;
                userAccess.SetUserName(fname, lname);
               // userAccess.GetUserName(out newUser.Fname, out newUser.Lname);
                return newUser;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/User/{userID}")]
        public Users GetUser(uint userID)//make a list-y version
        {
            try
            {
                userAccess.SelectUser(userID);
                userAccess.GetUserName(out string fname, out string lname);

                Users userDetails = new Users();
                userDetails.Fname = fname;
                userDetails.Lname = lname;
                userDetails.UserID = userID;
                return userDetails;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/User/GetUsername")]
        public string GetUsername(uint accountID, out string fname, out string lname)
        {
            try
            {
                userAccess.SelectUser(accountID);
                userAccess.GetUserName(out fname, out lname);
                return fname + " " + lname;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/User/SetUsername")]
        public int SetUsername(uint accountID, string fname, string lname)
        {
            try
            {
                userAccess.SelectUser(accountID);
                userAccess.SetUserName(fname, lname);
                return 1;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }
    }
}
