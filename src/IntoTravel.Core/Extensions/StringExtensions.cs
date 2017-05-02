﻿using System;
using System.Collections.Generic;
using System.Text;
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
    }
}