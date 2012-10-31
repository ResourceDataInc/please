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

            foreach (var nuspecFile in parse.Out.NuspecFiles)
            {
                bumpNuspec.In.FileName = nuspecFile;
                bumpNuspec.Execute();
            }
        }
    }
}
