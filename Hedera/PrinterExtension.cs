using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hedera
{
    public static class PrinterExtension
    {
        public static string Info(this string STRING_Message)
        {
            return $"{"[INFO]".Pastel(Color.Black).PastelBg("68C355")} - {STRING_Message}";
        }

        public static string Error(this string STRING_Message)
        {
            return $"{"[ERROR]".Pastel(Color.White).PastelBg("E93519")} - {STRING_Message}";
        }

        public static string Warning(this string STRING_Message)
        {
            return $"{"[WARN]".Pastel(Color.Blue).PastelBg("FF9800")} - {STRING_Message}";
        }

        public static string Match(this string STRING_Message)
        {
            return $"{"[MATCH]".Pastel(Color.Blue).PastelBg("FF9800")} - {STRING_Message}";
        }
    }
}
