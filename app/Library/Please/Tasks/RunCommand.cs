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
        public MigrateDatabase MigrateDatabase { get; set; }

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
                    MigrateDatabase.In.Args = In.Args.Skip(1).ToArray();
                    MigrateDatabase.Execute();
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
