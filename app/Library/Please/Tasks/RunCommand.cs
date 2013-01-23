using System;
using System.Linq;
using Library.Bump.Tasks;
using Library.Migrate.Tasks;
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
        public RunSql RunSql { get; set; }

        public override void Execute()
        {
            try
            {
                if (In.Args[0] == "bump")
                {
                    BumpFiles.In.Args = In.Args.Skip(1).ToArray();
                    BumpFiles.Execute();
                }
                else if (In.Args[0] == "migrate")
                {
                    RunSql.In.Args = In.Args.Skip(1).ToArray();
                    // TODO - assume true for now
                    RunSql.In.WithVersioning = true;
                    RunSql.Execute();
                }
                else
                {
                    Console.WriteLine("What?");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Sorry, but something went wrong."));
                Console.WriteLine(e.Message);
            }
        }
    }
}
