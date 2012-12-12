using System.IO;
using Library.Migrate;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class InsertInstalledVersionTest
    {
        [Test]
        public void should_insert_version()
        {
            // Arrange
            File.Delete(@"Migrate\Files\test.db");
            File.Copy(@"Migrate\Files\empty.db", @"Migrate\Files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var insertInstalledVersion = Task.New<InsertInstalledVersion>();
            insertInstalledVersion.In.ConnectionName = "Test";
            insertInstalledVersion.In.Version = new Version {Id = "999"};

            // Act
            insertInstalledVersion.Execute();

            // Assert
            Check.That(insertInstalledVersion.Out.RowsAffected == 1, "Expected to insert 1 version.");
        }
    }
}
