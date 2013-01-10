using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class GetMigrationScriptsTest
    {
        [Test]
        public void should_find_files_in_given_directory()
        {
            // Arrange
            var getMigrationScripts = Task.New<GetMigrationScripts>();
            getMigrationScripts.In.Directory = @"Migrate\files\sql\versioned";

            // Act
            getMigrationScripts.Execute();

            // Assert
            const string expectedFileName = @"Migrate\files\sql\versioned\000001_create-testing-table.sql";
            Check.That(getMigrationScripts.Out.Migrations.Length == 4, "Expected to find 1 migration.");
            Check.That(getMigrationScripts.Out.Migrations[0].FileName == expectedFileName,
                "Expected fileName of {0} not {1}", expectedFileName, getMigrationScripts.Out.Migrations[0].FileName);
        }

        [Test]
        public void should_find_version_in_file_names()
        {
            // Arrange
            var getMigrationScripts = Task.New<GetMigrationScripts>();
            getMigrationScripts.In.Directory = @"Migrate\files\sql\versioned";

            // Act
            getMigrationScripts.Execute();

            // Assert
            const string version = "000001";
            Check.That(getMigrationScripts.Out.Migrations[0].VersionId == version,
                "Expected version {0} not {1}", version, getMigrationScripts.Out.Migrations[0].VersionId);
        }
    }
}
