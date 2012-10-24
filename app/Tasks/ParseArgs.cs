using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Simpler;

namespace Tasks
{
    public class ParseArgs : InOutTask<ParseArgs.Input, ParseArgs.Output>
    {
        public class Input
        {
            public string[] Args { get; set; }
        }

        public class Output
        {
            public BumpType BumpType { get; set; }
            public string[] AssemblyInfoFiles { get; set; }
            public string[] NuspecFiles { get; set; }
        }

        public override void Execute()
        {
            var bumpType = Enum.Parse(typeof(BumpType), In.Args[0], true);
            Out.BumpType = (BumpType) bumpType;

            var assemblyInfoRegEx = new Regex(@"AssemblyInfo\.cs$");
            var assemblyInfoFiles = new List<string>();
            for (var i = 1; i < In.Args.Length; i++)
            {
                var match = assemblyInfoRegEx.Match(In.Args[i]);
                if (match.Success)
                {
                    assemblyInfoFiles.Add(In.Args[i]);
                }
            }
            Out.AssemblyInfoFiles = assemblyInfoFiles.ToArray();

            var nuspecRegEx = new Regex(@"\.nuspec$");
            var nuspecFiles = new List<string>();
            for (var i = 1; i < In.Args.Length; i++)
            {
                var match = nuspecRegEx.Match(In.Args[i]);
                if (match.Success)
                {
                    nuspecFiles.Add(In.Args[i]);
                }
            }
            Out.NuspecFiles = nuspecFiles.ToArray();
        }
    }
}
