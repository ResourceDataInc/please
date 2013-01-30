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
        const string BeforeFile = @".\Releases\files\AssemblyInfo_Before.cs";
        const string AfterFile = @".\Releases\files\AssemblyInfo_After.cs";
        const string MajorFile = @".\Releases\files\AssemblyInfo_MajorBumped.cs";
        const string MinorFile = @".\Releases\files\AssemblyInfo_MinorBumped.cs";
        const string PatchFile = @".\Releases\files\AssemblyInfo_PatchBumped.cs";

        static void TestBump(BumpType bumpType, string fileContainingExpectedContents)
        {
            // Arrange
            File.Delete(AfterFile);
            File.Copy(BeforeFile, AfterFile);

            var bump = Task.New<BumpAssemblyInfo>();
            bump.In.FileName = AfterFile;
            bump.In.BumpType = bumpType;

            // Act
            bump.Execute();

            // Assert
            var afterContents = File.ReadAllText(AfterFile);
            var expectedContents = File.ReadAllText(fileContainingExpectedContents);
            Assert.That(afterContents, Is.EqualTo(expectedContents));
        }

        [Test]
        public void should_bump_major()
        {
            TestBump(BumpType.Major, MajorFile);
        }

        [Test]
        public void should_bump_minor()
        {
            TestBump(BumpType.Minor, MinorFile);
        }

        [Test]
        public void should_bump_patch()
        {
            TestBump(BumpType.Patch, PatchFile);
        }
    }
}
