using System.Collections.Generic;
using System.Drawing;

namespace DBLib
{
    public class Database
    {
        private readonly List<DataStruct> database;

        public static Database Instance { get; } = new Database();

        private Database()
        {
            database = new List<DataStruct>();
            //populate database list
            var databaseGenerator = new DatabaseGenerator();
            for (int i = 0; i < 100000; i++)
            {
                var newDataStruct = new DataStruct();
                databaseGenerator.GetNextAccount(out newDataStruct.pin, out newDataStruct.acctNo, out newDataStruct.firstName, out newDataStruct.lastName,
                                                 out newDataStruct.balance, out newDataStruct.icon);
                database.Add(newDataStruct);
            }
        }

        public uint GetAcctNoByIndex(int index)
        {
            return database[index].acctNo;
        }

        public uint GetPinByIndex(int index)
        {
            return database[index].pin;
        }

        public string GetFNameByIndex(int index)
        {
            return database[index].firstName;
        }

        public string GetLNameByIndex(int index)
        {
            return database[index].lastName;
        }

        public int GetBalanceByIndex(int index)
        {
            return database[index].balance;
        }

        public Bitmap GetIconByIndex(int index)
        {
            return database[index].icon;
        }

        public int GetNumRecords()
        {
            return database.Count;
        }
    }
}
