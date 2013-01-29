using System;
using Library;
using Library.Releases;
using Library.Releases.Tasks;
using Library.Sql.Tasks;
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
            please.Execute();

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

                Assert.That(bump.In.BumpType, Is.EqualTo(BumpType.Patch));
                Assert.That(bump.In.FileType, Is.EqualTo(FileType.Script));
                Assert.That(bump.In.FileName, Is.EqualTo(file));
            }
         }

        [Test]
        public void should_run_sql_with_versioning()
        {
            var runSql = ShouldExecute<RunSql>("run sql with versioning");

            Check.That(runSql.In.WithVersioning, "Expected with versioning to be true.");
        }

        [Test]
        public void should_run_sql_without_versioning()
        {
            var runSql = ShouldExecute<RunSql>("run sql");

            Check.That(!runSql.In.WithVersioning, "Expected with versioning to be false.");
        }

        [Test]
        public void should_run_sql_on_database()
        {
            var runSql = ShouldExecute<RunSql>("run sql on DEV");

            Check.That(runSql.In.ConnectionName == "DEV", "Expected database to be DEV.");
        }

        [Test]
        public void should_run_sql_in_directory()
        {
            var directories =
                new[]
                    {
                        @"SomeDirectory",
                        @"Some\Directory",
                        @"SomeDirectory\",
                        @".\SomeDirectory",
                        @".\Some\Directory\",
                        @"\\SomeDirectory",
                        @"\\Some\Directory\",
                        @"c:\SomeDirectory",
                        @"c:\Some\Directory\",
                        @"Some Directory",
                        @"Some Directory\",
                        @".\Some Directory",
                        @"c:\Some Directory"
                    };

            foreach (var directory in directories)
            {
                var commandText = String.Format("run sql in {0} on DEV", directory);
                var runSql = ShouldExecute<RunSql>(commandText);

                Assert.That(runSql.In.Directory, Is.EqualTo(directory));
            }
        }

    }
}
