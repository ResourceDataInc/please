using System.IO;
using Library.Releases;
using Library.Releases.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Releases.Tasks
{
    [TestFixture]
    public class BumpAssemblyInfoTest
    {
        static void TestBump(BumpType bumpType, string fileContainingExpectedContents)
        {
            // Arrange
            File.Delete(Config.Releases.Files.AssemblyInfo.After);
            File.Copy(Config.Releases.Files.AssemblyInfo.Before, Config.Releases.Files.AssemblyInfo.After);

            var bump = Task.New<BumpAssemblyInfo>();
            bump.In.FileName = Config.Releases.Files.AssemblyInfo.After;
            bump.In.BumpType = bumpType;

            // Act
            bump.Execute();

            // Assert
            var afterContents = File.ReadAllText(Config.Releases.Files.AssemblyInfo.After);
            var expectedContents = File.ReadAllText(fileContainingExpectedContents);
            Assert.That(afterContents, Is.EqualTo(expectedContents));
        }

        [Test]
        public void should_bump_major()
        {
            TestBump(BumpType.Major, Config.Releases.Files.AssemblyInfo.MajorBumped);
        }

        [Test]
        public void should_bump_minor()
        {
            TestBump(BumpType.Minor, Config.Releases.Files.AssemblyInfo.MinorBumped);
        }

        [Test]
        public void should_bump_patch()
        {
            TestBump(BumpType.Patch, Config.Releases.Files.AssemblyInfo.PatchBumped);
        }
    }
}
