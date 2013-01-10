using Simpler;

namespace Library.Migrate.Tasks
{
    public class RunSql : InTask<RunSql.Input>
    {
        public class Input
        {
            public string[] Args { get; set; }
        }

        public CheckForVersionTable CheckForVersionTable { get; set; }
        public CreateVersionTable CreateVersionTable { get; set; }
        public GetSqlScripts GetSqlScripts { get; set; }
        public FetchInstalledVersions FetchInstalledVersions { get; set; }
        public RunMissingVersions RunMissingVersions { get; set; }

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

            GetSqlScripts.In.Directory = directory;
            GetSqlScripts.Execute();

            FetchInstalledVersions.In.ConnectionName = connectionName;
            FetchInstalledVersions.Execute();

            RunMissingVersions.In.ConnectionName = connectionName;
            RunMissingVersions.In.SqlScripts = GetSqlScripts.Out.SqlScripts;
            RunMissingVersions.In.InstalledVersions = FetchInstalledVersions.Out.Versions;
            RunMissingVersions.Execute();
        }
    }
}
