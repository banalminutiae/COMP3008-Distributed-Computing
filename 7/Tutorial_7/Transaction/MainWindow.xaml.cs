using System;
using System.Collections.Generic;
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
using RestSharp;
using Tutorial_7_BlockChain;
using Miner;
using System.Diagnostics;

namespace Transaction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RestClient client;

        private readonly string blockChainURL = "https://localhost:44351/";
        private readonly string minerURL = "https://localhost:44350/";

        public MainWindow()
        {
            InitializeComponent();
        }

        // given data for valid transaction from the client, creates it and sends to miner web service
        private async void CreateTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            uint walletIDFrom, walletIDTo;
            float transactionAmount;
            string walletIDFromInput = WalletIDFromBox.Text;
            string walletIDToInput= WalletIDToBox.Text;
            string transactionAmountInput = WalletTransactionAmountBox.Text;

            if (!uint.TryParse(walletIDFromInput, out walletIDFrom) || !uint.TryParse(walletIDToInput, out walletIDTo) ||
                (!float.TryParse(transactionAmountInput, out transactionAmount)))
            {
                MessageBox.Show("Invalid transaction information");
            }
            else if (walletIDFrom == walletIDTo)
            {
                MessageBox.Show("Sending and receiving ID's cannot be the same");
            }
            else
            {
                client = new RestClient(minerURL);
                var request = new RestRequest("api/Createtransaction/" + walletIDFrom.ToString() + "/" + walletIDTo.ToString() + "/" + transactionAmount.ToString());
                var resp = await client.ExecutePostAsync(request);
                if (!resp.IsSuccessful)
                {
                    MessageBox.Show("Server request error");
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Could not create transaction");
                }
                Refresh();
            }
        }

        private async void CheckBalancesButton(object sender, RoutedEventArgs e)
        {
            string walletIDInput = WalletIDBox.Text; 

            if (!uint.TryParse(walletIDInput, out uint walletID))
            {
                MessageBox.Show("Invalid ID input");
            }
            else
            {
                client = new RestClient(blockChainURL);
                var request = new RestRequest("api/GetBalances/" + walletID);
                var resp = await client.ExecuteAsync(request);// different to ExecuteGetAsync?
                string balance = resp.Content;
                if (!resp.IsSuccessful)
                {
                    MessageBox.Show("Server request error");
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Could not retrieve balance");
                }
                else 
                {
                    if (balance == "-1")
                    {
                        MessageBox.Show("ID does not exist in the blockchain");
                    }
                    else
                    {
                        BalanceBox.Text = balance.ToString();
                    }
                }
            }
        }
        
        // mutltithreaded shenanigans will result in block data window being out of date
        // this is called frequently to get the most recent and correct information
        private async void Refresh()
        {
            client = new RestClient(blockChainURL);
            var request = new RestRequest("api/GetCurrentState");
            var resp = await client.ExecuteAsync(request);

            if (!resp.IsSuccessful)
            {
                MessageBox.Show("Server request error");
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                MessageBox.Show("Could not retrieve block information");
            }
            else
            {
                // no need to deserialise, display IResponse string
                NumBlocksDisplay.Text = resp.Content;
            }
        }

        // methods can't call button callback methods and the updating needs to happen from the display and the function in this file
        private void BlockDisplayRefresh(object sender, RoutedEventArgs args)
        {
            Refresh();
        }
    }
}
