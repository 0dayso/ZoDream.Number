using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Number.Model
{
    public class NumberRule
    {
        public string Name { get; set; }

        public string Pattern { get; set; }

        public NumberRule()
        {
            
        }

        /// <summary>
        /// 根据号码规则提取
        /// </summary>
        /// <param name="rule">以空格分开</param>
        public NumberRule(string rule)
        {
            var m = Regex.Match(rule, @"^([^\s]+)\s+([^\s]+)");
            Name = m.Groups[1].Value;
            Pattern = m.Groups[2].Value;
        }

        public NumberRule(string name, string pattern)
        {
            Name = name;
            Pattern = pattern;
        }
    }
}
