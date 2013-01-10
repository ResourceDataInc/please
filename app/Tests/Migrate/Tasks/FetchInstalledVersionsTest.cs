using System.IO;
using Library.Migrate;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class FetchInstalledVersionsTest
    {
        [Test]
        public void should_fetch_installed_versions()
        {
            // Arrange
            File.Delete(@"Migrate\files\test.db");
            File.Copy(@"Migrate\files\empty.db", @"Migrate\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runMigration = Task.New<RunMigration>();
            runMigration.In.ConnectionName = "Test";
            runMigration.In.Migration = new Migration { FileName = @"Migrate\files\insert-version.sql" };
            runMigration.Execute();

            var fetchInstalledVersions = Task.New<FetchInstalledVersions>();
            fetchInstalledVersions.In.ConnectionName = "Test";

            // Act
            fetchInstalledVersions.Execute();

            // Assert
            Check.That(fetchInstalledVersions.Out.Versions.Length == 1, "Expected to find 1 version.");
        }
    }
}
