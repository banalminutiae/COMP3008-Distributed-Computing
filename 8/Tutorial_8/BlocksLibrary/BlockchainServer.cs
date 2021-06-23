using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlocksLibrary;

// Implementation of BlockchainServerInterface
namespace BlocksLibrary
{
    public class BlockchainServer : BlockchainServerInterface
    {
        public List<Block> GetBlockChain()
        {
            return BlockChain.GetChain();
        }

        public Block GetCurrentBlock()
        {
            return BlockChain.GetLastBlock();
        }

        public void ReceiveNewTransaction(uint walletIDfrom, uint walletIDTo, float amount)
        {
            Transactions.AddTransaction(walletIDfrom, walletIDTo, amount);
        }
    }
}
