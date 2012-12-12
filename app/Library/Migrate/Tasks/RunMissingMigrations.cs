using System.Linq;
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
        public InsertInstalledVersion InsertInstalledVersion { get; set; }

        public override void Execute()
        {
            var allVersionIds = In.Migrations
                .OrderBy(m => m.VersionId)
                .Select(m => m.VersionId).Distinct();

            foreach (var versionId in allVersionIds)
            {
                if (In.InstalledVersions.All(installed => installed.Id != versionId))
                {
                    var missingVersionId = versionId;
                    var migrationsForMissingVersion = In.Migrations
                        .Where(m => m.VersionId == missingVersionId)
                        .OrderBy(m => m.FileName);

                    foreach (var migration in migrationsForMissingVersion)
                    {
                        RunMigration.In.ConnectionName = In.ConnectionName;
                        RunMigration.In.Migration = migration;
                        RunMigration.Execute();
                    }

                    InsertInstalledVersion.In.ConnectionName = In.ConnectionName;
                    InsertInstalledVersion.In.Version = new Version {Id = missingVersionId};
                    InsertInstalledVersion.Execute();
                }
            }
        }
    }
}
