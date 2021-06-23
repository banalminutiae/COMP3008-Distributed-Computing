using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Drawing;

namespace BusinessTierInterface
{
    [ServiceContract]
    public interface BusinessServerInterface
    {
        [OperationContract]
        int GetNumEntries();

        [OperationContract]
        [FaultContract(typeof(IndexOutOfRangeFault))]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin,
                               out int bal, out string fname, out string lname, out Bitmap icon);

        [OperationContract]
        [FaultContract(typeof(EntryNotExistantFault))]
        int Search(string keyword);
    }
}
