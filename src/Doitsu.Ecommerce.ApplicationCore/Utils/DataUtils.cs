using System;
using System.Linq;

namespace Doitsu.Ecommerce.ApplicationCore.Utils
{
    public static class DataUtils
    {
        private static Random internalRandom = new Random();

        /// <summary>
        /// Data to gen code ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[internalRandom.Next(s.Length)]).ToArray());
        }

        public static string GenerateOrderCode(int length, Random r)
        {
            string[] data = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string code = "";
            code += data[length].ToUpper();
            int b = 1; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < length)
            {
                code += data[r.Next(data.Length - 1)].ToUpper();
                b++;
            }

            return code;
        }

        public static string GenerateRandomString(int length, bool isNonAlphanumeric = false, bool isResetPassword = false)
        {
            var validChars = @"ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789{}[]()/\'""`~,;:.<>";

            if (isNonAlphanumeric)
                validChars = validChars.Substring(0, 61);

            var random = new Random();
            var chars = new char[length];
            var i = 0;

            if (isResetPassword)
            {
                if (length < 3)chars = new char[3];
                chars[0] = 'P';
                chars[1] = 'w';
                chars[2] = '0';
                i += 3;
            }

            for (; i < length; ++i)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);

        }
    }
}