using System;
using System.IO;
using Library.Sql;
using Library.Sql.Tasks;
using NUnit.Framework;
using Simpler;
using Simpler.Data;

namespace Tests.Sql.Tasks
{
    [TestFixture]
    public class RunSqlScriptsTest
    {
        [Test]
        public void should_run_given_sql_scripts()
        {
            // Arrange
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runSqlScripts = Task.New<RunSqlScripts>();
            runSqlScripts.In.ConnectionName = "Test";
            runSqlScripts.In.SqlScripts = new[] {new SqlScript {FileName = @"Sql\files\insert-version.sql"}};

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runSqlScripts.Execute();
            }

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count = Db.GetScalar(connection, "select count(1) from db_version;");
                Assert.That(Convert.ToInt32(count), Is.EqualTo(1));
            }
        }

        [Test]
        public void should_run_given_sql_script()
        {
            // Arrange
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runSqlScripts = Task.New<RunSqlScripts>();
            runSqlScripts.In.ConnectionName = "Test";
            runSqlScripts.In.SqlScripts = new[] { new SqlScript { FileName = @"Sql\files\insert-version.sql" } };

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runSqlScripts.Execute();
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
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var runSqlScripts = Task.New<RunSqlScripts>();
            runSqlScripts.In.ConnectionName = "Test";
            runSqlScripts.In.SqlScripts = new[] {new SqlScript{FileName = @"Sql\files\sql\repeatable\create-four-tables.sql"}};

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runSqlScripts.Execute();
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
