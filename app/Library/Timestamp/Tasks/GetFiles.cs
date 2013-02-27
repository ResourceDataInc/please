using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Simpler;

namespace Library.Timestamp.Tasks
{
    public class GetFiles : InOutTask<string, TimestampFile[]>
    {
        const string timestampPattern = "^\\d{14}_";

        public override void Execute()
        {    
            var files = new List<TimestampFile>();
            var fileNames = Directory.GetFiles(In);

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

            Out = files.ToArray();
        }
    }
}
