using System;
using System.Security.Cryptography;
using System.Text;

using Doitsu.Service.Core.Utils;

namespace System
{
    public static class StringUtils
    {
        public static string Md5Hash(this string value)
        {
            return OptionalUtils.Using(MD5.Create, md5 => value.Map(Encoding.UTF8.GetBytes)
                .Map(md5.ComputeHash).Map(Convert.ToBase64String));
        }

        public static string Sha256Hash(this string value)
        {
            return OptionalUtils.Using(SHA256.Create, sha256 => value.Map(Encoding.UTF8.GetBytes)
                .Map(sha256.ComputeHash).Map(Convert.ToBase64String));
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return String.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !String.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return !String.IsNullOrWhiteSpace(str);
        }
    }
}