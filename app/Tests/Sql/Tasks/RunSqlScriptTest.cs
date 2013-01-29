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
    public class RunSqlScriptTest
    {
        [Test]
        public void should_run_given_sql_script()
        {
            // Arrange
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runSqlScript = Task.New<RunSqlScript>();
            runSqlScript.In.ConnectionName = "Test";
            runSqlScript.In.SqlScript = new SqlScript {FileName = @"Sql\files\insert-version.sql"};

            // Act
            runSqlScript.Execute();

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count = Db.GetScalar(connection, "select count(1) from db_version;");
                Check.That(Convert.ToInt32(count) == 1, "Expected to find one version record in the database not {0}.", count);
            }
        }

        [Test]
        public void should_split_sql_if_it_contains_GO_keyword()
        {
            // Arrange
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var runSqlScript = Task.New<RunSqlScript>();
            runSqlScript.In.ConnectionName = "Test";
            runSqlScript.In.SqlScript = new SqlScript {FileName = @"Sql\files\sql\repeatable\create-four-tables.sql"};

            // Act
            runSqlScript.Execute();

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count1 = Db.GetScalar(connection, "select count(1) from sqlite_master where name = 'table1';");
                Check.That(Convert.ToInt32(count1) == 1, "Expected to table1.");

                var count2 = Db.GetScalar(connection, "select count(1) from sqlite_master where name = 'table2';");
                Check.That(Convert.ToInt32(count2) == 1, "Expected to table2.");

                var count3 = Db.GetScalar(connection, "select count(1) from sqlite_master where name = 'table3';");
                Check.That(Convert.ToInt32(count3) == 1, "Expected to table3.");

                var count4 = Db.GetScalar(connection, "select count(1) from sqlite_master where name = 'table4';");
                Check.That(Convert.ToInt32(count4) == 1, "Expected to table4.");
            }
        }
    }
}
