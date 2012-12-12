using Simpler;

namespace Library.Migrate.Tasks
{
    public class MigrateDatabase : InTask<MigrateDatabase.Input>
    {
        public class Input
        {
            public string[] Args { get; set; }
        }

        public GetMigrationScripts GetMigrationScripts { get; set; }
        public FetchInstalledVersions FetchInstalledVersions { get; set; }
        public RunMissingMigrations RunMissingMigrations { get; set; }

        public override void Execute()
        {
            var connectionName = In.Args[0];
            var directory = In.Args[1];

            GetMigrationScripts.In.Directory = directory;
            GetMigrationScripts.Execute();

            FetchInstalledVersions.In.ConnectionName = connectionName;
            FetchInstalledVersions.Execute();
        }
    }
}
