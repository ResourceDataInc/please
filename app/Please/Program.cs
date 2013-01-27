using Simpler;

namespace Please
{
    public class Program
    {
        static void Main(string[] args)
        {
            var please = Task.New<Library.Please>();
            please.In.Args = args;
            please.Execute();
        }
    }
}
