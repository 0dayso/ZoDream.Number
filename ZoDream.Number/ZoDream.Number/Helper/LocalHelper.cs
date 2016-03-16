using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ZoDream.Number.Model;

namespace ZoDream.Number.Helper
{
    public class LocalHelper
    {
        public static MobileItem Get(string number)
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
                if (line.IndexOf(num) > 0)
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

        /// <summary>
        /// 浏览文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void ExploreFile(string filePath)
        {
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = "explorer",
                    Arguments = @"/select," + filePath
                }
            };
            //打开资源管理器
            //选中"notepad.exe"这个程序,即记事本
            proc.Start();
        }

        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void ExplorePath(string path)
        {
            Process.Start("explorer.exe", path);
        }
    }
}
