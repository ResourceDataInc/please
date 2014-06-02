using System.IO;
using Library.Releases;
using Library.Releases.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Releases.Tasks
{
    [TestFixture]
    public class BumpScriptTest
    {

        static void TestBump(BumpType bumpType, string fileContainingExpectedContents)
        {
            // Arrange
            File.Delete(Config.Releases.Files.Script.After);
            File.Copy(Config.Releases.Files.Script.Before, Config.Releases.Files.Script.After);

            var bump = Task.New<BumpScript>();
            bump.In.FileName = Config.Releases.Files.Script.After;
            bump.In.BumpType = bumpType;

            // Act
            bump.Execute();

            // Assert
            var afterContents = File.ReadAllText(Config.Releases.Files.Script.After);
            var expectedContents = File.ReadAllText(fileContainingExpectedContents);
            Assert.That(afterContents, Is.EqualTo(expectedContents));
        }

        [Test]
        public void should_bump_major()
        {
            TestBump(BumpType.Major, Config.Releases.Files.Script.MajorBumped);
        }

        [Test]
        public void should_bump_minor()
        {
            TestBump(BumpType.Minor, Config.Releases.Files.Script.MinorBumped);
        }

        [Test]
        public void should_bump_patch()
        {
            TestBump(BumpType.Patch, Config.Releases.Files.Script.PatchBumped);
        }
    }
}
