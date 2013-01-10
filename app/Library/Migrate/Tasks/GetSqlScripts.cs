using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Simpler;

namespace Library.Migrate.Tasks
{
    public class GetSqlScripts : InOutTask<GetSqlScripts.Input, GetSqlScripts.Output>
    {
        public class Input
        {
            public string Directory { get; set; }
        }

        public class Output 
        {
            public SqlScript[] SqlScripts { get; set; }
        }

        public override void Execute()
        {
            const string pattern = @"^(?<Version>\d+)[_]";
            var sqlScripts = new List<SqlScript>();
            var fileNames = Directory.GetFiles(In.Directory);

            foreach (var fileName in fileNames)
            {
                var fileNameWithoutPath = Path.GetFileName(fileName);
                var match = Regex.Match(fileNameWithoutPath, pattern);
                Check.That(match.Groups.Count == 2,
                    "Expected to find version number at the beginning of file {0}.", fileNameWithoutPath);

                sqlScripts.Add(new SqlScript
                                   {
                                       FileName = fileName,
                                       VersionId = match.Groups[1].Value
                                   });
            }

            Out.SqlScripts = sqlScripts.ToArray();
        }
    }
}
