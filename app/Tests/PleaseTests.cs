using System;
using Library;
using Library.Bump;
using Library.Bump.Tasks;
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
            var bumpFile = ShouldExecute<BumpFile>("bump major in AssemblyInfo.cs");

            Check.That(bumpFile.In.BumpType == BumpType.Major, "Expected bump type to be major.");
            Check.That(bumpFile.In.FileType == FileType.AssemblyInfo, "Expected file type to be AssemblyInfo.");
        }

        [Test]
        public void should_bump_minor_in_nuspec()
        {
            var bumpFile = ShouldExecute<BumpFile>("bump minor in Something.nuspec");

            Check.That(bumpFile.In.BumpType == BumpType.Minor, "Expected bump type to be minor.");
            Check.That(bumpFile.In.FileType == FileType.Nuspec, "Expected file type to be nuspec.");
        }

        [Test]
        public void should_bump_patch_in_script()
        {
            var bumpFile = ShouldExecute<BumpFile>("bump patch in Whatever.bat");

            Check.That(bumpFile.In.BumpType == BumpType.Patch, "Expected bump type to be patch.");
            Check.That(bumpFile.In.FileType == FileType.Script, "Expected file type to be script.");
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
