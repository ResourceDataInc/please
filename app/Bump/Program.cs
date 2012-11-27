using Simpler;

namespace Bump
{
    public class Program
    {
        static void Main(string[] args)
        {
            var bump = Task.New<Library.Tasks.Bump>();
            bump.In.Args = args;
            bump.Execute();
        }
    }
}
