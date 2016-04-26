using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ZoDream.Helper;
using ZoDream.Helper.Http;

namespace ZoDream.Number.Helper
{
    public class NumberHelper
    {
        
        public static List<string> Get(string html)
        {
            return new NumberHelper().GetNumberWithText(html);
        }

        public List<string> GetNumberWithText(string text)
        {
            var ms = Regex.Matches(text, @"[1１][34578３４５７８][\d０１２３４５６７８９]{9}");
            return (from Match item in ms select StringHelper.ToDbc(item.Value)).ToList();
        }

        public List<string> GetNumberWithHtml(string html)
        {
            var numbers = GetNumberWithText(html);
            // 避免 分割开的
            var helper = new Html(html);
            numbers.AddRange(GetNumberWithText(helper.GetText().Replace(" ", "")));
            return numbers.Distinct().ToList();
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

        public List<string> GetNumberByRules(IList<string> numbers, string pattern)
        {
            return numbers.Where(item => Regex.IsMatch(item, pattern)).ToList();
        }

        /// <summary>
        /// 获取号段
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetSectionNumber(string number)
        {
            var match = Regex.Match(number, @"^(?<num>1[34578]\d{5})\d{4}$");
            if (match.Length <= 0)
            {
                return null;
            }
            return match.Groups["num"].Value;
        }
    }
}
