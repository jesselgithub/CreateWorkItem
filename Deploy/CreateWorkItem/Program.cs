using System;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CreateWorkItem
{
    internal class Program
    {
        private static bool ValidateArguments(string[] args)
        {
            if (args.Length != 4)
            {
                return false;
            }
            int integer;
            if (!Int32.TryParse(args[0], out integer))
            {
                return false;
            }
            if (!args[1].Contains("\\"))
            {
                return false;
            }
            if (!args[2].Contains("\\"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 1 - WorkItemId
        /// 2 - AreaPath
        /// 3 - IterationPath
        /// 4 - Assigned To Name
        /// example
        ///     CreateWorkItem.exe   1234562  TIA\Development\TIA Portal\Engineering Platform\UIA\Library  "TIA\TIA Portal\V15\V15.0.0.0\Inc 01" "Bhaskar Babu, T. (CT DD DS AA DF-PD ES) (IN002.IC006714)"
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            if (!ValidateArguments(args))
            {
                Console.WriteLine(@"
Syntax/Usage : CreateWorkItem.exe   1  2  3  4
 1 - WorkItemId (Request)
 2 - AreaPath
 3 - IterationPath
 4 - Assigned To Name
example
    CreateWorkItem.exe   123456   ""TIA\Development\TIA Portal\Engineering Platform\UIA\Library""   ""TIA\TIA Portal\V15\V15.0.0.0\Inc 01""   ""Bhaskar Babu, T. (CT DD DS AA DF-PD ES) (IN002.IC006714)""

N O T E : Use double quotes when the argument has spaces...

");
                Environment.Exit(-1);
            }
            try
            {
                int idd = Convert.ToInt32(args[0]);
                //ParseArguments();
                TfsHandler.CreateWorkItem(idd,
                    "https://iiaastfs.ww004.siemens.net:443/tfs/tia",
                    args[3],
                    "Feature",
                    "RQ#",
                    $"{idd} - ",//@"TIA\Development\TIA Portal\Engineering Platform\UIA\Library",
                    args[1],//"Bhaskar Babu, T. (CT DD DS AA DF-PD ES) (IN002.IC006714)",
                    args[2]);
            }
            catch (Exception e)
            {
                var x = e;
                do
                {
                    Console.WriteLine($"Exception of type {x.GetType().Name} & Message='{x.Message}'");
                    Console.WriteLine(x.StackTrace);
                    x = x.InnerException;
                } while (x != null);
            }
        }
    }
}