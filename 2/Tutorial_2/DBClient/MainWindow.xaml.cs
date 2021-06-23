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
using BusinessTierInterface;
using System.ServiceModel;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DBClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BusinessServerInterface businessComponent;
        public MainWindow()
        {
            InitializeComponent();
            var tcp = new NetTcpBinding();
            var channelFactory =
                new ChannelFactory<BusinessServerInterface>(tcp, "net.tcp://localhost:8000/BusinessService");
            businessComponent = channelFactory.CreateChannel();

            NumItemsBox.Content = "Total Items: " + businessComponent.GetNumEntries();
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
            try
            {
                var regex = new Regex("^[a-zA-Z]*$");
                if (!regex.IsMatch(SearchBox.Text))
                {
                    MessageBox.Show("Invalid search term format");
                }
                else
                {
                    int matchingIndex = businessComponent.Search(SearchBox.Text);
                    LoadDetails(matchingIndex);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid search term format");
            }
            catch (FaultException<EntryNotExistantFault> exception)
            {
                MessageBox.Show(exception.Detail.Message);
            }
        }

        private void LoadDetails(int index)
        {
            try
            {
                businessComponent.GetValuesForEntry(index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out var icon);
                FirstNameBox.Text = fName;
                LastNameBox.Text = lName;
                BalanceBox.Text = bal.ToString("C");
                AccountNumberBox.Text = acctNo.ToString();
                PinBox.Text = pin.ToString("D4");
                // Bitmap creation method from Alex's example solution
                Pfp.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                icon.Dispose();
            }
            catch (FaultException<IndexOutOfRangeFault> exception)
            {
                MessageBox.Show(exception.Detail.Message);
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

