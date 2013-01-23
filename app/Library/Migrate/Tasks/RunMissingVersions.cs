using System;
using System.IO;
using System.Linq;
using Simpler;

namespace Library.Migrate.Tasks
{
    public class RunMissingVersions : InTask<RunMissingVersions.Input>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public Version[] InstalledVersions { get; set; }
            public SqlScript[] SqlScripts { get; set; }
        }

        public RunSqlScript RunSqlScript { get; set; }
        public InsertInstalledVersion InsertInstalledVersion { get; set; }

        public override void Execute()
        {
            var allVersionIds = In.SqlScripts
                .OrderBy(m => m.VersionId)
                .Select(m => m.VersionId).Distinct();

            foreach (var versionId in allVersionIds)
            {
                if (In.InstalledVersions.All(installed => installed.Id != versionId))
                {
                    var missingVersionId = versionId;
                    Console.WriteLine("{0} not installed - running migrations.", missingVersionId);

                    var sqlScriptsForMissingVersion = In.SqlScripts
                        .Where(m => m.VersionId == missingVersionId)
                        .OrderBy(m => m.FileName);

                    foreach (var sqlScript in sqlScriptsForMissingVersion)
                    {
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

                    InsertInstalledVersion.In.ConnectionName = In.ConnectionName;
                    InsertInstalledVersion.In.Version = new Version {Id = missingVersionId};
                    InsertInstalledVersion.Execute();
                }
                else
                {
                    Console.WriteLine("{0} already installed.", versionId);
                }
            }
        }
    }
}
