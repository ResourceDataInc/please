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
            var bump = ShouldExecute<Bump>("bump major version in AssemblyInfo.cs");

            Check.That(bump.In.BumpType == BumpType.Major, "Expected bump type to be major.");
            Check.That(bump.In.FileType == FileType.AssemblyInfo, "Expected file type to be AssemblyInfo.");
        }

        [Test]
        public void should_bump_minor_in_nuspec()
        {
            var bump = ShouldExecute<Bump>("bump minor version in Something.nuspec");

            Check.That(bump.In.BumpType == BumpType.Minor, "Expected bump type to be minor.");
            Check.That(bump.In.FileType == FileType.Nuspec, "Expected file type to be nuspec.");
        }

        [Test]
        public void should_bump_patch_in_script()
        {
            var bump = ShouldExecute<Bump>("bump patch version in Whatever.bat");

            Check.That(bump.In.BumpType == BumpType.Patch, "Expected bump type to be patch.");
            Check.That(bump.In.FileType == FileType.Script, "Expected file type to be script.");
        }

        [Test]
        public void should_run_sql_on_database_in_directory()
        {
            var runSql = ShouldExecute<RunSql>("run sql in SomeDirectory on DEV ");

            Check.That(runSql.In.ConnectionName == "DEV", "Expected database to be DEV.");
            Check.That(runSql.In.Directory == "SomeDirectory", "Expected directory to be SomeDirectory.");
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
    }
}
