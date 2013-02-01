using Simpler;

namespace Please
{
    public class Program
    {
        static int Main(string[] args)
        {
            var please = Task.New<Library.Please>();
            please.In.Args = args;
            please.Execute();
            return please.Out.ExitCode;
        }
    }
}
