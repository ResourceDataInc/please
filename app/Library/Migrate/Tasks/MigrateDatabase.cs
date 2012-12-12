using Simpler;

namespace Library.Migrate.Tasks
{
    public class MigrateDatabase : InTask<MigrateDatabase.Input>
    {
        public class Input
        {
            public string[] Args { get; set; }
        }

        public CheckForVersionTable CheckForVersionTable { get; set; }
        public CreateVersionTable CreateVersionTable { get; set; }
        public GetMigrationScripts GetMigrationScripts { get; set; }
        public FetchInstalledVersions FetchInstalledVersions { get; set; }
        public RunMissingMigrations RunMissingMigrations { get; set; }

        public override void Execute()
        {
            var connectionName = In.Args[0];
            var directory = In.Args[1];

            CheckForVersionTable.In.ConnectionName = connectionName;
            CheckForVersionTable.Execute();

            if (!CheckForVersionTable.Out.TableExists)
            {
                CreateVersionTable.In.ConnectionName = connectionName;
                CreateVersionTable.Execute();
            }

            GetMigrationScripts.In.Directory = directory;
            GetMigrationScripts.Execute();

            FetchInstalledVersions.In.ConnectionName = connectionName;
            FetchInstalledVersions.Execute();

            RunMissingMigrations.In.ConnectionName = connectionName;
            RunMissingMigrations.In.Migrations = GetMigrationScripts.Out.Migrations;
            RunMissingMigrations.In.InstalledVersions = FetchInstalledVersions.Out.Versions;
            RunMissingMigrations.Execute();
        }
    }
}
