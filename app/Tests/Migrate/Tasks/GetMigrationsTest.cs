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
            Check.That(getMigrations.Out.Migrations.Length == 4, "Expected to find 1 migration.");
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

        [Test]
        public void should_return_files_in_alphabetical_order()
        {
            // Arrange
            var getMigrations = Task.New<GetMigrations>();
            getMigrations.In.Directory = @"Migrate\Files\Migrations";

            // Act
            getMigrations.Execute();

            // Assert
            const string first = "000001_create-testing-table.sql";
            const string second = "000002_001.sql";
            const string third = "000002_002.sql";
            const string fourth = "000003_do-something-else.sql";
            Check.That(getMigrations.Out.Migrations[0].FileName == first,
                "Expected first file to be {0} not {1}", first, getMigrations.Out.Migrations[0].FileName);
            Check.That(getMigrations.Out.Migrations[1].FileName == second,
                "Expected first file to be {0} not {1}", first, getMigrations.Out.Migrations[1].FileName);
            Check.That(getMigrations.Out.Migrations[2].FileName == third,
                "Expected first file to be {0} not {1}", first, getMigrations.Out.Migrations[2].FileName);
            Check.That(getMigrations.Out.Migrations[3].FileName == fourth,
                "Expected first file to be {0} not {1}", first, getMigrations.Out.Migrations[3].FileName);
        }
    }
}
