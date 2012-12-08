﻿using System;
using System.IO;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;
using Simpler.Data;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class RunDbScriptText
    {
        [Test]
        public void should_run_given_script()
        {
            // Arrange
            File.Delete(@"Migrate\Files\test.db");
            File.Copy(@"Migrate\Files\empty.db", @"Migrate\Files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runDbScript = Task.New<RunDbScript>();
            runDbScript.In.ConnectionName = "Test";
            runDbScript.In.FileName = @"Migrate\Files\insert-version.sql";

            // Act
            runDbScript.Execute();

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count = Db.GetScalar(connection, "select count(1) from schema_migrations;");
                Check.That(Convert.ToInt32(count) == 1, "Expected to find one version record in the database not {0}.", count);
            }
        }
    }
}
