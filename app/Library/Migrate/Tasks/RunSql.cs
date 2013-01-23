using System;
using System.IO;
using Simpler;

namespace Library.Migrate.Tasks
{
    public class RunSql : InTask<RunSql.Input>
    {
        public class Input
        {
            public string[] Args { get; set; }
            public bool WithVersioning { get; set; }
        }

        public CheckForVersionTable CheckForVersionTable { get; set; }
        public CreateVersionTable CreateVersionTable { get; set; }
        public GetSqlScripts GetSqlScripts { get; set; }
        public FetchInstalledVersions FetchInstalledVersions { get; set; }
        public RunMissingVersions RunMissingVersions { get; set; }
        public RunSqlScript RunSqlScript { get; set; }

        public override void Execute()
        {
            var connectionName = In.Args[0];
            var directory = In.Args[1];

            if (In.WithVersioning)
            {
                CheckForVersionTable.In.ConnectionName = connectionName;
                CheckForVersionTable.Execute();

                if (!CheckForVersionTable.Out.TableExists)
                {
                    CreateVersionTable.In.ConnectionName = connectionName;
                    CreateVersionTable.Execute();
                }

                FetchInstalledVersions.In.ConnectionName = connectionName;
                FetchInstalledVersions.Execute();
            }

            GetSqlScripts.In.Directory = directory;
            GetSqlScripts.In.CheckForVersionedFilesOnly = In.WithVersioning;
            GetSqlScripts.Execute();

            if (In.WithVersioning)
            {
                RunMissingVersions.In.ConnectionName = connectionName;
                RunMissingVersions.In.SqlScripts = GetSqlScripts.Out.SqlScripts;
                RunMissingVersions.In.InstalledVersions = FetchInstalledVersions.Out.Versions;
                RunMissingVersions.Execute();
            }
            else
            {
                foreach (var sqlScript in GetSqlScripts.Out.SqlScripts)
                {
                    // TODO - this whole block is repeated in RunMissingVersions
                    var fileName = Path.GetFileName(sqlScript.FileName);
                    try
                    {
                        RunSqlScript.In.ConnectionName = connectionName;
                        RunSqlScript.In.SqlScript = sqlScript;
                        RunSqlScript.Execute();
                        Console.WriteLine("  {0} ran successfully.", fileName);
                    }
                    catch (Exception e)
                    {
                        throw new RunSqlException(String.Format("{0} failed.\n  Message: {1}", fileName, e.Message));
                    }
                }
            }
        }
    }
}
