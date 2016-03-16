using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Number.Helper.Import
{
    class Txt
    {
        public static string Import(string file)
        {
            var content = string.Empty;
            if (!File.Exists(file)) return content;
            var tn = new TxtEncoder();
            var sr = new StreamReader(file, tn.GetEncoding(file));
            content = sr.ReadToEnd();
            sr.Close();
            return content;
        }
    }
}
