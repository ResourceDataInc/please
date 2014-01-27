using System;
using System.Diagnostics;
using Simpler;

namespace Library.Scripts.Tasks
{
    public class RunProcess : InTask<RunProcess.Input>
    {
        public class Input
        {
            public string FileName { get; set; }
            public string Arguments { get; set; }
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
                process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new RunException(String.Format("{0} failed with arguments {1}.", In.FileName, In.Arguments));
                }
            }
        }
    }
}
