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
            getSqlScripts.In.Directory = @"Scripts\files\sql\versioned";
            getSqlScripts.In.Extensions = new[] { ".sql" };

            // Act
            getSqlScripts.Execute();

            // Assert
            const string expectedFileName = @"Scripts\files\sql\versioned\000001_create-testing-table.sql";
            Assert.That(getSqlScripts.Out.Scripts.Length, Is.EqualTo(4));
            Assert.That(getSqlScripts.Out.Scripts[0].FileName, Is.EqualTo(expectedFileName));
        }

        [Test]
        public void should_find_py_files_in_given_directory()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = @"Scripts\files\py";
            getSqlScripts.In.Extensions = new[] { ".py" };

            // Act
            getSqlScripts.Execute();

            // Assert
            const string expectedFileName = @"Scripts\files\py\1_hello.py";
            Assert.That(getSqlScripts.Out.Scripts.Length, Is.EqualTo(2));
            Assert.That(getSqlScripts.Out.Scripts[0].FileName, Is.EqualTo(expectedFileName));
        }

        [Test]
        public void should_respect_whitelist()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = @"Scripts\files\sql\versioned";
            getSqlScripts.In.Extensions = new[] { ".sql" };
            getSqlScripts.In.WhitelistFile = @"Scripts\files\sql\versioned\whitelist.txt";


            // Act
            getSqlScripts.Execute();

            // Assert
            Assert.That(getSqlScripts.Out.Scripts.Length, Is.EqualTo(2));
            Assert.That(getSqlScripts.Out.Scripts[0].FileName, Is.EqualTo(@"Scripts\files\sql\versioned\000002_001.sql"));
            Assert.That(getSqlScripts.Out.Scripts[1].FileName, Is.EqualTo(@"Scripts\files\sql\versioned\000002_002.sql"));
        }

        [Test]
        public void should_mark_versioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = @"Scripts\files\sql\versioned";
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
        public void should_mark_unversioned_files()
        {
            // Arrange
            var getSqlScripts = Task.New<GetScripts>();
            getSqlScripts.In.Directory = @"Scripts\files\sql\repeatable";
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
            getSqlScripts.In.Directory = @"Scripts\files\sql\repeatable";
            getSqlScripts.In.CheckForVersionedFilesOnly = true;
            getSqlScripts.In.Extensions = new[] { ".sql" };

            // Act & Assert
            Assert.Throws<CheckException>(getSqlScripts.Execute);
        }
    }
}
