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
            public string ReleaseId { get; set; }
            public BumpType BumpType { get; set; }
            public string[] AssemblyInfoFiles { get; set; }
            public string[] NuspecFiles { get; set; }
            public string[] OtherFiles { get; set; }
        }

        public override void Execute()
        {
            Out.ReleaseId = In.Args[0];

            var bumpType = Enum.Parse(typeof(BumpType), In.Args[1], true);
            Out.BumpType = (BumpType) bumpType;

            var assemblyInfoRegEx = new Regex(@"AssemblyInfo\.cs$");
            var nuspecRegEx = new Regex(@"\.nuspec$");
            var assemblyInfoFiles = new List<string>();
            var nuspecFiles = new List<string>();
            var otherFiles = new List<string>();
            for (var i = 2; i < In.Args.Length; i++)
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
                    otherFiles.Add(In.Args[i]);
                }
            }
            Out.AssemblyInfoFiles = assemblyInfoFiles.ToArray();
            Out.NuspecFiles = nuspecFiles.ToArray();
            Out.OtherFiles = otherFiles.ToArray();
        }
    }
}
