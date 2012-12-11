using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Library.Migrate.Model;
using Simpler;

namespace Library.Migrate.Tasks
{
    public class GetMigrations : InOutTask<GetMigrations.Input, GetMigrations.Output>
    {
        public class Input
        {
            public string Directory { get; set; }
        }

        public class Output 
        {
            public Migration[] Migrations { get; set; }
        }

        public override void Execute()
        {
            const string pattern = @"^(?<Version>\d+)[_]";
            var migrations = new List<Migration>();
            var fileNames = Directory.GetFiles(In.Directory);

            foreach (var fileName in fileNames)
            {
                var fileNameOnly = Path.GetFileName(fileName);
                var match = Regex.Match(fileNameOnly, pattern);
                Check.That(match.Groups.Count == 2,
                    "Expected to find version number at the beginning of file {0}.", fileNameOnly);

                migrations.Add(new Migration
                                   {
                                       FileName = fileNameOnly,
                                       FileNameWithPath = fileName,
                                       VersionNumber = match.Groups[1].Value
                                   });
            }

            Out.Migrations = migrations.ToArray();
        }
    }
}
