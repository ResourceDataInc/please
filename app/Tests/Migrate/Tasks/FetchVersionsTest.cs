using System.IO;
using Library.Migrate.Model;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class FetchVersionsTest
    {
        [Test]
        public void should_fetch_installed_versions()
        {
            // Arrange
            File.Delete(@"Migrate\Files\test.db");
            File.Copy(@"Migrate\Files\empty.db", @"Migrate\Files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runMigration = Task.New<RunMigration>();
            runMigration.In.ConnectionName = "Test";
            runMigration.In.Migration = new Migration {FileNameWithPath = @"Migrate\Files\insert-version.sql"};
            runMigration.Execute();

            var fetchVersions = Task.New<FetchVersions>();
            fetchVersions.In.ConnectionName = "Test";

            // Act
            fetchVersions.Execute();

            // Assert
            Check.That(fetchVersions.Out.Versions.Length == 1, "Expected to find 1 version.");
        }
    }
}
