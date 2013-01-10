using System;
using System.IO;
using Library.Migrate;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;
using Simpler.Data;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class RunMigrationTest
    {
        [Test]
        public void should_run_given_migration_script()
        {
            // Arrange
            File.Delete(@"Migrate\files\test.db");
            File.Copy(@"Migrate\files\empty.db", @"Migrate\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runMigration = Task.New<RunMigration>();
            runMigration.In.ConnectionName = "Test";
            runMigration.In.Migration = new Migration {FileName = @"Migrate\files\insert-version.sql"};

            // Act
            runMigration.Execute();

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count = Db.GetScalar(connection, "select count(1) from schema_migrations;");
                Check.That(Convert.ToInt32(count) == 1, "Expected to find one version record in the database not {0}.", count);
            }
        }
    }
}
