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
        public static Version[] TestVersions =
            new[]
                {
                    new Version {Id = "001"},
                    new Version {Id = "002"},
                    new Version {Id = "003"}
                };

        public static SqlScript[] TestSqlScripts =
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
            runSql.GetScripts = Fake.Task<GetScripts>(gss => gss.Out.SqlScripts = new SqlScript[0]);
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>();

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
            runSql.GetScripts = Fake.Task<GetScripts>();
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>();

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
            runSql.GetScripts = Fake.Task<GetScripts>();
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>();

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
            runSql.GetScripts =
                Fake.Task<GetScripts>(gss =>
                {
                    passedDirectory = gss.In.Directory;
                    gss.Out.SqlScripts = TestSqlScripts;
                });
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runSql.Execute();
            }

            // Assert
            Assert.That(runSql.GetScripts.Stats.ExecuteCount, Is.EqualTo(1));
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
            runSql.GetScripts =
                Fake.Task<GetScripts>(gss => passedDirectory = gss.In.Directory);
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.GetScripts.Stats.ExecuteCount, Is.EqualTo(1));
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
            runSql.GetScripts = Fake.Task<GetScripts>();
            runSql.FetchInstalledVersions =
                Fake.Task<FetchInstalledVersions>(fiv => passedConnectionName = fiv.In.ConnectionName);
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.FetchInstalledVersions.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedConnectionName, Is.EqualTo(connectionNameArgument));
        }

        [Test]
        public void should_run_sql_specified_sql_file_without_versioning()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var runSql = Task.New<RunSql>();
            runSql.In.File = "001_first.sql";
            runSql.In.ConnectionName = connectionNameArgument;
            runSql.In.WithVersioning = false;
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>(rss => passedConnectionName = rss.In.ConnectionName);

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runSql.Execute();
            }

            // Assert
            Assert.That(runSql.RunSqlScripts.Stats.ExecuteCount, Is.GreaterThan(0));
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
            runSql.GetScripts =
                Fake.Task<GetScripts>(gss => gss.Out.SqlScripts = TestSqlScripts);
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>(rss => passedConnectionName = rss.In.ConnectionName);

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runSql.Execute();
            }

            // Assert
            Assert.That(runSql.RunSqlScripts.Stats.ExecuteCount, Is.GreaterThan(0));
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
            runSql.GetScripts = Fake.Task<GetScripts>();
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions =
                Fake.Task<RunMissingVersions>(rmm => passedConnectionName = rmm.In.ConnectionName);
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.RunMissingVersions.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedConnectionName, Is.EqualTo(connectionNameArgument));
        }

        [Test]
        public void should_run_missing_sql_scripts_based_on_sql_scripts_and_versions_with_versioning()
        {
            // Arrange
            var sqlScripts = TestSqlScripts.Take(1).ToArray();
            var versions = TestVersions.Take(1).ToArray();
            var passedSqlScripts = new SqlScript[0];
            var passedVersions = new Version[0];

            var runSql = Task.New<RunSql>();
            runSql.In.WithVersioning = true;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetScripts =
                Fake.Task<GetScripts>(gms => gms.Out.SqlScripts = sqlScripts);
            runSql.FetchInstalledVersions = 
                Fake.Task<FetchInstalledVersions>(fiv => fiv.Out.Versions = versions);
            runSql.RunMissingVersions =
                Fake.Task<RunMissingVersions>(rmm =>
                                                    {
                                                        passedSqlScripts = rmm.In.SqlScripts;
                                                        passedVersions = rmm.In.InstalledVersions;
                                                    });
            runSql.RunSqlScripts = Fake.Task<RunSqlScripts>();

            // Act
            runSql.Execute();

            // Assert
            Assert.That(runSql.RunMissingVersions.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(passedSqlScripts, Is.EqualTo(sqlScripts));
            Assert.That(passedVersions, Is.EqualTo(versions));
        }
    }
}
