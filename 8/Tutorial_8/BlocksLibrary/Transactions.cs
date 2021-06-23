using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlocksLibrary
{
    public static class Transactions
    {
        private static Queue<TransactionStruct> transactionQueue = new Queue<TransactionStruct>();

        public static void AddTransaction(uint walletIDFrom, uint walletIDTo, float amount)
        {
            TransactionStruct t = new TransactionStruct()
            {
                walletIDFrom = walletIDFrom,
                walletIDTo = walletIDTo,
                amount = amount,
                processed = false
            };
            transactionQueue.Enqueue(t);
        }

        public static Queue<TransactionStruct> GetTransactions()
        {
            return transactionQueue;
        }

        /*
        public static void MarkTransactionAsProcessed(TransactionStruct t)
        {
            t.processed = true;
        }
        */
    }
}
