using System;
using System.IO;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;
using Simpler.Data;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class CreateVersionTableTest
    {
        [Test]
        public void should_create_version_table()
        {
            // Arrange
            File.Delete(@"Migrate\files\test.db");
            File.Copy(@"Migrate\files\empty.db", @"Migrate\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";

            // Act
            createVersionTable.Execute();

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count = Db.GetScalar(connection, "select count(1) from sqlite_master where type = 'table';");
                Check.That(Convert.ToInt32(count) == 1, "Expected to find one table in the database not {0}.", count);
            }
        }
    }
}
