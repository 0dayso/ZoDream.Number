using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Number.Helper.Export
{
    internal class Txt
    {
        public static void Export(IList<string> lists, string fullPath)
        {
            var fi = new FileInfo(fullPath);
            if (fi.Directory != null && !fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            var sw = new StreamWriter(fullPath, true, System.Text.Encoding.UTF8);
            foreach (var item in lists)
            {
                sw.WriteLine(item);
            }
            sw.Close();
        }
    }
}
