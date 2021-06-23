using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    public class TransactionController : ApiController
    {
        private BankDB.TransactionAccessInterface transactionAccess = Bank.bankDB.GetTransactionInterface();
        private BankDB.AccountAccessInterface accountAccess = Bank.bankDB.GetAccountInterface();

        [HttpPost]
        [Route("api/Transaction/CreateTransaction/{receiverID}/{senderID}/{amount}")]
        public TransactionDetails CreateTransaction(uint receiverID, uint senderID, uint amount)
        {
            try
            {
                accountAccess.SelectAccount(senderID);
                uint bal = accountAccess.GetBalance();
            
                TransactionDetails transaction = new TransactionDetails()
                {
                    TransactionID = transactionAccess.CreateTransaction(),
                    Amount = amount,
                    ReceiverID = receiverID,
                    SenderID = senderID
                };

                transactionAccess.SelectTransaction(transaction.TransactionID);
                transactionAccess.SetAmount(transaction.Amount);
                transactionAccess.SetRecvr(transaction.ReceiverID);
                transactionAccess.SetSendr(transaction.SenderID);

                return transaction;
            }
            catch(Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Transaction/SelectTransaction/{transactionID}")]
        public TransactionDetails SelectTransaction(uint transactionID)
        {
            try
            {
                transactionAccess.SelectTransaction(transactionID);
                TransactionDetails transaction = new TransactionDetails()
                {
                    TransactionID = transactionID,
                    Amount = transactionAccess.GetAmount(),
                    ReceiverID = transactionAccess.GetRecvrAcct(),
                    SenderID = transactionAccess.GetSendrAcct()
                };
                return transaction;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Transaction/GetAmount/{transactionID}")]
        //Get the amount transferred for a selected transaction denoted by given ID
        public uint GetAmount(uint transactionID)
        {
            try
            {            
                transactionAccess.SelectTransaction(transactionID);
                return transactionAccess.GetAmount();
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Transaction/GetSendrAcct/{transactionID}")]
        public Account GetSenderAccount(uint transactionID)
        {
            try
            {
                transactionAccess.SelectTransaction(transactionID);
                Account senderAccount = new Account()
                {
                    AccountID = transactionAccess.GetSendrAcct(),
                    Balance = accountAccess.GetBalance(),
                    UserID = accountAccess.GetOwner()
                };
                accountAccess.SelectAccount(senderAccount.AccountID);
                return senderAccount;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Transaction/GetRecvrAcct/{transactionID}")]
        public Account GetReceiverAccount(uint transactionID)
        {
             try
            {
                transactionAccess.SelectTransaction(transactionID);
                Account receiverAccount = new Account()
                {
                    AccountID = transactionAccess.GetRecvrAcct(),
                    Balance = accountAccess.GetBalance(),
                    UserID = accountAccess.GetOwner()
                };
                accountAccess.SelectAccount(receiverAccount.AccountID);// ??
                return receiverAccount;
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Transaction/SetAmount/{transactionID}/{amount}")]
        public void setAmount(uint transactionID, uint amount)
        {
            try
            {
                transactionAccess.SelectTransaction(transactionID);
                transactionAccess.SetAmount(amount);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Transaction/SetSendr/{transactionID}/{senderID}")]// elaborate, point the transaction interface a whole object from the account interface
        public void setSender(uint transactionID, uint senderID)
        {
            try
            {
                transactionAccess.SelectTransaction(transactionID);
                transactionAccess.SetSendr(senderID);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }

        [HttpPost]
        [Route("api/Transaction/SetRecvr/{transactionID}/{senderID}")]
        public void setReceiver(uint transactionID, uint senderID)
        {
            try
            {
                transactionAccess.SelectTransaction(transactionID);
                transactionAccess.SetRecvr(senderID);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
            }
        }
    }
}
