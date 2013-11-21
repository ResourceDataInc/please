using System;
using Simpler;

namespace Library.Sql.Tasks
{
    public class RunSql : InTask<RunSql.Input>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public string Directory { get; set; }
            public string[] Extensions { get; set; }
            public string File { get; set; }
            public bool WithVersioning { get; set; }
            public string WhitelistFile { get; set; }
        }

        public CheckForVersionTable CheckForVersionTable { get; set; }
        public CreateVersionTable CreateVersionTable { get; set; }
        public GetScripts GetScripts { get; set; }
        public FetchInstalledVersions FetchInstalledVersions { get; set; }
        public RunMissingVersions RunMissingVersions { get; set; }
        public RunSqlScripts RunSqlScripts { get; set; }

        public override void Execute()
        {
            if (In.File != null)
            {
                Console.WriteLine("{0} script was found.", In.File);
                RunSqlScripts.In.ConnectionName = In.ConnectionName;
                RunSqlScripts.In.SqlScripts = new[] { new SqlScript { FileName = In.File, IsVersioned = false} };
                RunSqlScripts.Execute();
                return;
            }
            
            if (In.WithVersioning)
            {
                CheckForVersionTable.In.ConnectionName = In.ConnectionName;
                CheckForVersionTable.Execute();

                if (!CheckForVersionTable.Out.TableExists)
                {
                    CreateVersionTable.In.ConnectionName = In.ConnectionName;
                    CreateVersionTable.Execute();
                }

                FetchInstalledVersions.In.ConnectionName = In.ConnectionName;
                FetchInstalledVersions.Execute();
            }


            GetScripts.In.Directory = In.Directory;
            GetScripts.In.Extensions = In.Extensions;
            GetScripts.In.CheckForVersionedFilesOnly = In.WithVersioning;
            GetScripts.In.WhitelistFile = In.WhitelistFile;
            GetScripts.Execute();

            if (In.WithVersioning)
            {
                RunMissingVersions.In.ConnectionName = In.ConnectionName;
                RunMissingVersions.In.SqlScripts = GetScripts.Out.SqlScripts;
                RunMissingVersions.In.InstalledVersions = FetchInstalledVersions.Out.Versions;
                RunMissingVersions.Execute();
            }
            else
            {
                Console.WriteLine("{0} scripts were found in {1}.", GetScripts.Out.SqlScripts.Length, In.Directory);
                RunSqlScripts.In.ConnectionName = In.ConnectionName;
                RunSqlScripts.In.SqlScripts = GetScripts.Out.SqlScripts;
                RunSqlScripts.Execute();
            }
        }
    }
}
