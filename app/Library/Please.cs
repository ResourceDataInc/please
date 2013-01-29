using System;
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
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Oops, something went wrong. Error: {0}", e.Message);
            }

            if (Out.Command == null)
            {
                Console.WriteLine("Didn't understand the given command.");
            }
        }
    }
}
