using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json;
using IronPython.Runtime;
using IronPython.Hosting;
using ClientLibrary;
using System.ServiceModel;
using PeerToPeerWebServer.Models;
using JobLibrary;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Threading;
using System.Diagnostics;

namespace ClientApplication
{
    // TODO: elegant solution to null client list or bad client list request
    public partial class MainWindow : Window
    {
        private RestClient client;
        private List<Client> clientList;
        private readonly string webServerUrl = "https://localhost:44355/";
         
        private uint portNum = 8100;

        public MainWindow()
        {
            InitializeComponent();
            client = new RestClient(webServerUrl);

            // start server and network threads off gui thread
            Thread networkingThread = new Thread(NetworkingThreadFunction);
            networkingThread.Start();

            Thread serverThread = new Thread(ServerThreadFunction);
            serverThread.Start();

            // WPF element defaults for iron python
            PythonInput.AcceptsReturn = true;
            PythonInput.AcceptsTab = true;
        }

        public List<Client> GetClientList()
        {
            // raises serialised exceptions, caller thread also handles error upon null client list
            List<Client> requestedList = null;
            var request = new RestRequest("api/Get");
            var resp = client.Get(request);
            if (!resp.IsSuccessful)
            {
                MessageBox.Show("Server request error in networking thread. Request was not successful");
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                MessageBox.Show("Error " + resp.StatusCode);
            }
            else
            {
                requestedList = JsonConvert.DeserializeObject<List<Client>>(resp.Content);
            }
            return requestedList;
        }

        // Takes in user-submitted python code and packages it to the server thread through a static interim data object.
        // Also start the server and networking thread
        public void UploadPythonButtonClick(object sender, RoutedEventArgs e)
        {
             if (!String.IsNullOrEmpty(PythonInput.Text))
             {
                 SHA256 SHA256Hash = SHA256.Create();
                 byte[] byteSrc  = Encoding.UTF8.GetBytes(PythonInput.Text);
                 string base64Src = Convert.ToBase64String(byteSrc);

                 byte[] hashBytes = Encoding.UTF8.GetBytes(base64Src );
                 byte[] hashedData = SHA256Hash.ComputeHash(hashBytes);

                //send to networking thread by putting it in joblist
                Job job = new Job()
                {
                    pythonSrc = base64Src,
                    hash = hashedData,
                    jobNum = JobList.jobList.Count() + 1,
                };

                JobList.jobList.Add(job);
            }
            else
            {
                MessageBox.Show("Invalid Python 2");  
            }
            
        }

        //client communication and running python
        public void NetworkingThreadFunction()
        {
            SHA256 hash = SHA256.Create();
            // early declaration and non-global/field prevents reinstantiating with every loop or race conditions
            string clientURL;
            NetTcpBinding tcp = new NetTcpBinding();
            ChannelFactory<ClientInterface> clientInterfaceFactory;
            ClientInterface clientComponent;

            while (true) // spinlock
            {
                clientList = GetClientList();
                if (clientList != null)
                {

                    //for each client, connect to the ip and port on the list and query any existing jobs
                    foreach (var c in clientList)
                    {
                        try
                        {
                            // gotta display job count



                            if (portNum != c.ports)// prevent clashing endpoints
                            {
                                // connect to client IP and port
                                clientURL = "net.tcp//" + c.ConstructClientURL() + "/JobServer";
                                clientInterfaceFactory = new ChannelFactory<ClientInterface>(tcp, clientURL);
                                clientComponent = clientInterfaceFactory.CreateChannel();
                                Job currentJob = clientComponent.RequestJob(); // gets a job i.e. python script info

                                // decode python source
                                byte[] decodedBytes =  Convert.FromBase64String(currentJob.pythonSrc);
                                string pySrc = Encoding.UTF8.GetString(decodedBytes);
                                byte[] hashArray = currentJob.hash;
                                byte[] computedSrcHash = hash.ComputeHash(Encoding.UTF8.GetBytes(currentJob.pythonSrc));

                                if (computedSrcHash.SequenceEqual(hashArray))
                                {
                                    try
                                    {
                                        // python interpretation
                                        var pythonEngine = Python.CreateEngine();
                                        var pythonScope = pythonEngine.CreateScope();

                                        pythonEngine.Execute(currentJob.pythonSrc, pythonScope);
                                        dynamic func = pythonScope.GetVariable("func");
                                        var result = func();
                                        // display on presentation tier

                                    }
                                    catch (SyntaxErrorException)
                                    {
                                        MessageBox.Show("Synatx error in the python somewhere can't help you sry");
                                    }
                                    catch (UnboundNameException)
                                    {
                                        MessageBox.Show("Invalid variable names found in the python script somehow this isn't covered by the above exception");
                                    }
                                }
                                clientComponent.UploadJobSolution(currentJob.pythonRes, currentJob.jobNum);
                                Dispatcher.Invoke(() =>
                                {
                                    Thread.Sleep(1000);
                                    Debug.WriteLine(currentJob.pythonRes);
                                    JobCompletionStatus.Content = "Job Completed";
                                });
                            }
                        }
                        catch(EndpointNotFoundException)
                        {
                            // something wrong with the IP or port for this job, remove perhaps?
                            MessageBox.Show("Error in IP or port for client " + c.ConstructClientURL());
                        }
                    }
                }
            }
        }

        // thread that utilises .NET Remoting somewhat
        public void ServerThreadFunction()
        {
            bool closeConnection = false;
            NetTcpBinding tcp;
            ServiceHost host;

            clientList = GetClientList();
            if (clientList != null)
            {
                while (true)
                {
                    try
                    {
                        // connect to client with .NET remotings
                        host = new ServiceHost(typeof(JobServer));
                        tcp = new NetTcpBinding();
                        host.AddServiceEndpoint(typeof(ClientInterface), tcp, "net.tcp://127.0.0.1:" + portNum.ToString() + "/JobServer");
                        host.Open();

                        var request = new RestRequest("api/Register");
                        // request.AddHeader("Content-type", "application/json");    
                        request.AddJsonBody(new Client
                        {
                            IPAddress = "127.0.0.1",
                            ports = portNum,
                        });

                        var resp = client.Post(request);
                        if (!resp.IsSuccessful)
                        {
                            MessageBox.Show("Server request errpr");
                        }
                        else if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            MessageBox.Show("Error: " + resp.StatusCode + " Could not register python interpretation job");
                        }

                        while (!closeConnection) { } // hold the server open

                        host.Close();
                    }
                    catch (AddressAlreadyInUseException)
                    {
                        portNum++;//failed, try again with next port
                        host = new ServiceHost(typeof(JobServer));
                    }
                }
            }
        }
    }
}
