using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Migrate;
using Library.Migrate.Tasks;
using NUnit.Framework;
using Simpler;
using Version = Library.Migrate.Version;

namespace Tests.Migrate.Tasks
{
    [TestFixture]
    public class RunMissingMigrationsTest
    {
        readonly Version[] _testVersions =
            new[]
                {
                    new Version {Id = "001"},
                    new Version {Id = "002"},
                    new Version {Id = "003"}
                };

        readonly Migration[] _testMigrations =
            new[]
                {
                    new Migration {VersionId = "001", FileName = "001_first"},
                    new Migration {VersionId = "002", FileName = "002_second"},
                    new Migration {VersionId = "002", FileName = "002_third"},
                    new Migration {VersionId = "003", FileName = "003_fourth"}
                };

        [Test]
        public void should_run_all_migrations_if_no_versions_exist()
        {
            // Arrange
            var runMissingMigrations = Task.New<RunMissingMigrations>();
            runMissingMigrations.In.InstalledVersions = new Version[0];
            runMissingMigrations.In.Migrations = _testMigrations;
            runMissingMigrations.RunMigration = Fake.Task<RunMigration>();
            runMissingMigrations.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingMigrations.Execute();
            }

            // Assert
            Check.That(runMissingMigrations.RunMigration.Stats.ExecuteCount == _testMigrations.Length,
                "Expected {0} migration to be ran.", _testMigrations.Length);
        }

        [Test]
        public void should_not_run_any_migrations_if_all_versions_exist()
        {
            // Arrange
            var runMissingMigrations = Task.New<RunMissingMigrations>();
            runMissingMigrations.In.InstalledVersions = _testVersions;
            runMissingMigrations.In.Migrations = _testMigrations;
            runMissingMigrations.RunMigration = Fake.Task<RunMigration>();
            runMissingMigrations.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingMigrations.Execute();
            }

            // Assert
            Check.That(runMissingMigrations.RunMigration.Stats.ExecuteCount == 0,
                "Didn't expect any migration to be ran.");
        }

        [Test]
        public void should_run_all_migrations_for_a_missing_version()
        {
            // Arrange
            var migrationsRan = new List<string>();
            var runMissingMigrations = Task.New<RunMissingMigrations>();
            runMissingMigrations.In.InstalledVersions = _testVersions.Where(version => version.Id != "002").ToArray();
            runMissingMigrations.In.Migrations = _testMigrations;
            runMissingMigrations.RunMigration = Fake.Task<RunMigration>(task => migrationsRan.Add(task.In.Migration.VersionId));
            runMissingMigrations.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingMigrations.Execute();
            }

            // Assert
            Check.That(runMissingMigrations.RunMigration.Stats.ExecuteCount == 2, "Expected {0} migration to be ran.", 2);
            Check.That(migrationsRan[0] == "002", "First migration should have been for version 002");
            Check.That(migrationsRan[1] == "002", "Second migration should have been for version 002");
        }

        [Test]
        public void should_insert_version_for_each_distinct_version_belonging_to_migrations_ran()
        {
            // Arrange
            var migrationsRan = new List<string>();
            var runMissingMigrations = Task.New<RunMissingMigrations>();
            runMissingMigrations.In.InstalledVersions = _testVersions.Where(version => version.Id != "002").ToArray();
            runMissingMigrations.In.Migrations = _testMigrations;
            runMissingMigrations.RunMigration = Fake.Task<RunMigration>(task => migrationsRan.Add(task.In.Migration.VersionId));
            runMissingMigrations.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingMigrations.Execute();
            }

            // Assert
            Check.That(runMissingMigrations.RunMigration.Stats.ExecuteCount == 2, "Expected 2 migration to be ran.");
            Check.That(runMissingMigrations.InsertInstalledVersion.Stats.ExecuteCount == 1, "Expected 1 version to be inserted.");
        }

        [Test]
        public void should_run_migrations_in_alphabetical_order()
        {
            // Arrange
            var migrationsRan = new List<string>();
            var runMissingMigrations = Task.New<RunMissingMigrations>();
            runMissingMigrations.In.InstalledVersions = new Version[0];
            runMissingMigrations.In.Migrations = _testMigrations.Reverse().ToArray();
            runMissingMigrations.RunMigration = Fake.Task<RunMigration>(task => migrationsRan.Add(task.In.Migration.FileName));
            runMissingMigrations.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                runMissingMigrations.Execute();
            }

            // Assert
            Check.That(migrationsRan[0] == _testMigrations[0].FileName, "{0} should have ran first.", _testMigrations[0].FileName);
            Check.That(migrationsRan[1] == _testMigrations[1].FileName, "{0} should have ran second.", _testMigrations[1].FileName);
            Check.That(migrationsRan[2] == _testMigrations[2].FileName, "{0} should have ran third.", _testMigrations[2].FileName);
            Check.That(migrationsRan[3] == _testMigrations[3].FileName, "{0} should have ran fourth.", _testMigrations[3].FileName);
        }

        [Test]
        public void should_throw_if_migration_fails()
        {
            // Arrange
            var runMissingMigrations = Task.New<RunMissingMigrations>();
            runMissingMigrations.In.InstalledVersions = new Version[0];
            runMissingMigrations.In.Migrations = _testMigrations.Take(1).ToArray();
            runMissingMigrations.RunMigration = Fake.Task<RunMigration>(rm => { throw new Exception(); });
            runMissingMigrations.InsertInstalledVersion = Fake.Task<InsertInstalledVersion>();

            // Act & Assert
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                Check.Throws<MigrationException>(runMissingMigrations.Execute);
            }
        }
    }
}
