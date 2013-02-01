using System;
using System.Linq;
using Simpler;

namespace Library
{
    public class Please : InOutTask<Please.Input, Please.Output>
    {
        public class Input
        {
            public string[] Args { get; set; }
        }

        public class Output
        {
            public ICommand Command { get; set; }
            public int ExitCode { get; set; }
        }

        public override void Execute()
        {
            try
            {
                var commandText = String.Join(" ", In.Args);
                foreach (var command in Commands.All)
                {
                    if (commandText.StartsWith(command.Name))
                    {
                        Out.Command = command;

                        var options = commandText.Substring(command.Name.Length);
                        command.Run(options);
                        Console.WriteLine("{0} completed in {1} seconds.",
                                          command.Name,
                                          command.Task.Stats.ExecuteDurations.Max(span => span.TotalSeconds));
                        break;
                    }
                }
                Out.ExitCode = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Oops, something went wrong. Error: {0}", e.Message);
                Out.ExitCode = 1;
            }

            if (Out.Command == null)
            {
                Console.WriteLine("Didn't understand the given command.");
                Out.ExitCode = 1;
            }
        }
    }
}
