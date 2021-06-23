using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.ServiceModel;
using RestSharp;
using Newtonsoft.Json;
using ClientLibrary;
using System.Diagnostics;
using BlocksLibrary;
using System.Net.Http;
namespace ClientApplication
{
   
    // Main application for the pper-to-peer blockchain app
    // Essentially the functionality in tutorial 7's block and transaction processing placed over the design of tutorila 6 i.e. main threads
    // here in the client are run, with the miner/operations thread doing work that is distributede over the peer to peert
    // architecture of tutorial 6 i.e. broadcasted over a list of clients connected by .NET remoting
    public partial class MainWindow : Window
    {
        private RestClient restClient;
        private string URL = "https://localhost:44347/"; // block chain webserver that manages client list
        private bool threadStarted = false; // miner thread is processing transactions
        bool portsOpen = false; // clients have ports they can connect to 

        private static readonly object threadLock = new object(); // essentially a mutex to prevent simultaneous access of data
        public MainWindow()
        {
            InitializeComponent();
            NumBlocksDisplay.Content = BlocksLibrary.BlockChain.GetNumBlocks();
            restClient = new RestClient(URL);
            Thread OpsThread = new Thread(Operations);
            OpsThread.Start();
            Thread BCThread = new Thread(BlockchainServer);
            BCThread.Start();
        }

        private void CheckBalanceClick(object sender, RoutedEventArgs e)
        {
            string walletIDStr = BalanceBox.Text;

            if (uint.TryParse(walletIDStr, out uint walletIDFrom))
            {
                float balance = BlocksLibrary.BlockChain.GetBalance(walletIDFrom);
                if (balance == -1)
                {
                    MessageBox.Show("Invalid ID: Corresponding wallet does not exist in blockchain");
                }
                else
                {
                    BalanceDisplay.Content = balance.ToString();
                }
            }
            else
            {
                MessageBox.Show("Invalid ID Format");
            }
        }

        private void CreateTransactionButtonClick(object sender, RoutedEventArgs args)
        {
            lock (threadLock) // one transaction registered at a time for all clients
            {
                string walletIDFromStr = WalletIDFromBox.Text.ToString();
                string walletIDToStr = WalletIDToBox.Text.ToString();
                string transactionBalance = AmountBox.Text.ToString();

                if (!uint.TryParse(walletIDFromStr, out uint walletIDFrom) || !uint.TryParse(walletIDToStr, out uint walletIDTo) || !float.TryParse(transactionBalance, out float amount))
                {
                    MessageBox.Show("Invalid transaction input format");
                }
                else if (walletIDFrom == walletIDTo)
                {
                    MessageBox.Show("Sending and receiving IDs cannot be the same");
                }
                else
                {
                    // send new transaction to all clients
                    // get client list 
                    NetTcpBinding tcp = new NetTcpBinding();
                    RestRequest getClientListRequest = new RestRequest("api/Client/GetClientList");
                    IRestResponse clientListResp = restClient.Get(getClientListRequest);

                    if (clientListResp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        MessageBox.Show("Error: Could not create request to client server");
                    }
                    else
                    {
                        List<Client> clientList = JsonConvert.DeserializeObject<List<Client>>(clientListResp.Content);

                        try
                        {
                            // do the transaction for every client's blockchain through .NET remoting
                            foreach (var c in clientList)
                             {
                                string currentClientURL = "net.tcp://" + c.ConstructClientURL() + "/BlockChainServer";
                                // very inefficient
                                Debug.WriteLine("Attempting to connect to client " + c.ConstructClientURL());
                                var blockchainFactory = new ChannelFactory<BlockchainServerInterface>(tcp, currentClientURL);
                                var clientBlockchainComponent = blockchainFactory.CreateChannel();

                                clientBlockchainComponent.ReceiveNewTransaction(walletIDFrom, walletIDTo, amount);
                                Debug.WriteLine("Transaction received by client " + c.ConstructClientURL());
                            }
                        }
                        catch (EndpointNotFoundException e)
                        {
                            MessageBox.Show("Error: " + e.Message);
                            Debug.WriteLine("Endpoint not found to create transaction at client " + e.Message);
                        }
                    }
                }
            }
        }

        // presents endpoints for clients to connect to
        private async void BlockchainServer()
        {
            ServiceHost host = new ServiceHost(typeof(BlockchainServer));
            uint port = 8100;
            while (!threadStarted) // spinlocking is inefficient but convenient, though Lock() should be considered
            {
                try
                {
                    NetTcpBinding tcp = new NetTcpBinding();
                    // borad cast endpoint to allow adding blockchainserverinterface methods to be available via .NET remoting
                    host.AddServiceEndpoint(typeof(BlockchainServerInterface), tcp, "net.tcp://127.0.0.1:" + port.ToString() + "/BlockchainServer");
                    host.Open();
                    Debug.WriteLine("net.tcp://127.0.0.1" + port + " opened");
                    threadStarted = true;
                    // register client with thread safety
                    // first client to be made at 127.0.0.1:8100 
                    var registrationRequest = new RestRequest("api/Client/Register").AddJsonBody(new Client() 
                    { 
                        IPAddress = "127.0.0.1",
                         ports = port
                    });
                    var registrationResp = await restClient.ExecutePostAsync(registrationRequest);
                    
                    threadStarted = true;
                    while (portsOpen) { }; // hold the endpoint open
                    host.Close();
                }
                catch (AddressAlreadyInUseException)
                {
                    Debug.WriteLine("Address already in use at " + port.ToString() + " | Trying again at next port");
                    port++;
                    host = new ServiceHost(typeof(BlockchainServer)); // try again
                }
                catch (Exception e) // http response exceptions caught this way e.g. bad status codes
                {
                    Debug.WriteLine(e.Message);
                }
            }
            Thread.Sleep(3000);
        }

        // Might also be referred to as the mining thread, processes transactions to be added into
        // the chain. Code lifted from last tutorial's mining controller
        private void Operations()
        {
            while (true)
            {
                try
                {
                    var transactions = Transactions.GetTransactions();
                    if (transactions.Count > 0)
                    {
                        var t = transactions.Dequeue();
                        if (!t.processed)
                        {
                            uint walletIDFrom = t.walletIDFrom;
                            uint walletIDTo = t.walletIDTo;
                            float amount = t.amount;

                            if (walletIDFrom >= 0 && walletIDTo >= 0 && amount >= 0)
                            {
                                // check that the actual chain has enough balance to make this transaction
                                float balance = BlocksLibrary.BlockChain.GetBalance(walletIDFrom);
                                if (balance >= amount)
                                {
                                    Block lastBlock = BlocksLibrary.BlockChain.GetLastBlock();

                                    // inefficient, refactor to make one block per 5 transactions later
                                    Block newBlock = new Block()
                                    {
                                        amount = amount,
                                        blockID = lastBlock.blockID++,
                                        offset = lastBlock.offset + 5,
                                        walletIDFrom = walletIDFrom,
                                        walletIDTo = walletIDTo,
                                        prevHash = lastBlock.currentHash,
                                        currentHash = " ",
                                    };
                                    // append and finalise
                                    newBlock = BlocksLibrary.BlockChain.GenerateHash(newBlock);
                                    BlocksLibrary.BlockChain.AddBlock(newBlock);
                                    //  Transactions.MarkTransactionAsProcessed(t);
                                }
                                else if (balance == -1)
                                {
                                    Debug.WriteLine("No corresponding wallet exists for given wallet ID");
                                }
                                else
                                {
                                    Debug.WriteLine("Not enough funds for this transaction");
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Transaction data cannot be negative");
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Empty transactions queue");
                        }

                        // this really ought to be it's own functon but the .NET remoting calls present an irritating point of failure up the
                        // call chain should it fail.
                        restClient = new RestClient(URL);
                        var clientListRequest = new RestRequest("api/Client/GetClientList");
                        var clientListResp = restClient.Get(clientListRequest);
                        if (clientListResp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            MessageBox.Show("Error connecting to client api");
                            Debug.WriteLine("Rest request to retrieve client list failed");
                        }
                        else
                        {
                            NetTcpBinding tcp = new NetTcpBinding();
                            var clientList = JsonConvert.DeserializeObject<List<Client>>(clientListResp.Content);
                            var hashFrequencyStore = new Dictionary<string, uint>();
                            string modeHash = " ";

                            foreach (var c in clientList)
                            {
                                string currentClientURL = "net.tcp://" + c.ConstructClientURL() + "/BlockchainServer";

                                ChannelFactory<BlockchainServerInterface> blockChainFactory = new ChannelFactory<BlockchainServerInterface>(tcp, currentClientURL);
                                BlockchainServerInterface blockChainComponent = blockChainFactory.CreateChannel();
                                string hash = blockChainComponent.GetCurrentBlock().currentHash;

                                if (hashFrequencyStore.ContainsKey(hash))
                                {
                                    hashFrequencyStore[hash]++; //at key, increment value i.e., frequency counter
                                }
                                else
                                {
                                    hashFrequencyStore.Add(hash, 1);
                                }
                            }
                            modeHash = GetMostPopularHash(hashFrequencyStore);

                            // resolve hash popularity issue through .NET remoting access
                            if (BlocksLibrary.BlockChain.GetLastBlock().currentHash != modeHash)
                            {
                                foreach (var c in clientList)
                                {
                                    string currentClientURL = "net.tcp://" + c.ConstructClientURL() + "/BlockchainServer";

                                    ChannelFactory<BlockchainServerInterface> blockChainFactory = new ChannelFactory<BlockchainServerInterface>(tcp, currentClientURL);
                                    BlockchainServerInterface blockChainComponent = blockChainFactory.CreateChannel();
                                    string hash = blockChainComponent.GetCurrentBlock().currentHash;

                                    if (blockChainComponent.GetCurrentBlock().currentHash == modeHash)
                                    {
                                        // swap for most popular hash
                                        BlocksLibrary.BlockChain.UpdateChain(blockChainComponent.GetBlockChain());

                                        // asynchronously update block counter
                                        Dispatcher.Invoke(() =>
                                        {
                                            NumBlocksDisplay.Content = BlocksLibrary.BlockChain.GetNumBlocks();
                                        });
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (EndpointNotFoundException e)
                {
                    Debug.WriteLine("Error connecting to the client " + e.Message);
                }
                Thread.Sleep(3000);
            }
        }

        private string GetMostPopularHash(Dictionary<string, uint> hashFrequencyStore)
        {
            string mostPopularHash = null;
            uint frequency = 0;
            foreach (var entry in hashFrequencyStore)
            {
                if (entry.Value > frequency)
                {
                    frequency = entry.Value; // highest frequency so far
                    mostPopularHash = entry.Key; 
                }
            }
            return mostPopularHash;
        }
    }
}
