using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Migrate;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;
using Version = Library.Migrate.Version;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class RunMissingVersions
    {
        readonly Version[] _testVersions =
            new[]
                {
                    new Version {Id = "001"},
                    new Version {Id = "002"},
                    new Version {Id = "003"}
                };

        readonly SqlScript[] _testSqlScripts =
            new[]
                {
                    new SqlScript {VersionId = "001", FileName = "001_first"},
                    new SqlScript {VersionId = "002", FileName = "002_second"},
                    new SqlScript {VersionId = "002", FileName = "002_third"},
                    new SqlScript {VersionId = "003", FileName = "003_fourth"}
                };

        [Test]
        public void should_run_all_migrations_if_no_versions_exist()
        {
            // Arrange
            var runMissingVersions = Task.New<Library.Migrate.Tasks.RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = new Version[0];
            runMissingVersions.In.SqlScripts = _testSqlScripts;
            runMissingVersions.RunSqlScript = Fake.Task<RunSqlScript>();
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Check.That(runMissingVersions.RunSqlScript.Stats.ExecuteCount == _testSqlScripts.Length,
                "Expected {0} migration to be ran.", _testSqlScripts.Length);
        }

        [Test]
        public void should_not_run_any_migrations_if_all_versions_exist()
        {
            // Arrange
            var runMissingVersions = Task.New<Library.Migrate.Tasks.RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = _testVersions;
            runMissingVersions.In.SqlScripts = _testSqlScripts;
            runMissingVersions.RunSqlScript = Fake.Task<RunSqlScript>();
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Check.That(runMissingVersions.RunSqlScript.Stats.ExecuteCount == 0,
                "Didn't expect any migration to be ran.");
        }

        [Test]
        public void should_run_all_migrations_for_a_missing_version()
        {
            // Arrange
            var versions = new List<string>();
            var runMissingVersions = Task.New<Library.Migrate.Tasks.RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = _testVersions.Where(version => version.Id != "002").ToArray();
            runMissingVersions.In.SqlScripts = _testSqlScripts;
            runMissingVersions.RunSqlScript = Fake.Task<RunSqlScript>(task => versions.Add(task.In.SqlScript.VersionId));
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Check.That(runMissingVersions.RunSqlScript.Stats.ExecuteCount == 2, "Expected {0} migration to be ran.", 2);
            Check.That(versions[0] == "002", "First migration should have been for version 002");
            Check.That(versions[1] == "002", "Second migration should have been for version 002");
        }

        [Test]
        public void should_insert_version_for_each_distinct_version_belonging_to_migrations_ran()
        {
            // Arrange
            var versions = new List<string>();
            var runMissingVersions = Task.New<Library.Migrate.Tasks.RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = _testVersions.Where(version => version.Id != "002").ToArray();
            runMissingVersions.In.SqlScripts = _testSqlScripts;
            runMissingVersions.RunSqlScript = Fake.Task<RunSqlScript>(task => versions.Add(task.In.SqlScript.VersionId));
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Check.That(runMissingVersions.RunSqlScript.Stats.ExecuteCount == 2, "Expected 2 migration to be ran.");
            Check.That(runMissingVersions.InsertInstalledVersion.Stats.ExecuteCount == 1, "Expected 1 version to be inserted.");
        }

        [Test]
        public void should_run_migrations_in_alphabetical_order()
        {
            // Arrange
            var versions = new List<string>();
            var runMissingVersions = Task.New<Library.Migrate.Tasks.RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = new Version[0];
            runMissingVersions.In.SqlScripts = _testSqlScripts.Reverse().ToArray();
            runMissingVersions.RunSqlScript = Fake.Task<RunSqlScript>(task => versions.Add(task.In.SqlScript.FileName));
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Check.That(versions[0] == _testSqlScripts[0].FileName, "{0} should have ran first.", _testSqlScripts[0].FileName);
            Check.That(versions[1] == _testSqlScripts[1].FileName, "{0} should have ran second.", _testSqlScripts[1].FileName);
            Check.That(versions[2] == _testSqlScripts[2].FileName, "{0} should have ran third.", _testSqlScripts[2].FileName);
            Check.That(versions[3] == _testSqlScripts[3].FileName, "{0} should have ran fourth.", _testSqlScripts[3].FileName);
        }

        [Test]
        public void should_throw_if_migration_fails()
        {
            // Arrange
            var runMissingVersions = Task.New<Library.Migrate.Tasks.RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = new Version[0];
            runMissingVersions.In.SqlScripts = _testSqlScripts.Take(1).ToArray();
            runMissingVersions.RunSqlScript = Fake.Task<RunSqlScript>(rm => { throw new Exception(); });
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act & Assert
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                Check.Throws<RunSqlException>(runMissingVersions.Execute);
            }
        }
    }
}
