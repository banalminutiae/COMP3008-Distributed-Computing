using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_7_BlockChain.Models;
using Miner.Models;
using RestSharp;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;

namespace Miner.Controllers
{
    public class MiningController : ApiController
    {
        private readonly string URL = "https://localhost:44351/"; // bank server url
        private static Queue<Transaction> transactionsQueue = new Queue<Transaction>(); // sorta like a job queue

        // makeshift semaphore for if the background thread is running
        private static bool threadStarted = false;

        // If the thread is not running, then it has never been run and the thread needs to start
        // Readies first transaction to be made into a block upon the thread starting
        [HttpPost]
        [Route("api/CreateTransaction/{walletIDFrom}/{walletIDTo}/{amount}")]
        public void CreateTransaction(uint walletIDFrom, uint walletIDTo, uint amount)
        {
            if (!threadStarted)  
            {
                // thread not started, begin processing of transactions in queue
                var processionThread = new Thread(ProcessTransaction);
                processionThread.Start();
                threadStarted = true;
                Debug.WriteLine("Procession thread started");
            }
            Transaction trans = new Transaction()
            {
                walletIDFrom = walletIDFrom,
                walletIDTo = walletIDTo,
                amount = amount,
                processed = false
            };
            transactionsQueue.Enqueue(trans);
            Debug.WriteLine("Transaction enqueued");
        }

        // makes requests for new blocks for new transactions to be added to the chain asynchronously
        private void ProcessTransaction()
        {
            while (true) // sorta like blocking in process management
            {
                try
                {
                    if (transactionsQueue.Count > 0)
                    {
                        Transaction trans = transactionsQueue.Dequeue();
                        if (!trans.processed) // slow enough as it is, don't need redunant computations
                        {
                            uint walletIDFrom = trans.walletIDFrom;
                            uint walletIDTo = trans.walletIDTo;
                            uint amount = trans.amount;

                            if (walletIDFrom >= 0 && walletIDTo >= 0 && amount > 0)
                            {
                                Debug.WriteLine("valid data");
                                // retrieve total blockchain balance 
                                var client = new RestClient(URL);
                                var transRequest = new RestRequest("api/GetBalances/" + walletIDFrom);
                                var resp = client.Get(transRequest);
                                float balance = float.Parse(resp.Content);

                                // check for sufficient funds to make this transaction
                                // necessary each attempt as the conditions of the chain may change during submission attempts
                                if (balance >= amount)
                                {
                                    //receive hash and ID of last block to put into new block 
                                    var lastBlockRequest = new RestRequest("api/GetLastBlock");
                                    var lastBlockResp = client.Get(lastBlockRequest);
                                    Block lastBlock = JsonConvert.DeserializeObject<Block>(lastBlockResp.Content);

                                    Block newBlock = new Block()
                                    {
                                        amount = amount,
                                        blockID = lastBlock.blockID++,
                                        offset = lastBlock.offset + 5,
                                        walletIDFrom = walletIDFrom,
                                        walletIDTo = walletIDTo,
                                        prevHash = lastBlock.currentHash,
                                        currentHash = " "
                                    };

                                    newBlock = BlockChainModel.GenerateHash(newBlock); // return with a valid current hash

                                    //append to the chain
                                    var addRequest = new RestRequest("api/AddBlock").AddJsonBody(newBlock);
                                    var addResp = client.Post(addRequest);
                                    Debug.WriteLine("Block Appended | amount: " + newBlock.amount + ", Block ID: " + newBlock.blockID + ", Offset: " + newBlock.offset + ", Sender ID: " + newBlock.walletIDFrom
                                                    + ", ReceiverID: " + newBlock.walletIDFrom + ", Previous Hash: " + newBlock.prevHash + ", Current Hash: " + newBlock.currentHash);
                                }
                                else if (balance == -1)
                                {
                                    Debug.WriteLine("Wallet does not exist");
                                }
                                else
                                {
                                    Debug.WriteLine("Not enough balance to make this transaction");
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Empty transaction Queue");
                    }
                }
                catch (Exception e)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.Message) });
                }
                Thread.Sleep(3000);
            }
        }
    }
}
