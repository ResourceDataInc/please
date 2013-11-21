using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Sql.Tasks;
using NUnit.Framework;
using Simpler;
using Version = Library.Sql.Version;

namespace Tests.Sql.Tasks
{
    [TestFixture]
    public class RunMissingVersionsTest
    {
        [Test]
        public void should_run_sql_scripts_if_no_versions_exist()
        {
            // Arrange
            var runMissingVersions = Task.New<RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = new Version[0];
            runMissingVersions.In.Scripts = RunTest.TestScripts;
            runMissingVersions.RunScripts = Fake.Task<RunScripts>();
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Assert.That(runMissingVersions.RunScripts.Stats.ExecuteCount, Is.GreaterThan(0));
        }

        [Test]
        public void should_not_run_any_sql_scripts_if_all_versions_exist()
        {
            // Arrange
            var runMissingVersions = Task.New<RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = RunTest.TestVersions;
            runMissingVersions.In.Scripts = RunTest.TestScripts;
            runMissingVersions.RunScripts = Fake.Task<RunScripts>();
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Assert.That(runMissingVersions.RunScripts.Stats.ExecuteCount, Is.EqualTo(0));
        }

        [Test]
        public void should_run_all_sql_scripts_for_a_missing_version()
        {
            // Arrange
            var versions = new List<string>();
            var runMissingVersions = Task.New<RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = RunTest.TestVersions.Where(version => version.Id != "002").ToArray();
            runMissingVersions.In.Scripts = RunTest.TestScripts;
            runMissingVersions.RunScripts = Fake.Task<RunScripts>(
                task => versions.AddRange(task.In.Scripts.Select(script => script.VersionId)));
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Assert.That(runMissingVersions.RunScripts.Stats.ExecuteCount, Is.GreaterThan(0));
            Assert.That(versions[0], Is.EqualTo("002"));
            Assert.That(versions[1], Is.EqualTo("002"));
        }

        [Test]
        public void should_insert_version_for_each_distinct_version_belonging_to_sql_scripts_ran()
        {
            // Arrange
            var versions = new List<string>();
            var runMissingVersions = Task.New<RunMissingVersions>();
            runMissingVersions.In.InstalledVersions = RunTest.TestVersions.Where(version => version.Id != "002").ToArray();
            runMissingVersions.In.Scripts = RunTest.TestScripts;
            runMissingVersions.RunScripts = Fake.Task<RunScripts>(
                task => versions.AddRange(task.In.Scripts.Select(script => script.VersionId)));
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
            runMissingVersions.In.Scripts = RunTest.TestScripts.Reverse().ToArray();
            runMissingVersions.RunScripts = Fake.Task<RunScripts>(
                task => runOrder.AddRange(task.In.Scripts.Select(script => script.FileName)));
            runMissingVersions.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingVersions.Execute();
            }

            // Assert
            Assert.That(runOrder[0], Is.EqualTo(RunTest.TestScripts[0].FileName));
            Assert.That(runOrder[1], Is.EqualTo(RunTest.TestScripts[1].FileName));
            Assert.That(runOrder[2], Is.EqualTo(RunTest.TestScripts[2].FileName));
            Assert.That(runOrder[3], Is.EqualTo(RunTest.TestScripts[3].FileName));
        }
    }
}
