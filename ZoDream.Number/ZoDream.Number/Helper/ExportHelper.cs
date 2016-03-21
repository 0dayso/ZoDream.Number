using System;
using System.Collections.Generic;
using ZoDream.Number.Helper.Export;

namespace ZoDream.Number.Helper
{
    public class ExportHelper
    {
        /// <summary>
        /// 添加时间
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string ExportRandomName(IList<string> lists, string suffix = "Number-")
        {
            return Export(lists, GetRandomPath(suffix));
        }

        public static string Export(IList<string> lists, string file)
        {
            Txt.Export(lists, file);
            return file; ;
        }

        /// <summary>
        /// 获取时间变化的文件路径
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="ext">拓展名</param>
        /// <returns></returns>
        public static string GetRandomPath(string prefix, string ext = "txt")
        {
            return AppDomain.CurrentDomain.BaseDirectory + "Mobile\\" + prefix + DateTime.Now.ToFileTime() + "." + ext;
        }

        public static string ExportCsv(IList<string> lists, string file)
        {
            Csv.Export(lists, file);
            return file; ;
        }
    }
}
