﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.WebEncoders;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace IdentityServer4.Core.Extensions
{
    internal static class NameValueCollectionExtensions
    {
        public static string ToQueryString(this NameValueCollection collection)
        {
            if (collection.Count == 0)
            {
                return String.Empty;
            }

            var builder = new StringBuilder(128);
            bool first = true;
            foreach (string name in collection)
            {
                string[] values = collection.GetValues(name);
                if (values == null || values.Length == 0)
                {
                    first = AppendNameValuePair(builder, first, true, name, String.Empty);
                }
                else
                {
                    foreach (string value in values)
                    {
                        first = AppendNameValuePair(builder, first, true, name, value);
                    }
                }
            }

            return builder.ToString();
        }

        public static string ToFormPost(this NameValueCollection collection)
        {
            var builder = new StringBuilder(128);
            const string inputFieldFormat = "<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />\n";

            foreach (string name in collection)
            {
                var values = collection.GetValues(name);
                var value = values.First();
                value = HtmlEncoder.Default.HtmlEncode(value);
                builder.AppendFormat(inputFieldFormat, name, value);
            }

            return builder.ToString();
        }

        public static NameValueCollection ToNameValueCollection(this Dictionary<string, string> data)
        {
            var result = new NameValueCollection();

            if (data == null || data.Count == 0)
            {
                return result;
            }

            foreach (string name in data.Keys)
            {
                var value = data[name];
                if (value != null)
                {
                    result.Add(name, value);
                }
            }

            return result;
        }

        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            var dict = new Dictionary<string, string>();
            
            if (collection == null || collection.Count == 0)
            {
                return dict;
            }

            foreach (string name in collection)
            {
                var value = collection.Get(name);
                if (value != null)
                {
                    dict.Add(name, value);
                }
            }

            return dict;
        }

        internal static string ConvertFormUrlEncodedSpacesToUrlEncodedSpaces(string str)
        {
            if ((str != null) && (str.IndexOf('+') >= 0))
            {
                str = str.Replace("+", "%20");
            }
            return str;
        }

        private static bool AppendNameValuePair(StringBuilder builder, bool first, bool urlEncode, string name, string value)
        {
            string effectiveName = name ?? String.Empty;
            string encodedName = urlEncode ? WebUtility.UrlEncode(effectiveName) : effectiveName;

            string effectiveValue = value ?? String.Empty;
            string encodedValue = urlEncode ? WebUtility.UrlEncode(effectiveValue) : effectiveValue;
            encodedValue = ConvertFormUrlEncodedSpacesToUrlEncodedSpaces(encodedValue);

            if (first)
            {
                first = false;
            }
            else
            {
                builder.Append("&");
            }

            builder.Append(encodedName);
            if (!String.IsNullOrEmpty(encodedValue))
            {
                builder.Append("=");
                builder.Append(encodedValue);
            }
            return first;
        }
    }
}