using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.CompilerServices;
using BusinessTierInterface;
using DBInterface;
using System.Drawing;

namespace BusinessServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class BusinessServer : BusinessServerInterface
    {
        private int logNum = 0;

        // connects to the open endpoint provided by the data tier
        private static NetTcpBinding tcp = new NetTcpBinding();
        private static ChannelFactory<DataServerInterface> dataComponentFactory = new ChannelFactory<DataServerInterface>(tcp, "net.tcp://localhost:8100/DataService");
        private static DataServerInterface dataComponent = dataComponentFactory.CreateChannel();

        public int GetNumEntries()
        {
            int numEntries =  dataComponent.GetNumEntries();
            Log(numEntries + " entries in database");
            return numEntries;
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fname, out string lname, out Bitmap icon)
        {
            if (index < 0 || index >= GetNumEntries())
            {
                throw new FaultException<IndexOutOfRangeFault>(new IndexOutOfRangeFault()
                { Message = "Index out of range" });
            }
            dataComponent.GetValuesForEntry(index, out acctNo, out pin, out bal, out fname, out lname, out icon);
            Log("Retrieved entry: " + fname + " " + lname + acctNo + " " + pin + " " + bal  + " at index " + index);
        }

        // checks search term for matching last name, then return database index
        public int Search(string keyword)
        {
            int matchingIdx = -1;

            for(int i = 0; i < dataComponent.GetNumEntries(); i++)
            {
                // we're only interested in the index in which the matching entry was found
                // returning the entire object is an alternative solution but complicates the client side
                GetValuesForEntry(i, out _, out _, out _, out _, out string lname, out _);
                if (lname.Equals(keyword))
                {
                    matchingIdx = i;
                    Log("Matching Entry at index " + matchingIdx + " for search term " + lname);
                    break;
                }
            }
            if (matchingIdx == -1)
            {
                throw new FaultException<EntryNotExistantFault>(new EntryNotExistantFault()
                { Message = "No corresponding entry exists for this last name" });
            }
            return matchingIdx;// if the result is still -1 then we know there was no match
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Log(string logString)
        {
            logNum++;
            Console.WriteLine("Log " + logNum + ": " + logString + "\n");
        }
    }
}
