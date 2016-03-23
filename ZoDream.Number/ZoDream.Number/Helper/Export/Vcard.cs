﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Number.Helper.Export
{
    public class Vcard
    {
        public static void Export(IList<string> lists, string file)
        {
            StreamWriter writer = new StreamWriter(file, true, Encoding.UTF8);
            foreach (var item in lists)
            {
                writer.WriteLine("BEGIN:VCARD");
                writer.WriteLine("VERSION:2.1");
                writer.WriteLine("N;CHARSET=UTF-8:" + GetRandomName(int.Parse(item.Substring(6, 2))));
                writer.WriteLine("TEL:"+item);
                writer.WriteLine("END:VCARD");
            }
            writer.Close();
        }

        private static string[] _firstName = new string[]{ "白","毕","卞","蔡","曹","岑","常","车","陈","成","程","池","邓","丁","范","方","樊","费","冯","符"
                                ,"傅","甘","高","葛","龚","古","关","郭","韩","何","贺","洪","侯","胡","华","黄","霍","姬","简","江"
                                ,"姜","蒋","金","康","柯","孔","赖","郎","乐","雷","黎","李","连","廉","梁","廖","林","凌","刘","柳"
                                ,"龙","卢","鲁","陆","路","吕","罗","骆","马","梅","孟","莫","母","穆","倪","宁","欧","区","潘","彭"
                                ,"蒲","皮","齐","戚","钱","强","秦","丘","邱","饶","任","沈","盛","施","石","时","史","司徒","苏","孙"
                                ,"谭","汤","唐","陶","田","童","涂","王","危","韦","卫","魏","温","文","翁","巫","邬","吴","伍","武"
                                ,"席","夏","萧","谢","辛","邢","徐","许","薛","严","颜","杨","叶","易","殷","尤","于","余","俞","虞"
                                ,"元","袁","岳","云","曾","詹","张","章","赵","郑","钟","周","邹","朱","褚","庄","卓" };

        public static string GetRandomName(int index)
        {
            var rand = new Random(index);
            return _firstName[rand.Next(100)] + rand.Next(100).ToString("00");
        }
    }
}
