﻿using System;
using System.IO;
using Library.RunSql.Tasks;
using NUnit.Framework;
using Simpler;
using Simpler.Data;
using Version = Library.RunSql.Version;

namespace Tests.RunSql.Tasks
{
    [TestFixture]
    public class InsertInstalledVersionTest
    {
        [Test]
        public void should_insert_version()
        {
            // Arrange
            File.Delete(@"RunSql\files\test.db");
            File.Copy(@"RunSql\files\empty.db", @"RunSql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var insertInstalledVersion = Task.New<InsertInstalledVersion>();
            insertInstalledVersion.In.ConnectionName = "Test";
            insertInstalledVersion.In.Version = new Version { Id = "999" };

            // Act
            insertInstalledVersion.Execute();

            // Assert
            Check.That(insertInstalledVersion.Out.RowsAffected == 1, "Expected to insert 1 version.");
        }

        [Test]
        public void should_insert_version_as_string()
        {
            // Arrange
            File.Delete(@"RunSql\files\test.db");
            File.Copy(@"RunSql\files\empty.db", @"RunSql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var insertInstalledVersion = Task.New<InsertInstalledVersion>();
            insertInstalledVersion.In.ConnectionName = "Test";
            insertInstalledVersion.In.Version = new Version { Id = "01" };

            // Act
            insertInstalledVersion.Execute();

            // Assert
            using (var connection = Db.Connect("Test"))
            {
                var count = Db.GetScalar(connection, "select version from db_version where version = '01';");
                Check.That(Convert.ToInt32(count) == 1, "Expected to find 1 record.");
            }
        }
    }
}