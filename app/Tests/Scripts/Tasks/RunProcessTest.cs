using System;
using System.IO;
using Library.Scripts;
using Library.Scripts.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Scripts.Tasks
{
    [TestFixture]
    public class RunProcessTest
    {
        [Test]
        public void should_run_python()
        {
            // Arrange
            var run = Task.New<RunProcess>();
            run.In.FileName = "python";
            run.In.Arguments = Config.Scripts.Files.Py.Hello;

            // Act
            string output;
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                run.Execute();
                output = sw.ToString();
            }

            // Assert
            Assert.That(output.Contains("Hello World."));
        }

        [Test]
        public void should_capture_python_errors()
        {
            // Arrange
            var run = Task.New<RunProcess>();
            run.In.FileName = "python";
            run.In.Arguments = Config.Scripts.Files.Py.Error;

            // Act
            string output;
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                Assert.Throws<RunException>(run.Execute);
                output = sw.ToString();
            }

            // Assert
            Assert.That(output.Contains("Error!"));
        }
    }
}
