using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class MigrateDatabaseTest
    {
        [Test]
        public void should_get_migration_scripts_using_given_directory()
        {
            // Arrange
            const string directoryArgument = "SomeDirectory";
            var passedDirectory = "";

            var migrateDatabase = Task.New<MigrateDatabase>();
            migrateDatabase.In.Args = new[] { "", directoryArgument };
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
    }
}
