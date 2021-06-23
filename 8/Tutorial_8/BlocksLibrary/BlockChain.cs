using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace BlocksLibrary
{
    // Block chain operations lifted from the previous tutorial
    public class BlockChain
    {
        private static List<Block> blockChain = new List<Block>(); 

        static BlockChain()
        {
            //initial empty chain
            Block block = new Block()
            {
                blockID = 0,
                walletIDFrom = 0,
                walletIDTo = 0,
                amount = float.MaxValue,
                offset = 0,
                prevHash = "",
                currentHash = "",
            };
            block = GenerateHash(block);

            blockChain.Add(block);
        }

        public static List<Block> GetChain()
        {
            return blockChain;
        }

        public static void UpdateChain(List<Block> newChain)
        {
            blockChain = newChain;
        }

        public static void AddBlock(Block block)
        {
            if (IsLargestID(block.blockID) &&
                block.amount <= GetBalance(block.walletIDFrom) &&
                block.amount > 0 &&
                block.prevHash == GetLastBlock().currentHash &&
                block.currentHash.StartsWith("12345") &&
                IsValidHash(block) &&
                AllFieldsPositive(block))

            {
                blockChain.Add(block);
                Debug.WriteLine("Block successfully added");
            }
            else
            {
                Debug.WriteLine("Invalid block");
            }

        }

        public static Block GenerateHash(Block block)
        {
            bool validHashFound = false;
            SHA256 hashCode = SHA256.Create();

            while (!validHashFound)
            {
                //brute force the hash, first iteration is null and iterates from there
                while (!block.currentHash.StartsWith("12345"))// worksheet specifies that the hash must end with "54321" but that hash takes at least 30 minutes to generate
                {
                    // new block offsets start at zero so this is always a multiple of five
                    block.offset = block.offset + 5;
                    // this is hideous I'm sorry
                    block.currentHash = BitConverter.ToUInt64(hashCode.ComputeHash(Encoding.UTF8.GetBytes(hashBlock(block))), 0).ToString();
                }
                validHashFound = true;
                Debug.WriteLine("Valid hash generated");
            }
            return block;
        }

        public static bool IsLargestID(uint ID)
        {
            uint largestID = 0;
            foreach (var block in blockChain)
            {
                if (block.blockID > largestID)
                {
                    largestID = block.blockID;
                }
            }
            if (ID >= largestID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static float GetBalance(uint walletID)
        {
            float bal = 0;
            bool existingID = false;
            if (walletID == 0)// if first block, just return the max val. Without this check it will iterate and deduct itself
            {
                return float.MaxValue;
            }
            else
            {
                // traverse and perform/reflect block transactions 
                foreach (var block in blockChain)
                {
                    if (block.walletIDFrom == walletID)
                    {
                        existingID = true;
                        bal -= block.amount;
                    }
                    else if (block.walletIDTo == walletID)
                    {
                        existingID = true;
                        bal += block.amount;
                    }
                }
                if (existingID)
                {
                    return bal;
                }
                else
                {
                    return -1;
                }
            }
        }

        public static bool IsValidHash(Block block)
        {
            bool valid = false;
            SHA256 hash = SHA256.Create();

            string blockString = hashBlock(block);

            // sorry
            string blockStringHash = BitConverter.ToUInt64(hash.ComputeHash((Encoding.UTF8.GetBytes(blockString))), 0).ToString();

            if (block.currentHash == blockStringHash) valid = true;

            return valid;
        }

        // stringifies given block fields
        public static string hashBlock(Block block)
        {
            return block.blockID.ToString() + block.walletIDFrom.ToString() + block.walletIDTo.ToString() + block.amount.ToString()
                + block.offset.ToString() + block.prevHash.ToString();
        }

        public static bool AllFieldsPositive(Block block)
        {
            bool allFieldsPositive = false;
            if (block.amount >= 0 && block.blockID >= 0 &&
                block.offset >= 0 && block.walletIDFrom >= 0 &&
                block.walletIDTo >= 0)
            {
                allFieldsPositive = true;
            }
            return allFieldsPositive;
        }

        public static int GetNumBlocks()
        {
            return blockChain.Count;
        }

        public static Block GetLastBlock()
        {
            return blockChain.Last();
        }
    }
}
