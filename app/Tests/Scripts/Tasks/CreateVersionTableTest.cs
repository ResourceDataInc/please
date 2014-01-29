using System;
using System.IO;
using Library.Scripts.Tasks;
using NUnit.Framework;
using Simpler;
using Simpler.Data;

namespace Tests.Scripts.Tasks
{
    [TestFixture]
    public class CreateVersionTableTest
    {
        [Test]
        public void should_create_version_table()
        {
            // Arrange
            File.Delete(@"Scripts\files\test.db");
            File.Copy(@"Scripts\files\empty.db", @"Scripts\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";

            // Act
            createVersionTable.Execute();

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count = Db.GetScalar(connection, "select count(1) from sqlite_master where type = 'table';");
                Assert.That(Convert.ToInt32(count), Is.EqualTo(1));
            }
        }
    }
}
