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
using System.Text.RegularExpressions;
using RestSharp;
using BizGUI;
using System.Drawing;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DBClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string URL = "https://localhost:44316/";
        private RestClient client;

        public MainWindow()
        {
            InitializeComponent();
            client = new RestClient(URL);

            RestRequest request = new RestRequest("api/GetNumEntries");
            IRestResponse numOfItems = client.Get(request);

            NumItemsBox.Content = "Number of Entries: " + numOfItems.Content;
            URLBox.Text = URL;
            IndexBox.Text = "0";
        }

        private void IndexButtonClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(IndexBox.Text, out int index))
            {
                LoadDetails(index);
            }
            else
            {
                MessageBox.Show("Invalid index value");
            }
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            var request = new RestRequest("api/SearchController/Search/").AddJsonBody(SearchBox.Text);

            IRestResponse resp = client.Post(request);
            
            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                MessageBox.Show("Error searching for entry");
            }
            else
            {
                SearchResponse deserialisedResponse = JsonConvert.DeserializeObject<SearchResponse>(resp.Content);
                LoadDetails(deserialisedResponse.matchingIndex);
            }
        }

        private void LoadDetails(int index)
        {
            if (index == -1)
            {
                MessageBox.Show("No matches found");
            }
            else
            {
                var request = new RestRequest("api/GetValuesForEntry/" + index.ToString());
                IRestResponse resp = client.Post(request);

                if (!resp.IsSuccessful)
                {
                    MessageBox.Show("Bad request response");
                }
                if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Failed to find entry for given index");
                }
                else
                {
                    try
                    {
                        OutputLabel.Content = resp.Content; // just to show that it is in fact returning
                        DataIntermediate data = JsonConvert.DeserializeObject<DataIntermediate>(resp.Content); 
                        
                        FirstNameBox.Text = data.fname;
                        LastNameBox.Text = data.lname;
                        BalanceBox.Text = data.bal.ToString("C");
                        PinBox.Text = data.pin.ToString("D4");
                        AccountNumberBox.Text = data.acct.ToString();
                    }
                    catch(NullReferenceException)
                    {
                        MessageBox.Show("wack");
                    }
                }
            }
        }

        private void SetURlButton(object sender, RoutedEventArgs e)
        {
            if (Uri.IsWellFormedUriString(URLBox.Text, UriKind.Absolute))
            {
                URL = URLBox.Text;
                URLBox.Text = URL;
                client = new RestClient(URL);
            }
            else
            {
                MessageBox.Show("Invalid URI Format");
            }
        }

        private void SetGuiWaitingState()
        {
            IndexBox.IsReadOnly = true;
            SearchBox.IsReadOnly = true;
            IndexButton.IsEnabled = false;
            SearchButton.IsEnabled = false;
        }
    }
}

