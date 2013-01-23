using System;
using System.IO;
using Library.Migrate;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class RunSqlTest
    {
        // TODO - factor out to shared test data
        readonly SqlScript[] _testSqlScripts =
            new[]
                {
                    new SqlScript {VersionId = "001", FileName = "001_first"},
                    new SqlScript {VersionId = "002", FileName = "002_second"},
                    new SqlScript {VersionId = "002", FileName = "002_third"},
                    new SqlScript {VersionId = "003", FileName = "003_fourth"}
                };

        [Test]
        public void should_not_create_version_table()
        {
            // Arrange
            var runSql = Task.New<RunSql>();
            runSql.In.Args = new[] { "", "" };
            runSql.In.WithVersioning = false;
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>(gss => gss.Out.SqlScripts = new SqlScript[0]);
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<RunMissingVersions>();
            runSql.RunSqlScript = Fake.Task<RunSqlScript>();

            // Act
            runSql.Execute();

            // Assert
            Check.That(runSql.CheckForVersionTable.Stats.ExecuteCount == 0,
                "Should not have checked for version table.");
            Check.That(runSql.CreateVersionTable.Stats.ExecuteCount == 0,
                "Expected version table to be not created.");
        }

        [Test]
        public void should_get_sql_scripts_using_from_directory()
        {
            // Arrange
            const string directoryArgument = "SomeDirectory";
            var passedDirectory = "";

            var runSql = Task.New<RunSql>();
            runSql.In.Args = new[] { "", directoryArgument };
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
            Check.That(runSql.GetSqlScripts.Stats.ExecuteCount == 1,
                "Expected to get migrations scripts.");
            Check.That(passedDirectory == directoryArgument,
                "Expected to get migrations scripts from given directory.");
        }

        [Test]
        public void should_run_sql_scripts_using_given_connection_name()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var runSql = Task.New<RunSql>();
            runSql.In.Args = new[] { connectionNameArgument, "" };
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
            Check.That(runSql.RunSqlScript.Stats.ExecuteCount == _testSqlScripts.Length,
                "Expected to run missing migrations.");
            Check.That(passedConnectionName == connectionNameArgument,
                "Expected to run missing migrations using given connection name.");
        }
    }
}
