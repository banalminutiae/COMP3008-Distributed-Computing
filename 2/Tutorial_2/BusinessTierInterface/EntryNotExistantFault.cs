using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BusinessTierInterface
{
    [DataContract]
    public class EntryNotExistantFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}
