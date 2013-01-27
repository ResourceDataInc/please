using System.IO;
using Library.Sql.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Sql.Tasks
{
    [TestFixture]
    public class CheckForVersionTableTest
    {
        [Test]
        public void should_find_version_table_if_it_exists()
        {
            // Arrange
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var checkForVersionTable = Task.New<CheckForVersionTable>();
            checkForVersionTable.In.ConnectionName = "Test";

            // Act
            checkForVersionTable.Execute();

            // Assert
            Check.That(checkForVersionTable.Out.TableExists, "Expected to find version table.");
        }

        [Test]
        public void should_not_find_version_table_if_it_does_not_exist()
        {
            // Arrange
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var checkForVersionTable = Task.New<CheckForVersionTable>();
            checkForVersionTable.In.ConnectionName = "Test";

            // Act
            checkForVersionTable.Execute();

            // Assert
            Check.That(!checkForVersionTable.Out.TableExists, "Expected to not find version table.");
        }
    }
}
