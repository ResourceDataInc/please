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
            var directoryPassedToGetMigrations = "";

            var migrateDatabase = Task.New<MigrateDatabase>();
            migrateDatabase.In.Args = new[] { "", directoryArgument};
            migrateDatabase.GetMigrationScripts = 
                Fake.Task<GetMigrationScripts>(gms => directoryPassedToGetMigrations = gms.In.Directory);
            migrateDatabase.FetchInstalledVersions = Fake.Task<FetchInstalledVersions>();
            migrateDatabase.RunMissingMigrations = Fake.Task<RunMissingMigrations>();
            
            // Act
            migrateDatabase.Execute();

            // Assert
            Check.That(migrateDatabase.GetMigrationScripts.Stats.ExecuteCount == 1, "Expected to get migrations scripts.");
            Check.That(directoryPassedToGetMigrations == directoryArgument, "Expected to get migrations scripts from given directory.");
        }
    }
}
