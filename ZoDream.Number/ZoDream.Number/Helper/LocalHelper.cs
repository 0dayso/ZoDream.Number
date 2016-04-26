using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZoDream.Helper.Local;
using ZoDream.Number.Model;

namespace ZoDream.Number.Helper
{
    public class LocalHelper
    {
        /// <summary>
        /// 从文件中直接获取号码
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<string> GetNumber(string file)
        {
            
            return NumberHelper.Get(Open.Read(file));
        }

        public static List<string> GetNumber(IList<string> files)
        {
            var numbers = new List<string>();
            foreach (var item in files)
            {
                numbers.AddRange(GetNumber(item));
            }
            numbers = numbers.Distinct().ToList();
            return numbers;
        }

        /// <summary>
        /// 从本地文件中匹配归属地查询
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static MobileItem GetInformation(string number)
        {
            var match = Regex.Match(number, @"^(?<num>1[34578]\d{5})\d{4}$");
            if (match.Length <= 0)
            {
                return new MobileItem();
            }
            var num = match.Groups["num"].Value;
            StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "mobile.sql", Encoding.UTF8);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.IndexOf(num, StringComparison.Ordinal) > 0)
                {
                    break;
                }
            }
            sr.Close();
            if (line == null)
            {
                return new MobileItem();
            }
            var ms = Regex.Matches(line, @"'([^',]+)'");
            return new MobileItem(
                num,
                ms[1].Groups[1].Value,
                ms[2].Groups[1].Value,
                ms[3].Groups[1].Value,
                ms[4].Groups[1].Value
            );
        }
        

        public static void SaveTxt(string text, string file = null)
        {
            if (string.IsNullOrEmpty(file))
            {
                file = ExportHelper.GetRandomPath("html");
            }
            FileStream fs = new FileStream(file, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(text);
            sw.Close();
        }
    }
}
