using System;
using System.IO;
using System.Text.RegularExpressions;
using Simpler;

namespace Library.Tasks
{
    public class BumpNuspec : InTask<BumpNuspec.Input>
    {
        public class Input
        {
            public string FileName { get; set; }
            public BumpType BumpType { get; set; }
        }

        public override void Execute()
        {
            var fileContents = File.ReadAllText(In.FileName);
            const string pattern = @"<version>(?<Major>\d+)[.](?<Minor>\d+)[.](?<Patch>\d+)</version>";

            var matches = Regex.Matches(fileContents, pattern);
            Check.That(matches.Count == 1, "Expected to find one version element in the {0} file.", In.FileName);
            Check.That(matches[0].Groups.Count == 4, "Expected to find a version in the format of X.X.X in {0} file.", In.FileName);

            var major = Int32.Parse(matches[0].Groups[1].Value);
            var minor = Int32.Parse(matches[0].Groups[2].Value);
            var patch = Int32.Parse(matches[0].Groups[3].Value);

            switch (In.BumpType)
            {
                case BumpType.Major:
                    major++;
                    minor = 0;
                    patch = 0;
                    break;
                case BumpType.Minor:
                    minor++;
                    patch = 0;
                    break;
                case BumpType.Patch:
                    patch++;
                    break;
            }

            var newVersion = String.Format(@"<version>{0}.{1}.{2}</version>", major, minor, patch);
            var newFileContents = Regex.Replace(fileContents, pattern, match => newVersion);
            File.WriteAllText(In.FileName, newFileContents);
        }
    }
}
