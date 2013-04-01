using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Simpler;

namespace Library.Timestamp.Tasks
{
    public class GetFiles : InOutTask<GetFiles.Input, GetFiles.Output>
    {
        const string timestampPattern = "^\\d{14}_";

        public class Input
        {
            public string Directory;
        }

        public class Output
        {
            public TimestampFile[] Files;
        }

        public override void Execute()
        {    
            var files = new List<TimestampFile>();
            var fileNames = Directory.GetFiles(In.Directory);

            foreach (var fileName in fileNames)
            {
                string fileNameWithoutPath = Path.GetFileName(fileName);
                var pathWithoutFileName = Path.GetDirectoryName(fileName) + "\\";

                TimestampFile file = new TimestampFile
                {
                    FileName = fileNameWithoutPath,
                    FileDirectory = pathWithoutFileName
                };

                file.IsTimestamped = Regex.IsMatch(fileNameWithoutPath, timestampPattern, RegexOptions.None);

                files.Add(file);
            }

            Out.Files = files.ToArray();
        }
    }
}
