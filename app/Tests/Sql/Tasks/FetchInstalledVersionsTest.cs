﻿using System;
using System.IO;
using Library.Sql;
using Library.Sql.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Sql.Tasks
{
    [TestFixture]
    public class FetchInstalledVersionsTest
    {
        [Test]
        public void should_fetch_installed_versions()
        {
            // Arrange
            File.Delete(@"Sql\files\test.db");
            File.Copy(@"Sql\files\empty.db", @"Sql\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runScripts = Task.New<RunScripts>();
            runScripts.In.ConnectionName = "Test";
            runScripts.In.Scripts = new[] {new Script {FileName = @"Sql\files\insert-version.sql"}};
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runScripts.Execute();
            }

            var fetchInstalledVersions = Task.New<FetchInstalledVersions>();
            fetchInstalledVersions.In.ConnectionName = "Test";

            // Act
            fetchInstalledVersions.Execute();

            // Assert
            Assert.That(fetchInstalledVersions.Out.Versions.Length, Is.EqualTo(1));
        }
    }
}
