using Simpler;

namespace Library.Releases.Tasks
{
    public class Bump : InTask<Bump.Input>
    {
        public class Input
        {
            public BumpType BumpType { get; set; }
            public FileType FileType { get; set; }
            public string FileName { get; set; }
        }

        public BumpNuspec BumpNuspec { get; set; }
        public BumpAssemblyInfo BumpAssemblyInfo { get; set; }
        public BumpScript BumpScript { get; set; }

        public override void Execute()
        {
            switch (In.FileType)
            {
                case FileType.AssemblyInfo:
                    BumpAssemblyInfo.In.BumpType = In.BumpType;
                    BumpAssemblyInfo.In.FileName = In.FileName;
                    BumpAssemblyInfo.Execute();
                    break;

                case FileType.Nuspec:
                    BumpNuspec.In.BumpType = In.BumpType;
                    BumpNuspec.In.FileName = In.FileName;
                    BumpNuspec.Execute();
                    break;

                case FileType.Script:
                    BumpScript.In.BumpType = In.BumpType;
                    BumpScript.In.FileName = In.FileName;
                    BumpScript.Execute();
                    break;
            }
        }
    }
}
