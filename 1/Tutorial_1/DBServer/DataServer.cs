using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Drawing;
using DBInterface;
using DBLib;

namespace DBServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class DataServer : DataServerInterface
    {
        private Database db = Database.Instance;//static class access

        public int GetNumEntries()
        {
            return db.GetNumRecords();
        }

        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal,
                                      out string fName, out string lName, out Bitmap icon)
        {
            if (index < 0 || index >= db.GetNumRecords())
            {
                throw new FaultException<IndexOutOfRangeFault>(new IndexOutOfRangeFault() {Message = "Index out of range"});
            }
            acctNo = db.GetAcctNoByIndex(index);
            pin = db.GetPinByIndex(index);
            bal = db.GetBalanceByIndex(index);
            fName = db.GetFNameByIndex(index);
            lName = db.GetLNameByIndex(index);
            icon = new Bitmap(db.GetIconByIndex(index));
        }
    }
}
