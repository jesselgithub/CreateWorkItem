using System;

namespace CreateWorkItem
{
    public class ParserHarness {
        public static void Ma1in(string[] argList)
        {
            var parser = new CommandParser("A Sample Test Harness for CommandParser.cs");
            
            bool update = false;
            bool populate = false;
            string host = String.Empty;
            string query = String.Empty;
            bool showHelp = false;

            parser.Argument("u", "uri", "TFS URI stuff", 
                            (p, v) => {
                                update = true;
                            });

            parser.Argument("p", "populate", "Populates stuff with an argument",
                            (p, v) => {
                                populate = true;
                            });

            parser.Argument("h", "host", "Specify a host ip address", "ip_addr",
                            CommandArgumentFlags.TakesParameter |
                            CommandArgumentFlags.Required,
                            (p, v) => {
                                host = v;
                            });

            parser.Argument("q", "query", "Not really a query, fooled you!", "query_str",
                            CommandArgumentFlags.TakesParameter,
                            (p, v) => {
                                query = v;
                            });

            parser.Argument("?", "help", "Shows the help screen.",
                            CommandArgumentFlags.HideInUsage,
                            (p, v) => {
                                showHelp = true;
                            });
                            
            parser.Parse();

            if (parser.UnknownCommands.Count > 0) {
                foreach (var unknown in parser.UnknownCommands) {
                    Console.WriteLine("Invalid command: " + unknown);
                }

                Console.WriteLine(parser.GetHelp());
            } else if (parser.MissingRequiredCommands.Count > 0) {
                foreach (var missing in parser.MissingRequiredCommands) {
                    Console.WriteLine("ERROR: Missing argument: " + missing);
                }

                Console.WriteLine(parser.GetHelp());
            } else if (!showHelp) {
                Console.WriteLine("update = " + update);
                Console.WriteLine("populate = " + populate);
                Console.WriteLine("host = " + host);
                Console.WriteLine("query = " + query);
            } else {
                Console.WriteLine(parser.GetHelp());
            }
        }
    }
}