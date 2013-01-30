using System;
using System.IO;
using Library.Sql.Tasks;
using NUnit.Framework;
using Simpler;
using Simpler.Data;
using Version = Library.Sql.Version;

namespace Tests.Sql.Tasks
{
    [TestFixture]
    public class InsertInstalledVersionTest
    {
        [Test]
        public void should_insert_version()
        {
            // Arrange
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var insertInstalledVersion = Task.New<InsertInstalledVersion>();
            insertInstalledVersion.In.ConnectionName = "Test";
            insertInstalledVersion.In.Version = new Version { Id = "999" };

            // Act
            insertInstalledVersion.Execute();

            // Assert
            Assert.That(insertInstalledVersion.Out.RowsAffected, Is.EqualTo(1));
        }

        [Test]
        public void should_insert_version_as_string()
        {
            // Arrange
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var insertInstalledVersion = Task.New<InsertInstalledVersion>();
            insertInstalledVersion.In.ConnectionName = "Test";
            insertInstalledVersion.In.Version = new Version { Id = "01" };

            // Act
            insertInstalledVersion.Execute();

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count = Db.GetScalar(connection, "select version from db_version where version = '01';");
                Assert.That(Convert.ToInt32(count), Is.EqualTo(1));
            }
        }
    }
}
