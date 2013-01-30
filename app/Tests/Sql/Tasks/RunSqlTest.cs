using System;
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
    public class RunSqlTest
    {
        // TODO - factor out to shared test data
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
        public void should_not_create_version_table_without_versioning()
        {
            // Arrange
            var runSql = Task.New<RunSql>();
            runSql.In.WithVersioning = false;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>(gss => gss.Out.SqlScripts = new SqlScript[0]);
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScript = Fake.Task<RunSqlScript>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runSql.Execute();
            }

            // Assert
            Assert.That(runSql.CheckForVersionTable.Stats.ExecuteCount, Is.EqualTo(0));
            Assert.That(runSql.CreateVersionTable.Stats.ExecuteCount, Is.EqualTo(0));
        }

        [Test]
        public void should_create_version_table_if_it_does_not_exist_with_versioning()
        {
            // Arrange
            var runSql = Task.New<RunSql>();
            runSql.In.WithVersioning = true;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>(cfvt => cfvt.Out.TableExists = false);
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>();
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScript = Fake.Task<RunSqlScript>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.CreateVersionTable.Stats.ExecuteCount, Is.EqualTo(1));
        }

        [Test]
        public void should_not_create_version_table_if_it_exists_with_versioning()
        {
            // Arrange
            var runSql = Task.New<RunSql>();
            runSql.In.WithVersioning = true;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>(cfvt => cfvt.Out.TableExists = true);
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>();
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScript = Fake.Task<RunSqlScript>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.CreateVersionTable.Stats.ExecuteCount, Is.EqualTo(0));
        }

        [Test]
        public void should_get_sql_scripts_using_from_directory_without_versioning()
        {
            // Arrange
            const string directoryArgument = "SomeDirectory";
            var passedDirectory = "";

            var runSql = Task.New<RunSql>();
            runSql.In.Directory = directoryArgument;
            runSql.In.WithVersioning = false;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts =
                Fake.Task<GetSqlScripts>(gss =>
                {
                    passedDirectory = gss.In.Directory;
                    gss.Out.SqlScripts = _testSqlScripts;
                });
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScript = Fake.Task<RunSqlScript>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runSql.Execute();
            }

            // Assert
            Assert.That(runSql.GetSqlScripts.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedDirectory, Is.EqualTo(directoryArgument));
        }

        [Test]
        public void should_get_sql_scripts_using_from_directory_with_versioning()
        {
            // Arrange
            const string directoryArgument = "SomeDirectory";
            var passedDirectory = "";

            var runSql = Task.New<RunSql>();
            runSql.In.Directory = directoryArgument;
            runSql.In.WithVersioning = true;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts =
                Fake.Task<GetSqlScripts>(gss => passedDirectory = gss.In.Directory);
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScript = Fake.Task<RunSqlScript>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.GetSqlScripts.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedDirectory, Is.EqualTo(directoryArgument));
        }

        [Test]
        public void should_fetch_installed_versions_using_given_connection_name_with_versioning()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var runSql = Task.New<RunSql>();
            runSql.In.ConnectionName = connectionNameArgument;
            runSql.In.WithVersioning = true;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>();
            runSql.FetchInstalledVersions =
                Fake.Task<FetchInstalledVersions>(fiv => passedConnectionName = fiv.In.ConnectionName);
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScript = Fake.Task<RunSqlScript>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.FetchInstalledVersions.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedConnectionName, Is.EqualTo(connectionNameArgument));
        }

        [Test]
        public void should_run_sql_scripts_using_given_connection_name_without_versioning()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var runSql = Task.New<RunSql>();
            runSql.In.ConnectionName = connectionNameArgument;
            runSql.In.WithVersioning = false;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts =
                Fake.Task<GetSqlScripts>(gss => gss.Out.SqlScripts = _testSqlScripts);
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScript = Fake.Task<RunSqlScript>(rss => passedConnectionName = rss.In.ConnectionName);

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runSql.Execute();
            }

            // Assert
            Assert.That(runSql.RunSqlScript.Stats.ExecuteCount, Is.EqualTo(_testSqlScripts.Length));
            Assert.That(passedConnectionName, Is.EqualTo(connectionNameArgument));
        }

        [Test]
        public void should_run_missing_sql_scripts_using_given_connection_name_with_versioning()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var runSql = Task.New<RunSql>();
            runSql.In.ConnectionName = connectionNameArgument;
            runSql.In.WithVersioning = true;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>();
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions =
                Fake.Task<RunMissingVersions>(rmm => passedConnectionName = rmm.In.ConnectionName);
            runSql.RunSqlScript = Fake.Task<RunSqlScript>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.RunMissingVersions.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedConnectionName, Is.EqualTo(connectionNameArgument));
        }

        [Test]
        public void should_run_missing_sql_scripts_based_on_migrations_and_versions_with_versioning()
        {
            // Arrange
            var migrations = _testSqlScripts.Take(1).ToArray();
            var versions = _testVersions.Take(1).ToArray();
            var passedMigrations = new SqlScript[0];
            var passedVersions = new Version[0];

            var runSql = Task.New<RunSql>();
            runSql.In.WithVersioning = true;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = 
                Fake.Task<GetSqlScripts>(gms => gms.Out.SqlScripts = migrations);
            runSql.FetchInstalledVersions = 
                Fake.Task<FetchInstalledVersions>(fiv => fiv.Out.Versions = versions);
            runSql.RunMissingVersions =
                Fake.Task<RunMissingVersions>(rmm =>
                                                    {
                                                        passedMigrations = rmm.In.SqlScripts;
                                                        passedVersions = rmm.In.InstalledVersions;
                                                    });
            runSql.RunSqlScript = Fake.Task<RunSqlScript>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.RunMissingVersions.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedMigrations, Is.EqualTo(migrations));
            Assert.That(passedVersions, Is.EqualTo(versions));
        }
    }
}
