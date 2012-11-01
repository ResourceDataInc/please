using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Simpler;

namespace Library.Tasks
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
            public string[] ScriptFiles { get; set; }
        }

        public override void Execute()
        {
            var bumpType = Enum.Parse(typeof(BumpType), In.Args[0], true);
            Out.BumpType = (BumpType) bumpType;

            var assemblyInfoRegEx = new Regex(@"AssemblyInfo\.cs$");
            var nuspecRegEx = new Regex(@"\.nuspec$");

            var assemblyInfoFiles = new List<string>();
            var nuspecFiles = new List<string>();
            var scriptFiles = new List<string>();

            for (var i = 1; i < In.Args.Length; i++)
            {
                var assemblyInfoMatch = assemblyInfoRegEx.Match(In.Args[i]);
                var nuspecMatch = nuspecRegEx.Match(In.Args[i]);
                if (assemblyInfoMatch.Success)
                {
                    assemblyInfoFiles.Add(In.Args[i]);
                }
                else if (nuspecMatch.Success)
                {
                    nuspecFiles.Add(In.Args[i]);
                }
                else
                {
                    scriptFiles.Add(In.Args[i]);
                }
            }

            Out.AssemblyInfoFiles = assemblyInfoFiles.ToArray();
            Out.NuspecFiles = nuspecFiles.ToArray();
            Out.ScriptFiles = scriptFiles.ToArray();
        }
    }
}
