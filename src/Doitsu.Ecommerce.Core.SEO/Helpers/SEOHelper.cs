using System;
using System.Collections.Generic;
using System.Text;

namespace Doitsu.Ecommerce.Core.SEO.Helpers
{
    public class SEOHelper
    {
        public static List<Uri> ChangeToUri(List<string> baseList)
        {
            var uriResult = new List<Uri>();
            baseList.RemoveAll(uri => string.IsNullOrWhiteSpace(uri));

            foreach (var uriStr in baseList)
            {
                try
                {
                    var uri = new Uri(uriStr);
                    uriResult.Add(uri);
                }
                catch (Exception)
                {
                }
            }

            return uriResult;
        }
    }
}
