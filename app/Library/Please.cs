using System;
using Simpler;

namespace Library
{
    public class Please : InTask<Please.Input>
    {
        public class Input
        {
            public string[] Args { get; set; }
        }

        public override void Execute()
        {
            var commandText = String.Join(" ", In.Args);
            foreach (var command in Commands.All)
            {
                if (commandText.StartsWith(command.Name))
                {
                    var options = commandText.Substring(command.Name.Length);
                    command.Run(options);
                    break;
                }
            }
        }
    }
}
