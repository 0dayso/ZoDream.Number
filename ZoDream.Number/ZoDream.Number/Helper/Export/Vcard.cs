using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.Number.Helper.Export
{
    public class Vcard
    {
        public static void Export(IList<string> lists, string file)
        {
            file = file.Replace(".vcf", "");
            int count = (int)Math.Ceiling((double)lists.Count/5000);
            var tasks = new Task[count];
            for (var i = 0; i < count; i++)
            {
                tasks[i] = new Task(index =>
                {
                    var start = (int) index * 5000;
                    var length = Math.Min(5000, lists.Count - start);
                    var writer = new StreamWriter(file + index + ".vcf", false, Encoding.ASCII);
                    var nameHelper = new NameHelper();
                    for (var j = 0; j < length; j++)
                    {
                        var name = lists[start + j];
                        writer.WriteLine("BEGIN:VCARD");
                        writer.WriteLine("VERSION:2.1");
                        writer.WriteLine("N;CHARSET=UTF-8:" + name);
                        writer.WriteLine("TEL:" + name);
                        writer.WriteLine("END:VCARD");
                    }
                    writer.Close();
                }, i);
            }
            var continuation = Task.Factory.ContinueWhenAll(tasks, (task) =>
            { });
            foreach (var task in tasks)
            {
                task.Start();
            }
            while (!continuation.IsCompleted)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
