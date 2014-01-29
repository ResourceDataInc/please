using System;
using System.Linq;
using Simpler;

namespace Library.Scripts.Tasks
{
    public class RunMissingVersions : InTask<RunMissingVersions.Input>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public Version[] InstalledVersions { get; set; }
            public Script[] Scripts { get; set; }
        }

        public RunScripts RunScripts { get; set; }
        public InsertInstalledVersion InsertInstalledVersion { get; set; }

        public override void Execute()
        {
            var allVersionIds = In.Scripts
                .OrderBy(m => m.VersionId)
                .Select(m => m.VersionId).Distinct();

            foreach (var versionId in allVersionIds)
            {
                if (In.InstalledVersions.All(installed => installed.Id != versionId))
                {
                    var missingVersionId = versionId;
                    Console.WriteLine("{0} not installed - running scripts.", missingVersionId);

                    var sqlScriptsForMissingVersion = In.Scripts
                        .Where(m => m.VersionId == missingVersionId)
                        .OrderBy(m => m.FileName);

                    Console.WriteLine("{0} scripts were found for version {1}.",
                                      sqlScriptsForMissingVersion.Count(),
                                      missingVersionId);
                    RunScripts.In.ConnectionName = In.ConnectionName;
                    RunScripts.In.Scripts = sqlScriptsForMissingVersion.ToArray();
                    RunScripts.Execute();

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
