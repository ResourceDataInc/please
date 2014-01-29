using System;
using System.IO;
using System.Linq;
using Library.Scripts;
using Library.Scripts.Tasks;
using NUnit.Framework;
using Simpler;
using Version = Library.Scripts.Version;

namespace Tests.Scripts.Tasks
{
    [TestFixture]
    public class RunTest
    {
        public static Version[] TestVersions =
            new[]
                {
                    new Version {Id = "001"},
                    new Version {Id = "002"},
                    new Version {Id = "003"}
                };

        public static Script[] TestScripts =
            new[]
                {
                    new Script {VersionId = "001", FileName = "001_first"},
                    new Script {VersionId = "002", FileName = "002_second"},
                    new Script {VersionId = "002", FileName = "002_third"},
                    new Script {VersionId = "003", FileName = "003_fourth"}
                };

        [Test]
        public void should_not_create_version_table_without_versioning()
        {
            // Arrange
            var run = Task.New<Run>();
            run.In.WithVersioning = false;
            run.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            run.CreateVersionTable = Fake.Task<CreateVersionTable>();
            run.GetScripts = Fake.Task<GetScripts>(gss => gss.Out.Scripts = new Script[0]);
            run.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            run.RunMissingVersions = Fake.Task<RunMissingVersions>();
            run.RunScripts = Fake.Task<RunScripts>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                run.Execute();
            }

            // Assert
            Assert.That(run.CheckForVersionTable.Stats.ExecuteCount, Is.EqualTo(0));
            Assert.That(run.CreateVersionTable.Stats.ExecuteCount, Is.EqualTo(0));
        }

        [Test]
        public void should_create_version_table_if_it_does_not_exist_with_versioning()
        {
            // Arrange
            var run = Task.New<Run>();
            run.In.WithVersioning = true;
            run.CheckForVersionTable = Fake.Task<CheckForVersionTable>(cfvt => cfvt.Out.TableExists = false);
            run.CreateVersionTable = Fake.Task<CreateVersionTable>();
            run.GetScripts = Fake.Task<GetScripts>();
            run.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            run.RunMissingVersions = Fake.Task<RunMissingVersions>();
            run.RunScripts = Fake.Task<RunScripts>();

            // Act
            run.Execute();

            // Assert
            Assert.That(run.CreateVersionTable.Stats.ExecuteCount, Is.EqualTo(1));
        }

        [Test]
        public void should_not_create_version_table_if_it_exists_with_versioning()
        {
            // Arrange
            var run = Task.New<Run>();
            run.In.WithVersioning = true;
            run.CheckForVersionTable = Fake.Task<CheckForVersionTable>(cfvt => cfvt.Out.TableExists = true);
            run.CreateVersionTable = Fake.Task<CreateVersionTable>();
            run.GetScripts = Fake.Task<GetScripts>();
            run.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            run.RunMissingVersions = Fake.Task<RunMissingVersions>();
            run.RunScripts = Fake.Task<RunScripts>();

            // Act
            run.Execute();

            // Assert
            Assert.That(run.CreateVersionTable.Stats.ExecuteCount, Is.EqualTo(0));
        }

        [Test]
        public void should_get_sql_scripts_using_from_directory_without_versioning()
        {
            // Arrange
            const string directoryArgument = "SomeDirectory";
            var passedDirectory = "";

            var run = Task.New<Run>();
            run.In.Directory = directoryArgument;
            run.In.WithVersioning = false;
            run.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            run.CreateVersionTable = Fake.Task<CreateVersionTable>();
            run.GetScripts =
                Fake.Task<GetScripts>(gss =>
                {
                    passedDirectory = gss.In.Directory;
                    gss.Out.Scripts = TestScripts;
                });
            run.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            run.RunMissingVersions = Fake.Task<RunMissingVersions>();
            run.RunScripts = Fake.Task<RunScripts>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                run.Execute();
            }

            // Assert
            Assert.That(run.GetScripts.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedDirectory, Is.EqualTo(directoryArgument));
        }

        [Test]
        public void should_get_sql_scripts_using_from_directory_with_versioning()
        {
            // Arrange
            const string directoryArgument = "SomeDirectory";
            var passedDirectory = "";

            var run = Task.New<Run>();
            run.In.Directory = directoryArgument;
            run.In.WithVersioning = true;
            run.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            run.CreateVersionTable = Fake.Task<CreateVersionTable>();
            run.GetScripts =
                Fake.Task<GetScripts>(gss => passedDirectory = gss.In.Directory);
            run.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            run.RunMissingVersions = Fake.Task<RunMissingVersions>();
            run.RunScripts = Fake.Task<RunScripts>();

            // Act
            run.Execute();

            // Assert
            Assert.That(run.GetScripts.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedDirectory, Is.EqualTo(directoryArgument));
        }

        [Test]
        public void should_fetch_installed_versions_using_given_connection_name_with_versioning()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var run = Task.New<Run>();
            run.In.ConnectionName = connectionNameArgument;
            run.In.WithVersioning = true;
            run.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            run.CreateVersionTable = Fake.Task<CreateVersionTable>();
            run.GetScripts = Fake.Task<GetScripts>();
            run.FetchInstalledVersions =
                Fake.Task<FetchInstalledVersions>(fiv => passedConnectionName = fiv.In.ConnectionName);
            run.RunMissingVersions = Fake.Task<RunMissingVersions>();
            run.RunScripts = Fake.Task<RunScripts>();

            // Act
            run.Execute();

            // Assert
            Assert.That(run.FetchInstalledVersions.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedConnectionName, Is.EqualTo(connectionNameArgument));
        }

        [Test]
        public void should_run_sql_specified_sql_file_without_versioning()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var run = Task.New<Run>();
            run.In.File = "001_first.sql";
            run.In.ConnectionName = connectionNameArgument;
            run.In.WithVersioning = false;
            run.RunScripts = Fake.Task<RunScripts>(rss => passedConnectionName = rss.In.ConnectionName);

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                run.Execute();
            }

            // Assert
            Assert.That(run.RunScripts.Stats.ExecuteCount, Is.GreaterThan(0));
            Assert.That(passedConnectionName, Is.EqualTo(connectionNameArgument));
        }

        [Test]
        public void should_run_sql_scripts_using_given_connection_name_without_versioning()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var run = Task.New<Run>();
            run.In.ConnectionName = connectionNameArgument;
            run.In.WithVersioning = false;
            run.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            run.CreateVersionTable = Fake.Task<CreateVersionTable>();
            run.GetScripts =
                Fake.Task<GetScripts>(gss => gss.Out.Scripts = TestScripts);
            run.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            run.RunMissingVersions = Fake.Task<RunMissingVersions>();
            run.RunScripts = Fake.Task<RunScripts>(rss => passedConnectionName = rss.In.ConnectionName);

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                run.Execute();
            }

            // Assert
            Assert.That(run.RunScripts.Stats.ExecuteCount, Is.GreaterThan(0));
            Assert.That(passedConnectionName, Is.EqualTo(connectionNameArgument));
        }

        [Test]
        public void should_run_missing_sql_scripts_using_given_connection_name_with_versioning()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var run = Task.New<Run>();
            run.In.ConnectionName = connectionNameArgument;
            run.In.WithVersioning = true;
            run.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            run.CreateVersionTable = Fake.Task<CreateVersionTable>();
            run.GetScripts = Fake.Task<GetScripts>();
            run.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            run.RunMissingVersions =
                Fake.Task<RunMissingVersions>(rmm => passedConnectionName = rmm.In.ConnectionName);
            run.RunScripts = Fake.Task<RunScripts>();

            // Act
            run.Execute();

            // Assert
            Assert.That(run.RunMissingVersions.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedConnectionName, Is.EqualTo(connectionNameArgument));
        }

        [Test]
        public void should_run_missing_sql_scripts_based_on_sql_scripts_and_versions_with_versioning()
        {
            // Arrange
            var scripts = TestScripts.Take(1).ToArray();
            var versions = TestVersions.Take(1).ToArray();
            var passedSqlScripts = new Script[0];
            var passedVersions = new Version[0];

            var run = Task.New<Run>();
            run.In.WithVersioning = true;
            run.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            run.CreateVersionTable = Fake.Task<CreateVersionTable>();
            run.GetScripts =
                Fake.Task<GetScripts>(gms => gms.Out.Scripts = scripts);
            run.FetchInstalledVersions = 
                Fake.Task<FetchInstalledVersions>(fiv => fiv.Out.Versions = versions);
            run.RunMissingVersions =
                Fake.Task<RunMissingVersions>(rmm =>
                                                    {
                                                        passedSqlScripts = rmm.In.Scripts;
                                                        passedVersions = rmm.In.InstalledVersions;
                                                    });
            run.RunScripts = Fake.Task<RunScripts>();

            // Act
            run.Execute();

            // Assert
            Assert.That(run.RunMissingVersions.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedSqlScripts, Is.EqualTo(scripts));
            Assert.That(passedVersions, Is.EqualTo(versions));
        }
    }
}
