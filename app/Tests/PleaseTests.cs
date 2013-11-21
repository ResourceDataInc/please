using System;
using System.IO;
using Library;
using Library.Releases;
using Library.Releases.Tasks;
using Library.Scripts.Tasks;
using Library.Timestamp.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests
{
    [TestFixture]
    public class PleaseTests
    {
        static TTask ShouldExecute<TTask>(string commandText) where TTask : Task
        {
            foreach (var command in Commands.All)
            {
                command.Task = Fake.Task<TTask>();
            }

            var please = Task.New<Please>();
            please.In.Args = commandText.Split(' ');
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                please.Execute();
            }

            var task = please.Out.Command.Task as TTask;
            if (task == null) throw new Exception("Unexpected command task was found.");
            return task;
        }

        [Test]
        public void should_bump_major_in_AssemblyInfo()
        {
            var files =
                new[]
                    {
                        @"AssemblyInfo.cs",
                        @"SomeDirectory\AssemblyInfo.cs",
                        @".\AssemblyInfo.cs",
                        @".\Some\Directory\AssemblyInfo.cs",
                        @"\\AssemblyInfo.cs",
                        @"\\Some\Directory\AssemblyInfo.cs",
                        @"c:\AssemblyInfo.cs",
                        @"c:\Some\Directory\AssemblyInfo.cs"
                    };

            foreach (var file in files)
            {
                var commandText = String.Format("bump major version in {0}", file);
                var bump = ShouldExecute<Bump>(commandText);

                Assert.That(bump.Stats.ExecuteCount, Is.EqualTo(1));
                Assert.That(bump.In.BumpType, Is.EqualTo(BumpType.Major));
                Assert.That(bump.In.FileType, Is.EqualTo(FileType.AssemblyInfo));
                Assert.That(bump.In.FileName, Is.EqualTo(file));
            }
        }

        [Test]
        public void should_bump_minor_in_nuspec()
        {
            var files =
                new[]
                    {
                        @"Something.nuspec",
                        @"SomeDirectory\Something.nuspec",
                        @".\Something.nuspec",
                        @".\Some\Directory\Something.nuspec",
                        @"\\Something.nuspec",
                        @"\\Some\Directory\Something.nuspec",
                        @"c:\Something.nuspec",
                        @"c:\Some\Directory\Something.nuspec"
                    };


            foreach (var file in files)
            {
                var commandText = String.Format("bump minor version in {0}", file);
                var bump = ShouldExecute<Bump>(commandText);

                Assert.That(bump.Stats.ExecuteCount, Is.EqualTo(1));
                Assert.That(bump.In.BumpType, Is.EqualTo(BumpType.Minor));
                Assert.That(bump.In.FileType, Is.EqualTo(FileType.Nuspec));
                Assert.That(bump.In.FileName, Is.EqualTo(file));
            }
       }

        [Test]
        public void should_bump_patch_in_script()
        {
            var files =
                new[]
                    {
                        @"Whatever.bat",
                        @"SomeDirectory\Whatever.bat",
                        @".\Whatever.bat",
                        @".\Some\Directory\Whatever.bat",
                        @"\\Whatever.bat",
                        @"\\Some\Directory\Whatever.bat",
                        @"c:\Whatever.bat",
                        @"c:\Some\Directory\Whatever.bat"
                    };


            foreach (var file in files)
            {
                var commandText = String.Format("bump patch version in {0}", file);
                var bump = ShouldExecute<Bump>(commandText);

                Assert.That(bump.Stats.ExecuteCount, Is.EqualTo(1));
                Assert.That(bump.In.BumpType, Is.EqualTo(BumpType.Patch));
                Assert.That(bump.In.FileType, Is.EqualTo(FileType.Script));
                Assert.That(bump.In.FileName, Is.EqualTo(file));
            }
         }

        [Test]
        public void should_run_sql_with_versioning()
        {
            var run = ShouldExecute<Run>("run sql with versioning");

            Assert.That(run.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(run.In.WithVersioning, "Expected with versioning to be true.");
        }

        [Test]
        public void should_run_sql_without_versioning()
        {
            var run = ShouldExecute<Run>("run sql");

            Assert.That(run.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(!run.In.WithVersioning, "Expected with versioning to be false.");
        }

        [Test]
        public void should_run_sql_on_database()
        {
            var run = ShouldExecute<Run>("run sql on DEV");

            Assert.That(run.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(run.In.ConnectionName, Is.EqualTo("DEV"));
        }

        [Test]
        public void should_run_sql_in_directory()
        {
            var directories =
                new[]
                    {
                        @"SomeDirectory",
                        @"Some\Directory",
                        @".\SomeDirectory",
                        @"\\SomeDirectory",
                        @"c:\SomeDirectory",
                        @"Some Directory",
                        @".\Some Directory",
                        @"c:\Some Directory"
                    };

            foreach (var directory in directories)
            {
                var commandText = String.Format("run sql in {0} on DEV", directory);
                var run = ShouldExecute<Run>(commandText);

                Assert.That(run.Stats.ExecuteCount, Is.EqualTo(1));
                Assert.That(run.In.Directory, Is.EqualTo(directory));
            }
        }

        [Test]
        public void should_run_sql_file()
        {
            var directories =
                new[]
                    {
                        @"SomeDirectory\test.sql",
                        @"Some\Directory\test.sql",
                        @".\SomeDirectory\test.sql",
                        @"\\SomeDirectory\test.sql",
                        @"c:\SomeDirectory\test.sql",
                        @"Some Directory\test.sql",
                        @".\Some Directory\test.sql",
                        @"c:\Some Directory\test.sql"
                    };

            foreach (var directory in directories)
            {
                var commandText = String.Format("run sql file {0} on DEV", directory);
                var run = ShouldExecute<Run>(commandText);

                Assert.That(run.Stats.ExecuteCount, Is.EqualTo(1));
                Assert.That(run.In.File, Is.EqualTo(directory));
            }
        }

        [Test]
        public void should_run_sql_include_whitelist_in_directory()
        {
            const string whitelistFile = @".\whitelist.txt";
            var run = ShouldExecute<Run>(@"run sql include " + whitelistFile + @" in .\Directory");

            Assert.That(run.Stats.ExecuteCount, Is.EqualTo(1));
            Assert.That(run.In.WhitelistFile, Is.EqualTo(whitelistFile));
        }

        [Test]
        public void should_add_timestamp_in_directory()
        {
            var directories =
                new[]
                    {
                        @"SomeDirectory",
                        @"Some\Directory",
                        @".\SomeDirectory",
                        @"\\SomeDirectory",
                        @"c:\SomeDirectory",
                        @"Some Directory",
                        @".\Some Directory",
                        @"c:\Some Directory"
                    };

            foreach (var directory in directories)
            {
                var commandText = String.Format("add timestamp in {0}", directory);
                var addTimestamp = ShouldExecute<AddTimestamp>(commandText);

                Assert.That(addTimestamp.Stats.ExecuteCount, Is.EqualTo(1));
                Assert.That(addTimestamp.In.Directory, Is.EqualTo(directory));
            }
        }

        [Test]
        public void should_return_0_on_success()
        {
            foreach (var command in Commands.All)
            {
                command.Task = Fake.Task<Run>();
            }

            var please = Task.New<Please>();
            please.In.Args = "run sql".Split(' ');
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                please.Execute();
            }

            Assert.That(please.Out.ExitCode, Is.EqualTo(0));
        }

        [Test]
        public void should_return_1_on_failure()
        {
            foreach (var command in Commands.All)
            {
                command.Task = Fake.Task<Run>();
            }

            var please = Task.New<Please>();
            please.In.Args = "this is wrong".Split(' ');
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                please.Execute();
            }

            Assert.That(please.Out.ExitCode, Is.EqualTo(1));
        }
    }
}
