using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlocksLibrary
{
    public class TransactionStruct
    {
        public uint walletIDFrom;
        public uint walletIDTo;
        public float amount;
        public bool processed;
    }
}