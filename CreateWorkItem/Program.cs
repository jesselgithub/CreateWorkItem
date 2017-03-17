using System;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CreateWorkItem
{
    class Program
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
        static void Main(string[] args)
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



        private static void ParseArguments()
        {
            CommandParser commandParser = new CommandParser("Create Workitem Application");
            string tfsUri = "https://iiaastfs.ww004.siemens.net:443/tfs/tia";
            string wiType = "Feature";
            string titlePrefix = string.Empty;
            string externalIdSuffix = string.Empty;
            string area;
            string name;
            int id;
            string iteration = @"TIA\TIA Portal\V15\V15.0.0.0";
            bool linktoid;
            bool showHelp = false;

            commandParser.Argument("i", "id", "Base Workitem ID", "int_wi_id",
                CommandArgumentFlags.TakesParameter | CommandArgumentFlags.Required,
                (p, v) =>
                {
                    int x = 0;
                    if (Int32.TryParse(v, out x))
                    {
                        id = x;
                    }
                    else
                    {
                        throw new ArgumentException("Error while parsing workitem ID: ID should be of type int");
                    }
                });
            commandParser.Argument("t", "type", "Target WI Type-{Feature, Request, Requirement etc.}", "workitemType",
                CommandArgumentFlags.TakesParameter | CommandArgumentFlags.Required,
                (p, v) =>
                {
                    wiType = v;
                });

            commandParser.Argument("a", "area", "Area Path", "str_area_path",
                CommandArgumentFlags.TakesParameter | CommandArgumentFlags.Required,
                (p, v) =>
                {
                    area = v;
                });

            commandParser.Argument("it", "iteration", @"Iteration Path. Default is TIA\TIA Portal\V15\V15.0.0.0", "str_iteration_path",
                CommandArgumentFlags.TakesParameter,
                (p, v) =>
                {
                    iteration = v ?? iteration;
                });

            commandParser.Argument("n", "name", "Assigned To Name", "str_assignedToName",
                CommandArgumentFlags.TakesParameter | CommandArgumentFlags.Required,
                (p, v) =>
                {
                    name = v;
                });

            commandParser.Argument("u", "uri", "TFS uri. default is https://iiaastfs.ww004.siemens.net:443/tfs/tia",
                "tfs_url",
                CommandArgumentFlags.TakesParameter,
                (p, v) =>
                {
                    tfsUri = v ?? tfsUri;
                });


            commandParser.Argument("p", "prefix", "Title Prefix", "str_prefix",
                CommandArgumentFlags.TakesParameter,
                (p, v) =>
                {
                    titlePrefix = v;
                });

            commandParser.Argument("s", "suffix", "Suffix of Ext Id Suffix", "str_suffix",
                CommandArgumentFlags.TakesParameter,
                (p, v) =>
                {
                    externalIdSuffix = v;
                });




            commandParser.Argument("l", "link", "Link to ID",
                (p, v) =>
                {
                    linktoid = true;
                });


            commandParser.Argument("?", "help", "Shows the help screen.",
                            CommandArgumentFlags.HideInUsage,
                            (p, v) =>
                            {
                                showHelp = true;
                            });

            commandParser.Parse();

            if (commandParser.UnknownCommands.Count > 0)
            {
                foreach (var unknown in commandParser.UnknownCommands)
                {
                    Console.WriteLine("Invalid command: " + unknown);
                }

                Console.WriteLine(commandParser.GetHelp());
            }
            else if (commandParser.MissingRequiredCommands.Count > 0)
            {
                foreach (var missing in commandParser.MissingRequiredCommands)
                {
                    Console.WriteLine("ERROR: Missing argument: " + missing);
                }

                Console.WriteLine(commandParser.GetHelp());
            }
            else if (!showHelp)
            {
                //Console.WriteLine($"TFS uri: {tfsUri}");
                //Console.WriteLine($"WI Type: {wiType}");
            }
            else
            {
                Console.WriteLine(commandParser.GetHelp());
            }
        }
    }
}
