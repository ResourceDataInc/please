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
        }
    }
}
