using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class GetSqlScriptsTest
    {
        [Test]
        public void should_find_files_in_given_directory()
        {
            // Arrange
            var getSqlScripts = Task.New<GetSqlScripts>();
            getSqlScripts.In.Directory = @"Migrate\files\sql\versioned";

            // Act
            getSqlScripts.Execute();

            // Assert
            const string expectedFileName = @"Migrate\files\sql\versioned\000001_create-testing-table.sql";
            Check.That(getSqlScripts.Out.SqlScripts.Length == 4, "Expected to find 1 migration.");
            Check.That(getSqlScripts.Out.SqlScripts[0].FileName == expectedFileName,
                "Expected fileName of {0} not {1}", expectedFileName, getSqlScripts.Out.SqlScripts[0].FileName);
        }

        [Test]
        public void should_find_versioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetSqlScripts>();
            getSqlScripts.In.Directory = @"Migrate\files\sql\versioned";

            // Act
            getSqlScripts.Execute();

            // Assert
            var script = getSqlScripts.Out.SqlScripts[0];
            const string version = "000001";
            Check.That(script.IsVersioned, "Expected script to be versioned.");
            Check.That(script.VersionId == version,
                "Expected version {0} not {1}", version, script.VersionId);
        }

        [Test]
        public void should_find_unversioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetSqlScripts>();
            getSqlScripts.In.Directory = @"Migrate\files\sql\repeatable";

            // Act
            getSqlScripts.Execute();

            // Assert
            var script = getSqlScripts.Out.SqlScripts[0];
            Check.That(!script.IsVersioned, "Expected script to be unversioned.");
        }

        [Test]
        public void should_check_for_versioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetSqlScripts>();
            getSqlScripts.In.Directory = @"Migrate\files\sql\repeatable";
            getSqlScripts.In.CheckForVersionedFilesOnly = true;

            // Act & Assert
            Check.Throws<CheckException>(getSqlScripts.Execute);
        }
    }
}
