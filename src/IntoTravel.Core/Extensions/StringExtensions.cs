using System;
using System.IO;
using System.Text.RegularExpressions;

namespace IntoTravel.Core.Utilities
{
    public static class StringExtensions
    {
        public static string UrlKey(this string p)
        {
            var pname = Regex.Replace(p, @"[\W_-[#]]+", " ");

            return pname.Trim().Replace("  ", " ").Replace(" ", "-").Replace("%", string.Empty).ToLower();
        }

        public static string GetFileNameFromUrl(this string url)
        {
            Uri uri = new Uri(url);

            string filename = Path.GetFileName(uri.LocalPath);

            return filename;
        }


        public static string GetFileExtensionLower(this string fileName)
        {
            return Path.GetExtension(fileName).ToLower().Replace(".", string.Empty);
        }

        public static string GetFileExtension(this string fileName)
        {
            return Path.GetExtension(fileName).Replace(".", string.Empty);
        }
    }
}