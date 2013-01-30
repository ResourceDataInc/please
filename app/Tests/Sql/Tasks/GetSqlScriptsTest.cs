using Library.Sql.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Sql.Tasks
{
    [TestFixture]
    public class GetSqlScriptsTest
    {
        [Test]
        public void should_find_files_in_given_directory()
        {
            // Arrange
            var getSqlScripts = Task.New<GetSqlScripts>();
            getSqlScripts.In.Directory = @"Sql\files\sql\versioned";

            // Act
            getSqlScripts.Execute();

            // Assert
            const string expectedFileName = @"Sql\files\sql\versioned\000001_create-testing-table.sql";
            Assert.That(getSqlScripts.Out.SqlScripts.Length, Is.EqualTo(4));
            Assert.That(getSqlScripts.Out.SqlScripts[0].FileName, Is.EqualTo(expectedFileName));
        }

        [Test]
        public void should_find_versioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetSqlScripts>();
            getSqlScripts.In.Directory = @"Sql\files\sql\versioned";

            // Act
            getSqlScripts.Execute();

            // Assert
            var script = getSqlScripts.Out.SqlScripts[0];
            const string version = "000001";
            Assert.That(script.IsVersioned, "Expected script to be versioned.");
            Assert.That(script.VersionId, Is.EqualTo(version));
        }

        [Test]
        public void should_find_unversioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetSqlScripts>();
            getSqlScripts.In.Directory = @"Sql\files\sql\repeatable";

            // Act
            getSqlScripts.Execute();

            // Assert
            var script = getSqlScripts.Out.SqlScripts[0];
            Assert.That(!script.IsVersioned, "Expected script to be unversioned.");
        }

        [Test]
        public void should_check_for_versioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetSqlScripts>();
            getSqlScripts.In.Directory = @"Sql\files\sql\repeatable";
            getSqlScripts.In.CheckForVersionedFilesOnly = true;

            // Act & Assert
            Assert.Throws<CheckException>(getSqlScripts.Execute);
        }
    }
}
