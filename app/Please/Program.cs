using System;
using System.Linq;
using Library.Bump.Tasks;
using Simpler;

namespace Please
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "bump")
            {
                var bumpFiles = Task.New<BumpFiles>();
                bumpFiles.In.Args = args.Skip(1).ToArray();
                bumpFiles.Execute();
            }
            else if (args[0] == "migrate")
            {
                // TODO
                Console.WriteLine("Not Implemented.");
            }
            else
            {
                Console.WriteLine("What?");
            }
        }
    }
}
