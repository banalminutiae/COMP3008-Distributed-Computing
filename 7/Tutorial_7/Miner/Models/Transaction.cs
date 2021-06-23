using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Miner.Models
{
    public class Transaction
    {
        public uint walletIDFrom;
        public uint walletIDTo;
        public uint amount;
        public bool processed;
    }
}