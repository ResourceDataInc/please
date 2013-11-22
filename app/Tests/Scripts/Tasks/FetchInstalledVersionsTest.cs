using System;
using System.IO;
using Library.Scripts;
using Library.Scripts.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Scripts.Tasks
{
    [TestFixture]
    public class FetchInstalledVersionsTest
    {
        [Test]
        public void should_fetch_installed_versions()
        {
            // Arrange
            File.Delete(@"Scripts\files\test.db");
            File.Copy(@"Scripts\files\empty.db", @"Scripts\files\test.db");

            var createVersionTable = Task.New<CreateVersionTable>();
            createVersionTable.In.ConnectionName = "Test";
            createVersionTable.Execute();

            var runScripts = Task.New<RunScripts>();
            runScripts.In.ConnectionName = "Test";
            runScripts.In.Scripts = new[] {new Script {FileName = @"Scripts\files\sql\insert-version.sql"}};
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
