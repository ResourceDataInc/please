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
            var list = new List<TimestampFile>();
            var files = Directory.GetFiles(In.Directory);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var directoryName = Path.GetDirectoryName(file);
                var isTimestamped = Regex.IsMatch(fileName, timestampPattern, RegexOptions.None);

                var timestampFile = new TimestampFile
                {
                    FileName = fileName,
                    FileDirectory = directoryName,
                    IsTimestamped = isTimestamped
                };

                list.Add(timestampFile);
            }

            Out.Files = list.ToArray();
        }
    }
}
