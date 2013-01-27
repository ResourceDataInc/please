using System;
using System.Linq;
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
        static BumpFile PleaseBump(string options)
        {
            var please = Task.New<Please>();
            please.In.Args = ("bump " + options).Split(' ');

            var bumpCommand = Commands.All.Single(c => c.Name == "bump");
            bumpCommand.Task = Fake.Task<BumpFile>();

            please.Execute();

            var bumpFile = bumpCommand.Task as BumpFile;
            if (bumpFile == null) throw new Exception();
            return bumpFile;
        }

        static RunSql PleaseRunSql(string options)
        {
            var please = Task.New<Please>();
            please.In.Args = ("run sql " + options).Split(' ');

            var runSqlCommand = Commands.All.Single(c => c.Name == "run sql");
            runSqlCommand.Task = Fake.Task<RunSql>();

            please.Execute();

            var runSql = runSqlCommand.Task as RunSql;
            if (runSql == null) throw new Exception();
            return runSql;
        }

        [Test]
        public void should_bump_major_in_AssemblyInfo()
        {
            var bumpFile = PleaseBump("major in AssemblyInfo.cs");

            Check.That(bumpFile.In.BumpType == BumpType.Major, "Expected bump type to be major.");
            Check.That(bumpFile.In.FileType == FileType.AssemblyInfo, "Expected file type to be AssemblyInfo.");
        }

        [Test]
        public void should_bump_minor_in_nuspec()
        {
            var bumpFile = PleaseBump("minor in Something.nuspec");

            Check.That(bumpFile.In.BumpType == BumpType.Minor, "Expected bump type to be minor.");
            Check.That(bumpFile.In.FileType == FileType.Nuspec, "Expected file type to be nuspec.");
        }

        [Test]
        public void should_bump_patch_in_script()
        {
            var bumpFile = PleaseBump("patch in Whatever.bat");

            Check.That(bumpFile.In.BumpType == BumpType.Patch, "Expected bump type to be patch.");
            Check.That(bumpFile.In.FileType == FileType.Script, "Expected file type to be script.");
        }

        [Test]
        public void should_run_sql_on_database_in_directory()
        {
            var runSql = PleaseRunSql("on DEV in SomeDirectory");

            Check.That(runSql.In.Args[0] == "DEV", "Expected database to be DEV.");
            Check.That(runSql.In.Args[1] == "SomeDirectory", "Expected directory to be SomeDirectory.");
        }

        [Test]
        public void should_run_sql_with_versioning()
        {
            var runSql = PleaseRunSql("with versioning");

            Check.That(runSql.In.WithVersioning, "Expected with versioning to be set to true.");
        }
    }
}
