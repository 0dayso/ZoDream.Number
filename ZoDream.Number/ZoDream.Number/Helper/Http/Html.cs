using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZoDream.Number.Helper.Http
{
    class Html
    {
        private string _html;

        public Html(string html)
        {
            _html = html;
        }

        public List<string> GetLinks()
        {
            return GetLinks("");
        }

        public List<string> GetLinks(string rootUrl)
        {
            var ms = GetMatches(@"\<a[^\<\>]+?[hH][Rr][Ee][fF][\s]?=[\s""]?(?<href>[^""\<\>\s]+)[^\<\>]+?\>");
            return (from Match item in ms select new Uri(new Uri(rootUrl), item.Groups["href"].Value) into uri select uri.ToString()).ToList();
        }

        public List<string> GetHref()
        {
            var ms = GetMatches(@"\<[^\<\>]+?[hH][Rr][Ee][fF][\s]?=[\s""]?(?<href>[^""\<\>\s]+)[^\<\>]+?\>");
            return (from Match item in ms select item.Groups["href"].Value).ToList();
        }

        public List<string> GetSrc()
        {
            var ms = GetMatches(@"\<[^\<\>]+?[sS][Rr][Cc][\s]?=[\s""]?(?<src>[^""\<\>\s]+)[^\<\>]+?\>");
            return (from Match item in ms select item.Groups["src"].Value).ToList();
        }
        
        public MatchCollection GetMatches(string pattern)
        {
            return Regex.Matches(_html, pattern);
        }

        public Match GetMatch(string pattern)
        {
            return Regex.Match(_html, pattern);
        }

        public string GetText()
        {
            //删除脚本   
            _html = Regex.Replace(_html, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除_html   
            _html = Regex.Replace(_html, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"([/r/n])[/s]+", "", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"-->", "", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"<!--.*", "", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&(quot|#34);", "/", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&(iexcl|#161);", "/xa1", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&(cent|#162);", "/xa2", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&(pound|#163);", "/xa3", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&(copy|#169);", "/xa9", RegexOptions.IgnoreCase);
            _html = Regex.Replace(_html, @"&#(/d+);", "", RegexOptions.IgnoreCase);
            //替换掉 < 和 > 标记
            _html = _html.Replace("<", "");
            _html = _html.Replace(">", "");
            _html = _html.Replace("/r/n", "");
            //返回去掉_html标记的字符串
            return _html;
        }

        public bool Comparer(string url, string url2)
        {
            if (url.IndexOf("//", StringComparison.Ordinal) < 0 || url2.IndexOf("//", StringComparison.Ordinal) < 0)
            {
                return true;
            }
            var regex = new Regex(@"//[^/]+");
            return regex.Match(url).Value.Equals(regex.Match(url2).Value, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
