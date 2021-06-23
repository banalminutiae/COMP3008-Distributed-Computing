using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace BusinessTierInterface
{
    [DataContract]
    public class IndexOutOfRangeFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}
