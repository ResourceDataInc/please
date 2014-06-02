using Library.Scripts.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Scripts.Tasks
{
    [TestFixture]
    public class CheckForVersionTableTest
    {
        [Test]
        public void should_find_version_table_if_it_exists()
        {
            // Arrange
            Database.Restore();

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = Database.Name;
            createVersionTable.Execute();

            var checkForVersionTable = Task.New<CheckForVersionTable>();
            checkForVersionTable.In.ConnectionName = Database.Name;

            // Act
            checkForVersionTable.Execute();

            // Assert
            Assert.That(checkForVersionTable.Out.TableExists, "Expected to find version table.");
        }

        [Test]
        public void should_not_find_version_table_if_it_does_not_exist()
        {
            // Arrange
            Database.Restore();

            var checkForVersionTable = Task.New<CheckForVersionTable>();
            checkForVersionTable.In.ConnectionName = Database.Name;

            // Act
            checkForVersionTable.Execute();

            // Assert
            Assert.That(!checkForVersionTable.Out.TableExists, "Expected to not find version table.");
        }
    }
}
