using System;
using System.IO;
using Library.Sql.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Sql.Tasks
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
            run.In.Arguments = @"Sql\files\py\1_hello.py";

            // Act
            string output;
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                run.Execute();
                output = sw.ToString();
            }

            // Assert
            Assert.That(run.Out.ExitCode, Is.EqualTo(0));
            Assert.That(output, Is.EqualTo("Hello World.\r\n"));
        }

        [Test]
        public void should_capture_python_errors()
        {
            // Arrange
            var run = Task.New<RunProcess>();
            run.In.FileName = "python";
            run.In.Arguments = @"Sql\files\py\2_error.py";

            // Act
            string output;
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                run.Execute();
                output = sw.ToString();
            }

            // Assert
            Assert.That(run.Out.ExitCode, Is.Not.EqualTo(0));
            Assert.That(output.Contains("Error!"));
        }
    }
}
