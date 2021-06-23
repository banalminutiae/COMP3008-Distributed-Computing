using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DBLib
{
    internal class DatabaseGenerator
    {
        private Random rand = new Random();
        private readonly string[] fNameList =
        {
            "Jyoti", "Marita", "Tigernán", "Naomh", "Linda", "Christian", "Arezoo", "Raimo", "Young-Sook", "Eirenaios"
        };
        private readonly string[] lNameList =
        {
            "Roberts", "Lee", "Sandoval", "Haynes", "Owen", "Holt", "Fox", "Riley", "Hunt", "Bargas",
        };

        private readonly List<Bitmap> iconsList;

        public DatabaseGenerator()
        {
            iconsList = new List<Bitmap>();

            // bitmap generation method lifted from Alex's example tutorial
            for (int i = 0; i < 10; i++)
            {
                var image = new Bitmap(64, 64);
                for (int x = 0; x < 64; x++)
                {
                    for (int y = 0; y < 64; y++)
                    {
                        image.SetPixel(x, y, Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)));
                    }
                }
                iconsList.Add(image);
            }
        }
        private string GetFName()
        {
            return fNameList[rand.Next(fNameList.Length)];
        }

        private string GetLName()
        {
            return lNameList[rand.Next(lNameList.Length)];
        }

        private int GetBalance()
        {
            return rand.Next(-10000, 10000);
        }

        private uint GetPin()
        {
            return (uint)rand.Next(9999);
        }

        private uint GetAcctNo()
        {
            return (uint)rand.Next(100000000, 999999999);
        }

        private Bitmap GetIcon()
        {
            return iconsList[rand.Next(iconsList.Count)];
        }

        public void GetNextAccount(out uint pin, out uint acctNo, out string firstName, out string
                lastName, out int balance, out Bitmap icon)
        {
            pin = GetPin();
            acctNo = GetAcctNo();
            firstName = GetFName();
            lastName = GetLName();
            balance = GetBalance();
            icon = GetIcon();
        }
    }
}

