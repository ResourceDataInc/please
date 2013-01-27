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
        public void should_run_given_migration_script()
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
    }
}
