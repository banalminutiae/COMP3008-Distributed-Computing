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
using DBInterface;
using System.ServiceModel;
using System.Drawing;

namespace DBClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataServerInterface foob;
        public MainWindow()
        {
            InitializeComponent();
            var tcp = new NetTcpBinding();
            var channelFactory =
                new ChannelFactory<DataServerInterface>(tcp, "net.tcp://localhost:8100/Dataservice");
            foob = channelFactory.CreateChannel();

            NumItemsBox.Content = "Total Items: " + foob.GetNumEntries();
            IndexBox.Text = "0";
        }

        // Calls data tier information with .NET remoting
        private void IndexButtonClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(IndexBox.Text, out int index))
            {
                try
                {
                    foob.GetValuesForEntry(index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out var icon);
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
            else
            {
                MessageBox.Show("Invalid Integer");
            }
        }

    }
}
