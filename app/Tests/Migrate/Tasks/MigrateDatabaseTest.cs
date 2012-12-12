using System.Linq;
using Library.Migrate;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class MigrateDatabaseTest
    {
        // TODO - factor out to shared test data
        readonly Version[] _testVersions =
            new[]
                {
                    new Version {Id = "001"},
                    new Version {Id = "002"},
                    new Version {Id = "003"}
                };

        readonly Migration[] _testMigrations =
            new[]
                {
                    new Migration {VersionId = "001", FileName = "001_first"},
                    new Migration {VersionId = "002", FileName = "002_second"},
                    new Migration {VersionId = "002", FileName = "002_third"},
                    new Migration {VersionId = "003", FileName = "003_fourth"}
                };

        [Test]
        public void should_create_version_table_if_it_does_not_exist()
        {
            // Arrange
            var migrateDatabase = Task.New<MigrateDatabase>();
            migrateDatabase.In.Args = new[] { "", "" };
            migrateDatabase.CheckForVersionTable = Fake.Task<CheckForVersionTable>(cfvt => cfvt.Out.TableExists = false);
            migrateDatabase.CreateVersionTable = Fake.Task<CreateVersionTable>();
            migrateDatabase.GetMigrationScripts = Fake.Task<GetMigrationScripts>();
            migrateDatabase.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            migrateDatabase.RunMissingMigrations = Fake.Task<RunMissingMigrations>();

            // Act
            migrateDatabase.Execute();

            // Assert
            Check.That(migrateDatabase.CreateVersionTable.Stats.ExecuteCount == 1,
                "Expected version table to be created.");
        }

        [Test]
        public void should_not_create_version_table_if_it_exists()
        {
            // Arrange
            var migrateDatabase = Task.New<MigrateDatabase>();
            migrateDatabase.In.Args = new[] { "", "" };
            migrateDatabase.CheckForVersionTable = Fake.Task<CheckForVersionTable>(cfvt => cfvt.Out.TableExists = true);
            migrateDatabase.CreateVersionTable = Fake.Task<CreateVersionTable>();
            migrateDatabase.GetMigrationScripts = Fake.Task<GetMigrationScripts>();
            migrateDatabase.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            migrateDatabase.RunMissingMigrations = Fake.Task<RunMissingMigrations>();

            // Act
            migrateDatabase.Execute();

            // Assert
            Check.That(migrateDatabase.CreateVersionTable.Stats.ExecuteCount == 0,
                "Expected version table to be not created.");
        }

        [Test]
        public void should_get_migration_scripts_using_given_directory()
        {
            // Arrange
            const string directoryArgument = "SomeDirectory";
            var passedDirectory = "";

            var migrateDatabase = Task.New<MigrateDatabase>();
            migrateDatabase.In.Args = new[] { "", directoryArgument };
            migrateDatabase.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            migrateDatabase.CreateVersionTable = Fake.Task<CreateVersionTable>();
            migrateDatabase.GetMigrationScripts =
                Fake.Task<GetMigrationScripts>(gms => passedDirectory = gms.In.Directory);
            migrateDatabase.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            migrateDatabase.RunMissingMigrations = Fake.Task<RunMissingMigrations>();

            // Act
            migrateDatabase.Execute();

            // Assert
            Check.That(migrateDatabase.GetMigrationScripts.Stats.ExecuteCount == 1,
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

            var migrateDatabase = Task.New<MigrateDatabase>();
            migrateDatabase.In.Args = new[] { connectionNameArgument, "" };
            migrateDatabase.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            migrateDatabase.CreateVersionTable = Fake.Task<CreateVersionTable>();
            migrateDatabase.GetMigrationScripts = Fake.Task<GetMigrationScripts>();
            migrateDatabase.FetchInstalledVersions =
                Fake.Task<FetchInstalledVersions>(fiv => passedConnectionName = fiv.In.ConnectionName);
            migrateDatabase.RunMissingMigrations = Fake.Task<RunMissingMigrations>();

            // Act
            migrateDatabase.Execute();

            // Assert
            Check.That(migrateDatabase.FetchInstalledVersions.Stats.ExecuteCount == 1,
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

            var migrateDatabase = Task.New<MigrateDatabase>();
            migrateDatabase.In.Args = new[] { connectionNameArgument, "" };
            migrateDatabase.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            migrateDatabase.CreateVersionTable = Fake.Task<CreateVersionTable>();
            migrateDatabase.GetMigrationScripts = Fake.Task<GetMigrationScripts>();
            migrateDatabase.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            migrateDatabase.RunMissingMigrations =
                Fake.Task<RunMissingMigrations>(rmm => passedConnectionName = rmm.In.ConnectionName);

            // Act
            migrateDatabase.Execute();

            // Assert
            Check.That(migrateDatabase.RunMissingMigrations.Stats.ExecuteCount == 1,
                "Expected to run missing migrations.");
            Check.That(passedConnectionName == connectionNameArgument,
                "Expected to run missing migrations using given connection name.");
        }

        [Test]
        public void should_run_missing_migrations_based_on_migrations_and_versions()
        {
            // Arrange
            var migrations = _testMigrations.Take(1).ToArray();
            var versions = _testVersions.Take(1).ToArray();
            var passedMigrations = new Migration[0];
            var passedVersions = new Version[0];

            var migrateDatabase = Task.New<MigrateDatabase>();
            migrateDatabase.In.Args = new[] { "", "" };
            migrateDatabase.CheckForVersionTable = Fake.Task<CheckForVersionTable>();
            migrateDatabase.CreateVersionTable = Fake.Task<CreateVersionTable>();
            migrateDatabase.GetMigrationScripts = 
                Fake.Task<GetMigrationScripts>(gms => gms.Out.Migrations = migrations);
            migrateDatabase.FetchInstalledVersions = 
                Fake.Task<FetchInstalledVersions>(fiv => fiv.Out.Versions = versions);
            migrateDatabase.RunMissingMigrations =
                Fake.Task<RunMissingMigrations>(rmm =>
                                                    {
                                                        passedMigrations = rmm.In.Migrations;
                                                        passedVersions = rmm.In.InstalledVersions;
                                                    });

            // Act
            migrateDatabase.Execute();

            // Assert
            Check.That(migrateDatabase.RunMissingMigrations.Stats.ExecuteCount == 1,
                "Expected to run missing migrations.");
            Check.That(passedMigrations == migrations,
                "Expected to run missing migrations using migrations found.");
            Check.That(passedVersions == versions,
                "Expected to run missing migrations using versions found.");
        }
    }
}
