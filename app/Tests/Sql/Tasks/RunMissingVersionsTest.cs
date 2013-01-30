using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Sql;
using Library.Sql.Tasks;
using NUnit.Framework;
using Simpler;
using Version = Library.Sql.Version;

namespace Tests.Sql.Tasks
{
    [TestFixture]
    public class RunMissingVersionsTest
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
        public void should_run_sql_scripts_if_no_versions_exist()
        {
            // Arrange
            var runMissingVersions = Task.New<RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = new Version[0];
            runMissingVersions.In.SqlScripts = _testSqlScripts;
            runMissingVersions.RunSqlScripts = Fake.Task<RunSqlScripts>();
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Assert.That(runMissingVersions.RunSqlScripts.Stats.ExecuteCount, Is.GreaterThan(0));
        }

        [Test]
        public void should_not_run_any_sql_scripts_if_all_versions_exist()
        {
            // Arrange
            var runMissingVersions = Task.New<RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = _testVersions;
            runMissingVersions.In.SqlScripts = _testSqlScripts;
            runMissingVersions.RunSqlScripts = Fake.Task<RunSqlScripts>();
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Assert.That(runMissingVersions.RunSqlScripts.Stats.ExecuteCount, Is.EqualTo(0));
        }

        [Test]
        public void should_run_all_sql_scripts_for_a_missing_version()
        {
            // Arrange
            var versions = new List<string>();
            var runMissingVersions = Task.New<RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = _testVersions.Where(version => version.Id != "002").ToArray();
            runMissingVersions.In.SqlScripts = _testSqlScripts;
            runMissingVersions.RunSqlScripts = Fake.Task<RunSqlScripts>(
                task => versions.AddRange(task.In.SqlScripts.Select(sqlScript => sqlScript.VersionId)));
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Assert.That(runMissingVersions.RunSqlScripts.Stats.ExecuteCount, Is.GreaterThan(0));
            Assert.That(versions[0], Is.EqualTo("002"));
            Assert.That(versions[1], Is.EqualTo("002"));
        }

        [Test]
        public void should_insert_version_for_each_distinct_version_belonging_to_sql_scripts_ran()
        {
            // Arrange
            var versions = new List<string>();
            var runMissingVersions = Task.New<RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = _testVersions.Where(version => version.Id != "002").ToArray();
            runMissingVersions.In.SqlScripts = _testSqlScripts;
            runMissingVersions.RunSqlScripts = Fake.Task<RunSqlScripts>(
                task => versions.AddRange(task.In.SqlScripts.Select(sqlScript => sqlScript.VersionId)));
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Assert.That(runMissingVersions.InsertInstalledVersion.Stats.ExecuteCount, Is.EqualTo(1));
        }

        [Test]
        public void should_run_sql_scripts_in_alphabetical_order()
        {
            // Arrange
            var runOrder = new List<string>();
            var runMissingVersions = Task.New<RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = new Version[0];
            runMissingVersions.In.SqlScripts = _testSqlScripts.Reverse().ToArray();
            runMissingVersions.RunSqlScripts = Fake.Task<RunSqlScripts>(
                task => runOrder.AddRange(task.In.SqlScripts.Select(sqlScript => sqlScript.FileName)));
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Assert.That(runOrder[0], Is.EqualTo(_testSqlScripts[0].FileName));
            Assert.That(runOrder[1], Is.EqualTo(_testSqlScripts[1].FileName));
            Assert.That(runOrder[2], Is.EqualTo(_testSqlScripts[2].FileName));
            Assert.That(runOrder[3], Is.EqualTo(_testSqlScripts[3].FileName));
        }
    }
}
