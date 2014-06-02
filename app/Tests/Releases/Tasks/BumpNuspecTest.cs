using System.IO;
using Library.Releases;
using Library.Releases.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Releases.Tasks
{
    [TestFixture]
    public class BumpNuspecTest
    {
        static void TestBump(BumpType bumpType, string fileContainingExpectedContents)
        {
            // Arrange
            File.Delete(Config.Releases.Files.Nuspec.After);
            File.Copy(Config.Releases.Files.Nuspec.Before, Config.Releases.Files.Nuspec.After);

            var bump = Task.New<BumpNuspec>();
            bump.In.FileName = Config.Releases.Files.Nuspec.After;
            bump.In.BumpType = bumpType;

            // Act
            bump.Execute();

            // Assert
            var afterContents = File.ReadAllText(Config.Releases.Files.Nuspec.After);
            var expectedContents = File.ReadAllText(fileContainingExpectedContents);
            Assert.That(afterContents, Is.EqualTo(expectedContents));
        }

        [Test]
        public void should_bump_major()
        {
            TestBump(BumpType.Major, Config.Releases.Files.Nuspec.MajorBumped);
        }

        [Test]
        public void should_bump_minor()
        {
            TestBump(BumpType.Minor, Config.Releases.Files.Nuspec.MinorBumped);
        }

        [Test]
        public void should_bump_patch()
        {
            TestBump(BumpType.Patch, Config.Releases.Files.Nuspec.PatchBumped);
        }
    }
}
