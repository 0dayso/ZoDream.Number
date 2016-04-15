using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
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
            
            return NumberHelper.Get(ImportHelper.Import(file));
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


        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static List<string> GetAllFile(string dir)
        {
            var files = new List<string>();
            if (string.IsNullOrWhiteSpace(dir))
            {
                return files;
            }
            var theFolder = new DirectoryInfo(dir);
            var dirInfo = theFolder.GetDirectories();
            //遍历文件夹
            foreach (var nextFolder in dirInfo)
            {
                files.AddRange(GetAllFile(nextFolder.FullName));
            }

            var fileInfo = theFolder.GetFiles();
            //遍历文件
            files.AddRange(fileInfo.Select(nextFile => nextFile.FullName));
            return files;
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <returns></returns>
        public static string ChooseFolder()
        {
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
            folder.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            folder.ShowNewFolderButton = false;
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return folder.SelectedPath;
            }
            return null;
        }

        public static string ChooseSaveFile(string filter = "文本文件|*.txt|CSV文件|*.csv|EXCEL文件|*.xls;*.xlsx|所有文件|*.*")
        {
            var open = new SaveFileDialog();
            open.Title = "选择保存路径";
            open.Filter = filter;
            if (open.ShowDialog() == true)
            {
                return open.FileName;
            }
            return null;
        }

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <returns></returns>
        public static List<string> ChooseFile(string filter = "脚本文件|*.txt|所有文件|*.*")
        {
            var files = new List<string>();
            var open = new OpenFileDialog
            {
                Multiselect = true,
                Filter = filter,
                Title = "选择文件",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (open.ShowDialog() == true)
            {
                files.AddRange(open.FileNames);
            }
            return files;
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
