using Simpler;
using Tasks;

namespace Bump
{
    public class Program
    {
        static void Main(string[] args)
        {
            var parse = Task.New<ParseArgs>();
            parse.In.Args = args;
            parse.Execute();

            var bumpNuspec = Task.New<BumpNuspec>();
            bumpNuspec.In.BumpType = parse.Out.BumpType;
            foreach (var file in parse.Out.NuspecFiles)
            {
                bumpNuspec.In.FileName = file;
                bumpNuspec.Execute();
            }

            var bumpAssemblyInfo = Task.New<BumpAssemblyInfo>();
            bumpAssemblyInfo.In.BumpType = parse.Out.BumpType;
            foreach (var file in parse.Out.AssemblyInfoFiles)
            {
                bumpAssemblyInfo.In.FileName = file;
                bumpAssemblyInfo.Execute();
            }

            var bumpScript = Task.New<BumpScript>();
            bumpScript.In.BumpType = parse.Out.BumpType;
            foreach (var file in parse.Out.ScriptFiles)
            {
                bumpScript.In.FileName = file;
                bumpScript.Execute();
            }
        }
    }
}
