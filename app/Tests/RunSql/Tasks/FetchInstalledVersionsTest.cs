using System.IO;
using Library.RunSql;
using Library.RunSql.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.RunSql.Tasks
{
    [TestFixture]
    public class FetchInstalledVersionsTest
    {
        [Test]
        public void should_fetch_installed_versions()
        {
            // Arrange
            File.Delete(@"RunSql\files\test.db");
            File.Copy(@"RunSql\files\empty.db", @"RunSql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runMigration = Task.New<RunSqlScript>();
            runMigration.In.ConnectionName = "Test";
            runMigration.In.SqlScript = new SqlScript { FileName = @"RunSql\files\insert-version.sql" };
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
