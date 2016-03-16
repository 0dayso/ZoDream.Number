using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZoDream.Number.Helper
{
    public class NumberHelper
    {
        public static List<string> Get(string html)
        {
            return new NumberHelper().GetNumber(html);
        }

        public List<string> GetNumber(string html)
        {
            //html = GetText(html).Replace(" ", "");
            var ms = Regex.Matches(html, @"[1１][34578３４５７８][\d０１２３４５６７８９]{9}");
            return (from Match item in ms select ToDbc(item.Value)).ToList();
        }

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ToDbc(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var c = input.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 去重复数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<string> GetUnque(List<string> list)
        {
            var list1 = new List<string>();
            var hash = new Hashtable();
            foreach (var stu in list)
            {
                var kk1 = stu.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries);
                var comword = kk1.Length == 3 ? kk1[2] : "";
                if (hash.ContainsKey(comword)) continue;
                hash.Add(comword, comword);
                list1.Add(stu);
            }
            hash.Clear();
            return list1;
        }
    }
}
