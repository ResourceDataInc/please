using System.IO;
using System.Linq;
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
            var fileNames = Directory.GetFiles(In.Directory);

            Out.Migrations = fileNames.Select(
                fileName => new Migration
                                {
                                    FileName = Path.GetFileName(fileName),
                                    FileNameWithPath = fileName
                                })
                .ToArray();
        }
    }
}
