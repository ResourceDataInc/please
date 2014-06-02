using Library.Scripts.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Scripts.Tasks
{
    [TestFixture]
    public class GetScriptsTest
    {
        [Test]
        public void should_find_sql_files_in_given_directory()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = Config.Scripts.Files.Sql.VersionedFiles;
            getSqlScripts.In.Extensions = new[] { ".sql" };

            // Act
            getSqlScripts.Execute();

            // Assert
            Assert.That(getSqlScripts.Out.Scripts.Length, Is.EqualTo(4));
            Assert.That(getSqlScripts.Out.Scripts[0].FileName, Is.EqualTo(Config.Scripts.Files.Sql.Versioned.First));
        }

        [Test]
        public void should_find_py_files_in_given_directory()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = Config.Scripts.Files.PyFiles;
            getSqlScripts.In.Extensions = new[] { ".py" };

            // Act
            getSqlScripts.Execute();

            // Assert
            Assert.That(getSqlScripts.Out.Scripts.Length, Is.EqualTo(2));
            Assert.That(getSqlScripts.Out.Scripts[0].FileName, Is.EqualTo(Config.Scripts.Files.Py.Hello));
        }

        [Test]
        public void should_respect_whitelist()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = Config.Scripts.Files.Sql.VersionedFiles;
            getSqlScripts.In.Extensions = new[] { ".sql" };
            getSqlScripts.In.WhitelistFile = Config.Scripts.Files.Sql.Versioned.Whitelist;


            // Act
            getSqlScripts.Execute();

            // Assert
            Assert.That(getSqlScripts.Out.Scripts.Length, Is.EqualTo(2));
            Assert.That(getSqlScripts.Out.Scripts[0].FileName, Is.EqualTo(Config.Scripts.Files.Sql.Versioned.Second));
            Assert.That(getSqlScripts.Out.Scripts[1].FileName, Is.EqualTo(Config.Scripts.Files.Sql.Versioned.Third));
        }

        [Test]
        public void should_mark_versioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = Config.Scripts.Files.Sql.VersionedFiles;
            getSqlScripts.In.Extensions = new[] { ".sql" };

            // Act
            getSqlScripts.Execute();

            // Assert
            var script = getSqlScripts.Out.Scripts[0];
            const string version = "000001";
            Assert.That(script.IsVersioned, "Expected script to be versioned.");
            Assert.That(script.VersionId, Is.EqualTo(version));
        }

        [Test]
        public void should_not_mark_unversioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = Config.Scripts.Files.Sql.GoFiles;
            getSqlScripts.In.Extensions = new[] { ".sql" };

            // Act
            getSqlScripts.Execute();

            // Assert
            var script = getSqlScripts.Out.Scripts[0];
            Assert.That(!script.IsVersioned, "Expected script to be unversioned.");
        }

        [Test]
        public void should_check_for_versioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = Config.Scripts.Files.Sql.GoFiles;
            getSqlScripts.In.CheckForVersionedFilesOnly = true;
            getSqlScripts.In.Extensions = new[] { ".sql" };

            // Act & Assert
            Assert.Throws<CheckException>(getSqlScripts.Execute);
        }
    }
}
