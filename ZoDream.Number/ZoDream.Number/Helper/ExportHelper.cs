using System;
using System.Collections.Generic;
using ZoDream.Number.Helper.Export;

namespace ZoDream.Number.Helper
{
    public class ExportHelper
    {

        public static string Export(IList<string> lists)
        {
            return Export(lists, AppDomain.CurrentDomain.BaseDirectory + "Mobile\\Number-" + DateTime.Now.ToFileTime() + ".txt");
        }

        public static string Export(IList<string> lists, string file)
        {
            Txt.Export(lists, file);
            return file; ;
        }
    }
}
