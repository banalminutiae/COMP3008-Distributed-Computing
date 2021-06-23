using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tutorial_7_BlockChain.Models
{
    public class Block
    {
        public uint blockID;
        public uint walletIDFrom;
        public uint walletIDTo;
        public float amount;
        public uint offset;
        public string prevHash;
        public string currentHash;
    }
}