using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_7_BlockChain.Models;

namespace Tutorial_7_BlockChain.Controllers
{
    public class BlockChainController : ApiController
    {
        [HttpGet]
        [Route("api/GetBalances/{walletIDFrom}")]
        public float GetBalances(uint walletIDFrom)
        {
            return BlockChainModel.GetBalance(walletIDFrom);
        }

        [HttpPost]
        [Route("api/AddBlock/")]
        public void AddBlock(Block block)
        {
            BlockChainModel.AddBlock(block);
        }

        [HttpGet]
        [Route("api/GetLastBlock")]
        public Block GetLastBlock()
        {
            return BlockChainModel.GetLastBlock();
        }

        [HttpGet]
        [Route("api/GetCurrentState")]
        public int GetCurrentState()
        {
            //assuming number of blocks is what is means by current state, the  balance too perhaps?
            return BlockChainModel.GetNumBlocks();
        }
    }
}
