using Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Tasks
{
    [TestFixture]
    public class ParseArgsTest
    {
        [Test]
        public void should_find_major_bump_type_in_first_arg()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = new[] { "major" };

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.BumpType == BumpType.Major, "Should have found Major.");
        }

        [Test]
        public void should_find_minor_bump_type_in_first_arg()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = new[] { "minor" };

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.BumpType == BumpType.Minor, "Should have found Minor.");
        }

        [Test]
        public void should_find_patch_bump_type_in_first_arg()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = new[] { "patch" };

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.BumpType == BumpType.Patch, "Should have found Patch.");
        }
    }
}
