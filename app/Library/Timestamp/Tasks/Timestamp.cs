using System;
using Simpler;
using System.IO;

namespace Library.Timestamp.Tasks
{
    public class Timestamp : InTask<Timestamp.Input>
    {
        public class Input
        {
            public string Directory { get; set; }
        }

        public GetFiles GetFiles { get; set; }

        public override void Execute()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss_");

            GetFiles.In = In.Directory;
            GetFiles.Execute();
            TimestampFile[] files = GetFiles.Out;

            foreach (var file in files)
            {
                if (file.IsTimestamped)
                    continue;

                string fileName = file.FileName;
                string directory = file.FileDirectory;

                File.Move(directory + fileName, directory + timestamp + fileName);
            }
        }
    }
}
