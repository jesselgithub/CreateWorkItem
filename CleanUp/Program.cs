using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanUp
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[]
            {""
                //@"D:\JDOCS\IC007161\Documents\Visual Studio 2015\Projects\CreateWorkItem\CreateFeatures\Fields\Feature.txt",
                //@"D:\JDOCS\IC007161\Documents\Visual Studio 2015\Projects\CreateWorkItem\CreateFeatures\Fields\Request.txt",
                //@"D:\JDOCS\IC007161\Documents\Visual Studio 2015\Projects\CreateWorkItem\CreateFeatures\Fields\Task.txt"
            };
            foreach (string s in args)
            {
                var text = File.ReadAllLines(s);
                StringBuilder sb = new StringBuilder();
                foreach (string line in text)
                {
                    var elements = line.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                    sb.AppendLine($"{elements[0],-60} | {elements[1],-60} | {elements[2],-60} | {elements[3],-60} | {elements[4],100}");
                }
                File.WriteAllText(s, sb.ToString());
            }
        }
    }
}
