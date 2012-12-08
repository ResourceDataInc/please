using Library.Bump.Tasks;
using Simpler;

namespace Bump
{
    public class Program
    {
        static void Main(string[] args)
        {
            var bumpFiles = Task.New<BumpFiles>();
            bumpFiles.In.Args = args;
            bumpFiles.Execute();
        }
    }
}
