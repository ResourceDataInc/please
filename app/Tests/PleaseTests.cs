using System.Linq;
using Library;
using Library.Bump;
using Library.Bump.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests
{
    [TestFixture]
    public class PleaseTests
    {
        [Test]
        public void should_bump_major_in_AssemblyInfo()
        {
            // Arrange
            var please = Task.New<Please>();
            please.In.Args = "bump major in AssemblyInfo.cs".Split(' ');

            var bumpType = BumpType.Unknown;
            var fileType = FileType.Unknown;
            var bumpCommand = Commands.All.Single(c => c.Name == "bump");
            bumpCommand.Task = Fake.Task<BumpFile>(bf =>
            {
                bumpType = bf.In.BumpType;
                fileType = bf.In.FileType;
            });

            // Act
            please.Execute();

            // Assert
            Check.That(bumpType == BumpType.Major, "Expected bump type to be major.");
            Check.That(fileType == FileType.AssemblyInfo, "Expected file type to be AssemblyInfo.");
        }

        [Test]
        public void should_bump_minor_in_nuspec()
        {
            // Arrange
            var please = Task.New<Please>();
            please.In.Args = "bump minor in Something.nuspec".Split(' ');

            var bumpType = BumpType.Unknown;
            var fileType = FileType.Unknown;
            var bumpCommand = Commands.All.Single(c => c.Name == "bump");
            bumpCommand.Task = Fake.Task<BumpFile>(bf =>
            {
                bumpType = bf.In.BumpType;
                fileType = bf.In.FileType;
            });

            // Act
            please.Execute();

            // Assert
            Check.That(bumpType == BumpType.Minor, "Expected bump type to be minor.");
            Check.That(fileType == FileType.Nuspec, "Expected file type to be nuspec.");
        }

        [Test]
        public void should_bump_patch_in_script()
        {
            // Arrange
            var please = Task.New<Please>();
            please.In.Args = "bump patch in Whatever.bat".Split(' ');

            var bumpType = BumpType.Unknown;
            var fileType = FileType.Unknown;
            var bumpCommand = Commands.All.Single(c => c.Name == "bump");
            bumpCommand.Task = Fake.Task<BumpFile>(bf =>
            {
                bumpType = bf.In.BumpType;
                fileType = bf.In.FileType;
            });
 
            // Act
            please.Execute();

            // Assert
            Check.That(bumpType == BumpType.Patch, "Expected bump type to be patch.");
            Check.That(fileType == FileType.Script, "Expected file type to be script.");
        }

        [Test]
        public void should_run_sql_on_database_in_directory()
        {
            // Arrange
            var please = Task.New<Please>();
            please.In.Args = "run sql on DEV in SomeDirectory".Split(' ');

            var database = "";
            var directory = "";
            var runSqlCommand = Commands.All.Single(c => c.Name == "run sql");
            runSqlCommand.Task = Fake.Task<Library.RunSql.Tasks.RunSql>(rs =>
                                                       {
                                                           database = rs.In.Args[0];
                                                           directory = rs.In.Args[1];
                                                       });

            // Act
            please.Execute();

            // Assert
            Check.That(database == "DEV", "Expected database to be DEV.");
            Check.That(directory == "SomeDirectory", "Expected directory to be SomeDirectory.");
        }

        [Test]
        public void should_run_sql_with_versioning()
        {
            // Arrange
            var please = Task.New<Please>();
            please.In.Args = "run sql with versioning".Split(' ');

            var withVersioning = false;
            var runSqlCommand = Commands.All.Single(c => c.Name == "run sql");
            runSqlCommand.Task = Fake.Task<Library.RunSql.Tasks.RunSql>(rs => withVersioning = rs.In.WithVersioning);

            // Act
            please.Execute();

            // Assert
            Check.That(withVersioning, "Expected with versioning to be set to true.");
        }
    }
}
