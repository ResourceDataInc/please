using Library.Bump.Tasks;
using Library.Migrate.Tasks;
using Library.Please.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Please
{
    [TestFixture]
    public class RunCommandTest
    {
        [Test]
        public void should_run_bump()
        {
            // Arrange
            var runCommand = Task.New<RunCommand>();
            runCommand.In.Args = new[] {"bump"};
            runCommand.BumpFiles = Fake.Task<BumpFiles>();
            runCommand.MigrateDatabase = Fake.Task<MigrateDatabase>();

            // Act
            runCommand.Execute();

            // Assert
            Check.That(runCommand.BumpFiles.Stats.ExecuteCount == 1, "Expected BumpFiles to run.");
        }

        [Test]
        public void should_run_migrate()
        {
            // Arrange
            var runCommand = Task.New<RunCommand>();
            runCommand.In.Args = new[] {"migrate"};
            runCommand.BumpFiles = Fake.Task<BumpFiles>();
            runCommand.MigrateDatabase = Fake.Task<MigrateDatabase>();

            // Act
            runCommand.Execute();

            // Assert
            Check.That(runCommand.MigrateDatabase.Stats.ExecuteCount == 1, "Expected MigrateDatabase to run.");
        }
    }
}
