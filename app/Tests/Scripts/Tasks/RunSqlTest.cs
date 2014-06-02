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
            Database.Restore();

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = Database.Name;
            createVersionTable.Execute();

            var runScripts = Task.New<RunSql>();
            runScripts.In.ConnectionName = Database.Name;
            runScripts.In.Sql = new Script { FileName = Config.Scripts.Files.Sql.InsertVersion };

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runScripts.Execute();
            }

            // Assert
            using (var connection = Db.Connect(Database.Name))
            {
                var count = Db.GetScalar(connection, "select count(1) from db_version;");
                Assert.That(Convert.ToInt32(count), Is.EqualTo(1));
            }
        }

        [Test]
        public void should_capture_sql_errors()
        {
            // Arrange
            Database.Restore();

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = Database.Name;
            createVersionTable.Execute();

            var runScripts = Task.New<RunSql>();
            runScripts.In.ConnectionName = Database.Name;
            runScripts.In.Sql = new Script { FileName = Config.Scripts.Files.Sql.Error };

            // Act & Assert
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                Assert.Throws<RunException>(runScripts.Execute);
            }
        }

        [Test]
        public void should_split_sql_if_it_contains_GO_keyword()
        {
            // Arrange
            Database.Restore();

            var runScripts = Task.New<RunSql>();
            runScripts.In.ConnectionName = Database.Name;
            runScripts.In.Sql = new Script{ FileName = Config.Scripts.Files.Sql.Go.CreateFourTables };

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runScripts.Execute();
            }

            // Assert
            using (var connection = Db.Connect(Database.Name))
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
