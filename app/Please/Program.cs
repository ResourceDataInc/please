using Library.Please.Tasks;
using Simpler;

namespace Please
{
    public class Program
    {
        static void Main(string[] args)
        {
            var runCommand = Task.New<RunCommand>();
            runCommand.In.Args = args;
            runCommand.Execute();
        }
    }
}
