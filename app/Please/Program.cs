using System;
using Library.Models;

namespace Please
{
    public class Program
    {
        static void Main(string[] args)
        {
            var commandText = String.Join(" ", args);
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
