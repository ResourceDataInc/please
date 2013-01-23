using System.Linq;
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
