using System.Linq;
using Library.Migrate.Model;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class RunMissingMigrationsTest
    {
        readonly Migration[] _testMigrations =
            new[]
                {
                    new Migration {VersionId = "001", FileName = "001_first"},
                    new Migration {VersionId = "002", FileName = "002_second"}
                };

        [Test]
        public void should_run_all_migrations_if_no_versions_exist()
        {
            // Arrange
            var runMissingMigrations = Task.New<RunMissingMigrations>();
            runMissingMigrations.In.InstalledVersions = new Version[0];
            runMissingMigrations.In.Migrations = _testMigrations;
            runMissingMigrations.RunMigration = Fake.Task<RunMigration>();

            // Act
            runMissingMigrations.Execute();

            // Assert
            Check.That(runMissingMigrations.RunMigration.Stats.ExecuteCount == _testMigrations.Length, 
                "Expected {0} migration to be ran.", _testMigrations.Length);
        }
    }
}
