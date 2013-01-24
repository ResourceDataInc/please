using System;
using System.IO;
using Library.RunSql.Tasks;
using NUnit.Framework;
using Simpler;
using Simpler.Data;

namespace Tests.RunSql.Tasks
{
    [TestFixture]
    public class CreateVersionTableTest
    {
        [Test]
        public void should_create_version_table()
        {
            // Arrange
            File.Delete(@"RunSql\files\test.db");
            File.Copy(@"RunSql\files\empty.db", @"RunSql\files\test.db");

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
