﻿using System.IO;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class FetchVersionsTest
    {
        [Test]
        public void should_fetch_installed_versions()
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
            runDbScript.Execute();

            var fetchVersions = Task.New<FetchVersions>();
            fetchVersions.In.ConnectionName = "Test";

            // Act
            fetchVersions.Execute();

            // Assert
            Check.That(fetchVersions.Out.Versions.Length == 1, "Expected to find 1 version.");
        }
    }
}