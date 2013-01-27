using System;
using System.IO;
using Simpler;

namespace Library.Sql.Tasks
{
    public class RunSql : InTask<RunSql.Input>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public string Directory { get; set; }
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

            GetSqlScripts.In.Directory = In.Directory;
            GetSqlScripts.In.CheckForVersionedFilesOnly = In.WithVersioning;
            GetSqlScripts.Execute();

            if (In.WithVersioning)
            {
                RunMissingVersions.In.ConnectionName = In.ConnectionName;
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
                        RunSqlScript.In.ConnectionName = In.ConnectionName;
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
