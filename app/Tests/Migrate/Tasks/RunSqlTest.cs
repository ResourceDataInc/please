using System.Linq;
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
        public void should_create_version_table_if_it_does_not_exist()
        {
            // Arrange
            var runSql = Task.New<RunSql>();
            runSql.In.Args = new[] { "", "" };
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>(cfvt => cfvt.Out.TableExists = false);
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>();
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<Library.Migrate.Tasks.RunMissingVersions>();

            // Act
            runSql.Execute();

            // Assert
            Check.That(runSql.CreateVersionTable.Stats.ExecuteCount == 1,
                "Expected version table to be created.");
        }

        [Test]
        public void should_not_create_version_table_if_it_exists()
        {
            // Arrange
            var runSql = Task.New<RunSql>();
            runSql.In.Args = new[] { "", "" };
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>(cfvt => cfvt.Out.TableExists = true);
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>();
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<Library.Migrate.Tasks.RunMissingVersions>();

            // Act
            runSql.Execute();

            // Assert
            Check.That(runSql.CreateVersionTable.Stats.ExecuteCount == 0,
                "Expected version table to be not created.");
        }

        [Test]
        public void should_get_migration_scripts_using_given_directory()
        {
            // Arrange
            const string directoryArgument = "SomeDirectory";
            var passedDirectory = "";

            var runSql = Task.New<RunSql>();
            runSql.In.Args = new[] { "", directoryArgument };
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts =
                Fake.Task<GetSqlScripts>(gms => passedDirectory = gms.In.Directory);
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions = Fake.Task<Library.Migrate.Tasks.RunMissingVersions>();

            // Act
            runSql.Execute();

            // Assert
            Check.That(runSql.GetSqlScripts.Stats.ExecuteCount == 1,
                "Expected to get migrations scripts.");
            Check.That(passedDirectory == directoryArgument,
                "Expected to get migrations scripts from given directory.");
        }

        [Test]
        public void should_fetch_installed_versions_using_given_connection_name()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var runSql = Task.New<RunSql>();
            runSql.In.Args = new[] { connectionNameArgument, "" };
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>();
            runSql.FetchInstalledVersions =
                Fake.Task<FetchInstalledVersions>(fiv => passedConnectionName = fiv.In.ConnectionName);
            runSql.RunMissingVersions = Fake.Task<Library.Migrate.Tasks.RunMissingVersions>();

            // Act
            runSql.Execute();

            // Assert
            Check.That(runSql.FetchInstalledVersions.Stats.ExecuteCount == 1,
                "Expected to fetch installed versions.");
            Check.That(passedConnectionName == connectionNameArgument,
                "Expected to fetch installed versions using given connection name.");
        }

        [Test]
        public void should_run_missing_migrations_using_given_connection_name()
        {
            // Arrange
            const string connectionNameArgument = "SomeConnection";
            var passedConnectionName = "";

            var runSql = Task.New<RunSql>();
            runSql.In.Args = new[] { connectionNameArgument, "" };
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = Fake.Task<GetSqlScripts>();
            runSql.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            runSql.RunMissingVersions =
                Fake.Task<Library.Migrate.Tasks.RunMissingVersions>(rmm => passedConnectionName = rmm.In.ConnectionName);

            // Act
            runSql.Execute();

            // Assert
            Check.That(runSql.RunMissingVersions.Stats.ExecuteCount == 1,
                "Expected to run missing migrations.");
            Check.That(passedConnectionName == connectionNameArgument,
                "Expected to run missing migrations using given connection name.");
        }

        [Test]
        public void should_run_missing_migrations_based_on_migrations_and_versions()
        {
            // Arrange
            var migrations = _testSqlScripts.Take(1).ToArray();
            var versions = _testVersions.Take(1).ToArray();
            var passedMigrations = new SqlScript[0];
            var passedVersions = new Version[0];

            var runSql = Task.New<RunSql>();
            runSql.In.Args = new[] { "", "" };
            runSql.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            runSql.CreateVersionTable = Fake.Task<CreateVersionTable>();
            runSql.GetSqlScripts = 
                Fake.Task<GetSqlScripts>(gms => gms.Out.SqlScripts = migrations);
            runSql.FetchInstalledVersions = 
                Fake.Task<FetchInstalledVersions>(fiv => fiv.Out.Versions = versions);
            runSql.RunMissingVersions =
                Fake.Task<Library.Migrate.Tasks.RunMissingVersions>(rmm =>
                                                    {
                                                        passedMigrations = rmm.In.SqlScripts;
                                                        passedVersions = rmm.In.InstalledVersions;
                                                    });

            // Act
            runSql.Execute();

            // Assert
            Check.That(runSql.RunMissingVersions.Stats.ExecuteCount == 1,
                "Expected to run missing migrations.");
            Check.That(passedMigrations == migrations,
                "Expected to run missing migrations using migrations found.");
            Check.That(passedVersions == versions,
                "Expected to run missing migrations using versions found.");
        }
    }
}
