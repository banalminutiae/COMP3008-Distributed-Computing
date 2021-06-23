using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Drawing;

namespace DBInterface
{
    [ServiceContract]
    public interface DataServerInterface
    {
        [OperationContract]
        int GetNumEntries();

        [OperationContract]
        [FaultContract(typeof(IndexOutOfRangeFault))] // susceptible to out of range exceptions on the client side
        void GetValuesForEntry(int index, out uint AcctNo, out uint pin, out int bal, out string fName, out string lname, out Bitmap icon);
    }
}
