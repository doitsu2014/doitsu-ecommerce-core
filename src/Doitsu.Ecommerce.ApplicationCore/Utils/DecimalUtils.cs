using System;
using System.Globalization;

namespace Doitsu.Ecommerce.ApplicationCore.Utils
{
    public static class DecimalUtils
    {
        public static string ConvertToVND(this decimal data)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            string vnd = data.ToString("#,###", cul.NumberFormat);
            return vnd;
        }
    }
}
