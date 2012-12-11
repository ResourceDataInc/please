using Library.Bump.Tasks;
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

            // Act
            runCommand.Execute();

            // Assert
            Check.That(runCommand.BumpFiles.Stats.ExecuteCount == 1, "Expected BumpFiles to run.");
        }
    }
}
