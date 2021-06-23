using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.ServiceModel;
using DBInterface;

namespace WebApp.Models
{
    public class DataModel
    {
        private NetTcpBinding tcp;
        private string URL;
        private ChannelFactory<DataServerInterface> dataComponentFactory;
        private DataServerInterface dataComponent;

        public DataModel()
        {
            tcp = new NetTcpBinding();
            URL = "net.tcp://localhost:8100/DataService";
            // Set URL and open port
            dataComponentFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            dataComponent = dataComponentFactory.CreateChannel();
        }

        //data tier interface implementation calls
        public void GetValuesForEntry(int idx, out uint acctNo, out uint pin, out int balance, out string fname, out string lname, out Bitmap icon)
        {
            dataComponent.GetValuesForEntry(idx, out acctNo, out pin, out balance, out fname, out lname, out icon);
        }

        public int GetNumEntries()
        {
            return dataComponent.GetNumEntries();
        }
    }
}