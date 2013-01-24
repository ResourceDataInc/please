using System.Linq;
using Library.Bump;
using Library.Bump.Tasks;
using Library.Migrate.Tasks;
using Library.Models;
using NUnit.Framework;
using Simpler;

namespace Tests.Models
{
    [TestFixture]
    public class CommandsTests
    {
        [Test]
        public void should_bump_major_in_AssemblyInfo()
        {
            // Arrange
            var bumpType = BumpType.Unknown;
            var fileType = FileType.Unknown;
            var bumpCommand = Commands.All.Single(c => c.Name == "bump");
            bumpCommand.Task = Fake.Task<BumpFile>(bf =>
            {
                bumpType = bf.In.BumpType;
                fileType = bf.In.FileType;
            });
            const string options = "major in AssemblyInfo.cs";

            // Act
            bumpCommand.Run(options);

            // Assert
            Check.That(bumpType == BumpType.Major, "Expected bump type to be major.");
            Check.That(fileType == FileType.AssemblyInfo, "Expected file type to be AssemblyInfo.");
        }

        [Test]
        public void should_bump_minor_in_nuspec()
        {
            // Arrange
            var bumpType = BumpType.Unknown;
            var fileType = FileType.Unknown;
            var bumpCommand = Commands.All.Single(c => c.Name == "bump");
            bumpCommand.Task = Fake.Task<BumpFile>(bf =>
            {
                bumpType = bf.In.BumpType;
                fileType = bf.In.FileType;
            });
            const string options = "minor in Something.nuspec";

            // Act
            bumpCommand.Run(options);

            // Assert
            Check.That(bumpType == BumpType.Minor, "Expected bump type to be minor.");
            Check.That(fileType == FileType.Nuspec, "Expected file type to be nuspec.");
        }

        [Test]
        public void should_bump_patch_in_script()
        {
            // Arrange
            var bumpType = BumpType.Unknown;
            var fileType = FileType.Unknown;
            var bumpCommand = Commands.All.Single(c => c.Name == "bump");
            bumpCommand.Task = Fake.Task<BumpFile>(bf =>
            {
                bumpType = bf.In.BumpType;
                fileType = bf.In.FileType;
            });
            const string options = "patch in Whatever.bat";

            // Act
            bumpCommand.Run(options);

            // Assert
            Check.That(bumpType == BumpType.Patch, "Expected bump type to be patch.");
            Check.That(fileType == FileType.Script, "Expected file type to be script.");
        }

        [Test]
        public void can_run_sql_with_versioning()
        {
            // Arrange
            var withVersioning = false;
            var runSqlCommand = Commands.All.Single(c => c.Name == "run sql");
            runSqlCommand.Task = Fake.Task<RunSql>(rs => withVersioning = rs.In.WithVersioning);
            const string options = "with versioning";

            // Act
            runSqlCommand.Run(options);

            // Assert
            Check.That(withVersioning, "Expected with versioning to be set to true.");
        }

        [Test]
        public void can_run_sql_on_database()
        {
            // Arrange
            var database = "";
            var runSqlCommand = Commands.All.Single(c => c.Name == "run sql");
            runSqlCommand.Task = Fake.Task<RunSql>(rs => database = rs.In.Args[0]);
            const string options = "on DEV";

            // Act
            runSqlCommand.Run(options);

            // Assert
            Check.That(database == "DEV", "Expected database to be DEV.");
        }

        [Test]
        public void can_run_sql_on_database_in_directory()
        {
            // Arrange
            var database = "";
            var directory = "";
            var runSqlCommand = Commands.All.Single(c => c.Name == "run sql");
            runSqlCommand.Task = Fake.Task<RunSql>(rs =>
                                                       {
                                                           database = rs.In.Args[0];
                                                           directory = rs.In.Args[1];
                                                       });
            const string options = "on DEV in SomeDirectory";

            // Act
            runSqlCommand.Run(options);

            // Assert
            Check.That(database == "DEV", "Expected database to be DEV.");
            Check.That(directory == "SomeDirectory", "Expected directory to be SomeDirectory.");
        }
    }
}
