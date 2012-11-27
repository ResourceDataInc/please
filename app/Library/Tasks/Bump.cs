using Simpler;

namespace Library.Tasks
{
    public class Bump : InTask<Bump.Input>
    {
        public class Input
        {
            public string[] Args { get; set; }
        }

        public ParseArgs ParseArgs { get; set; }
        public BumpNuspec BumpNuspec { get; set; }
        public BumpAssemblyInfo BumpAssemblyInfo { get; set; }
        public BumpScript BumpScript { get; set; }

        public override void Execute()
        {
            ParseArgs.In.Args = In.Args;
            ParseArgs.Execute();

            BumpNuspec.In.BumpType = ParseArgs.Out.BumpType;
            foreach (var file in ParseArgs.Out.NuspecFiles)
            {
                BumpNuspec.In.FileName = file;
                BumpNuspec.Execute();
            }

            BumpAssemblyInfo.In.BumpType = ParseArgs.Out.BumpType;
            foreach (var file in ParseArgs.Out.AssemblyInfoFiles)
            {
                BumpAssemblyInfo.In.FileName = file;
                BumpAssemblyInfo.Execute();
            }

            BumpScript.In.BumpType = ParseArgs.Out.BumpType;
            foreach (var file in ParseArgs.Out.ScriptFiles)
            {
                BumpScript.In.FileName = file;
                BumpScript.Execute();
            }
        }
    }
}
