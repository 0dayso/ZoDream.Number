using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Number.Helper.Import;

namespace ZoDream.Number.Helper
{
    public class ImportHelper
    {
        public static string Import(string file)
        {
            return Txt.Import(file);
        }
    }
}
