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
            public bool CheckForVersionedFilesOnly { get; set; }
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
                var fileIsVersioned = match.Groups.Count == 2;
                var script = new SqlScript
                                 {
                                     FileName = fileName,
                                     IsVersioned = fileIsVersioned,                   
                                 };

                if (script.IsVersioned) script.VersionId = match.Groups[1].Value;
                Check.That(!In.CheckForVersionedFilesOnly || (In.CheckForVersionedFilesOnly && script.IsVersioned),
                           "Expected to find version number at the beginning of file {0}.", fileNameWithoutPath);

                sqlScripts.Add(script);
            }

            Out.SqlScripts = sqlScripts.ToArray();
        }
    }
}
