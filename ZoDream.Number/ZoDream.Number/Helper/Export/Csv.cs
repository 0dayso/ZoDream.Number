using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Number.Helper.Export
{
    class Csv
    {
        /// <summary>
        /// 写入到CSV文件中
        /// </summary>
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
                sw.WriteLine(GetString(item));
            }
            sw.Close();
        }

        public static string GetColumn(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = GetString(args[i]);
            }
            return string.Join(",", args);
        } 

        /// <summary>
        /// 转义字符串
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetString(string content)
        {
            content = content.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
            if (content.Contains(',') || content.Contains('"')
                || content.Contains('\r') || content.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
            {
                content = $"\"{content}\"";
            }
            return content;
        }
    }
}
