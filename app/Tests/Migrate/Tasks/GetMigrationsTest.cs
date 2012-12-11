using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class GetMigrationsTest
    {
        [Test]
        public void should_find_files_in_given_directory()
        {
            // Arrange
            var getMigrations = Task.New<GetMigrations>();
            getMigrations.In.Directory = @"Migrate\Files\Migrations";

            // Act
            getMigrations.Execute();

            // Assert
            const string expectedFileName = @"000001_create-testing-table.sql";
            Check.That(getMigrations.Out.Migrations.Length == 1, "Expected to find 1 migration.");
            Check.That(getMigrations.Out.Migrations[0].FileName == expectedFileName,
                "Expected fileName of {0} not {1}", expectedFileName, getMigrations.Out.Migrations[0].FileName);
        }

        [Test]
        public void should_find_version_in_file_names()
        {
            // Arrange
            var getMigrations = Task.New<GetMigrations>();
            getMigrations.In.Directory = @"Migrate\Files\Migrations";

            // Act
            getMigrations.Execute();

            // Assert
            const string version = "000001";
            Check.That(getMigrations.Out.Migrations[0].VersionNumber == version,
                "Expected version {0} not {1}", version, getMigrations.Out.Migrations[0].VersionNumber);
        }
    }
}
