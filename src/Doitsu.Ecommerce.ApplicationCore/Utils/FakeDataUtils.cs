using System;
using System.Collections.Generic;
using System.Text;

namespace Doitsu.Ecommerce.ApplicationCore.Utils
{
    public static class FakeDataUtils
    {

        public static string FakeProductCode(int length, Random r)
        {
            string[] data = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x","1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string Code = "";
            Code += data[r.Next(data.Length)].ToUpper();
            int b = 1; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < length)
            {
                Code += data[r.Next(data.Length)].ToUpper();
                b++;
            }

            return Code;
        }

        public static string FakeProductName(int length, Random r)
        {
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < length)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;

        }

    }
}
