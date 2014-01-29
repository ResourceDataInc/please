using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Simpler;
using System.Linq;

namespace Library.Scripts.Tasks
{
    public class GetScripts : InOutTask<GetScripts.Input, GetScripts.Output>
    {
        public class Input
        {
            public string Directory { get; set; }
            public string[] Extensions { get; set; }
            public bool CheckForVersionedFilesOnly { get; set; }
            public string WhitelistFile { get; set; }
        }

        public class Output
        {
            public Script[] Scripts { get; set; }
        }

        public override void Execute()
        {
            const string versionedPattern = @"^(?<Version>\d+)[_]";
            var scripts = new List<Script>();
            var fileNames = Directory.GetFiles(In.Directory);

            var checkWhitelist = false;
            var whitelist = "";
            if (!String.IsNullOrEmpty(In.WhitelistFile))
            {
                checkWhitelist = true;
                whitelist = File.ReadAllText(In.WhitelistFile);
                whitelist = whitelist.Replace(@"/", @"\");
            }

            foreach (var fileName in fileNames)
            {
                var fileNameWithoutPath = Path.GetFileName(fileName);
                if (fileNameWithoutPath == null) throw new RunException(String.Format("{0} is not a file.", fileName));

                var fileQualifies = In.Extensions.Contains(Path.GetExtension(fileNameWithoutPath));

                if (fileQualifies && checkWhitelist)
                {
                    var fileSearch = new Regex(String.Format(@"(^|\\){0}\r?$", Regex.Escape(fileNameWithoutPath)),
                                               RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    fileQualifies = fileSearch.IsMatch(whitelist);
                }

                if (fileQualifies)
                {
                    var match = Regex.Match(fileNameWithoutPath, versionedPattern);
                    var fileIsVersioned = match.Groups.Count == 2;
                    var script = new Script
                    {
                        FileName = fileName,
                        IsVersioned = fileIsVersioned,
                    };

                    if (script.IsVersioned) script.VersionId = match.Groups[1].Value;
                    Check.That(!In.CheckForVersionedFilesOnly || (In.CheckForVersionedFilesOnly && script.IsVersioned),
                               "Expected to find version number at the beginning of file {0}.", fileNameWithoutPath);

                    scripts.Add(script);
                }
            }

            Out.Scripts = scripts.ToArray();
        }
    }
}
