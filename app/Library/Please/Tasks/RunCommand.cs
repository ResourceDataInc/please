using System;
using System.Linq;
using Library.Bump.Tasks;
using Simpler;

namespace Library.Please.Tasks
{
    public class RunCommand : InTask<RunCommand.Input>
    {
        public class Input
        {
            public string[] Args { get; set; }
        }

        public BumpFiles BumpFiles { get; set; }

        public override void Execute()
        {
            if (In.Args[0] == "bump")
            {
                BumpFiles.In.Args = In.Args.Skip(1).ToArray();
                BumpFiles.Execute();
            }
            else if (In.Args[0] == "migrate")
            {
                // TODO
                Console.WriteLine("Not Implemented.");
            }
            else
            {
                Console.WriteLine("What?");
            }
        }
    }
}
