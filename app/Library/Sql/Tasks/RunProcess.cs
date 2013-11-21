using System;
using System.Diagnostics;
using Simpler;

namespace Library.Sql.Tasks
{
    public class RunProcess : InOutTask<RunProcess.Input, RunProcess.Output>
    {
        public class Input
        {
            public string FileName { get; set; }
            public string Arguments { get; set; }
        }

        public class Output
        {
            public int ExitCode { get; set; }
        }

        public override void Execute()
        {
            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = In.FileName;
                process.StartInfo.Arguments = In.Arguments;
                process.Start();

                string message;
                while ((message = process.StandardOutput.ReadLine()) != null)
                {
                    Console.WriteLine(message);
                }

                string error;
                while ((error = process.StandardError.ReadLine()) != null)
                {
                    Console.WriteLine(error);
                }

                process.WaitForExit();

                Out.ExitCode = process.ExitCode;
            }
        }
    }
}
