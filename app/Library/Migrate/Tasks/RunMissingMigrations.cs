using System.Linq;
using Library.Migrate.Model;
using Simpler;

namespace Library.Migrate.Tasks
{
    public class RunMissingMigrations : InTask<RunMissingMigrations.Input>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public Version[] InstalledVersions { get; set; }
            public Migration[] Migrations { get; set; }
        }

        public RunMigration RunMigration { get; set; }

        public override void Execute()
        {
            foreach (var migrationVersion in In.Migrations
                .OrderBy(m => m.VersionNumber)
                .Select(m => m.VersionNumber).Distinct())
            {
                if (In.InstalledVersions.All(installed => installed.Number != migrationVersion))
                {
                    foreach (var migration in In.Migrations
                        .Where(m => m.VersionNumber == migrationVersion)
                        .OrderBy(m => m.FileName))
                    {
                        RunMigration.In.ConnectionName = In.ConnectionName;
                        RunMigration.In.Migration = migration;
                        RunMigration.Execute();
                    }
                }
            }
        }
    }
}
