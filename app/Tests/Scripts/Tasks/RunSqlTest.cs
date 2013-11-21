using System;
using System.IO;
using Library.Scripts;
using Library.Scripts.Tasks;
using NUnit.Framework;
using Simpler;
using Simpler.Data;

namespace Tests.Scripts.Tasks
{
    [TestFixture]
    public class RunSqlTest
    {
        [Test]
        public void should_run_sql()
        {
            // Arrange
            File.Delete(@"Scripts\files\test.db");
            File.Copy(@"Scripts\files\empty.db", @"Scripts\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runScripts = Task.New<RunSql>();
            runScripts.In.ConnectionName = "Test";
            runScripts.In.Sql = new Script {FileName = @"Scripts\files\insert-version.sql"};

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runScripts.Execute();
            }

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count = Db.GetScalar(connection, "select count(1) from db_version;");
                Assert.That(Convert.ToInt32(count), Is.EqualTo(1));
            }
        }

        [Test]
        public void should_split_sql_if_it_contains_GO_keyword()
        {
            // Arrange
            File.Delete(@"Scripts\files\test.db");
            File.Copy(@"Scripts\files\empty.db", @"Scripts\files\test.db");

            var runScripts = Task.New<RunSql>();
            runScripts.In.ConnectionName = "Test";
            runScripts.In.Sql = new Script{FileName = @"Scripts\files\sql\repeatable\create-four-tables.sql"};

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runScripts.Execute();
            }

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count1 = Db.GetScalar(connection, "select count(1) from sqlite_master where name = 'table1';");
                Assert.That(Convert.ToInt32(count1), Is.EqualTo(1));

                var count2 = Db.GetScalar(connection, "select count(1) from sqlite_master where name = 'table2';");
                Assert.That(Convert.ToInt32(count2), Is.EqualTo(1));

                var count3 = Db.GetScalar(connection, "select count(1) from sqlite_master where name = 'table3';");
                Assert.That(Convert.ToInt32(count3), Is.EqualTo(1));

                var count4 = Db.GetScalar(connection, "select count(1) from sqlite_master where name = 'table4';");
                Assert.That(Convert.ToInt32(count4), Is.EqualTo(1));
            }
        }
    }
}
