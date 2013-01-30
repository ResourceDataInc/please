using System;
using System.Linq;
using Simpler;

namespace Library.Sql.Tasks
{
    public class RunMissingVersions : InTask<RunMissingVersions.Input>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public Version[] InstalledVersions { get; set; }
            public SqlScript[] SqlScripts { get; set; }
        }

        public RunSqlScripts RunSqlScripts { get; set; }
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
                    Console.WriteLine("{0} not installed - running sql scripts.", missingVersionId);

                    var sqlScriptsForMissingVersion = In.SqlScripts
                        .Where(m => m.VersionId == missingVersionId)
                        .OrderBy(m => m.FileName);

                    Console.WriteLine("{0} scripts were found for version {1}.",
                                      sqlScriptsForMissingVersion.Count(),
                                      missingVersionId);
                    RunSqlScripts.In.ConnectionName = In.ConnectionName;
                    RunSqlScripts.In.SqlScripts = sqlScriptsForMissingVersion.ToArray();
                    RunSqlScripts.Execute();

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
