using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NPEG.Extensions
{
  public static class StringExtensions
  {
    public static string ToHexStringFromByteArray(this byte[] bytes)
    {
      var hex = new StringBuilder(bytes.Length * 2);
      foreach (byte b in bytes)
        hex.AppendFormat(" {0:x2}", b);
      return hex.ToString();
    }

    public static List<byte> ToBytesFromHexString(string hexString)
    {
      var items = Regex.Matches(hexString, @"(?<Hex>[0-9a-fA-F]{2})");

      return (from Match item in items select Convert.ToByte(item.Groups["Hex"].Value, 16)).ToList();
    }
  }
}
