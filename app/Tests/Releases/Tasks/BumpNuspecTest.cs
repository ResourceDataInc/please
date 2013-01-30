using System;
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
        const string BeforeFile = @".\Releases\files\Nuspec_Before.nuspec";
        const string AfterFile = @".\Releases\files\Nuspec_After.nuspec";
        const string MajorFile = @".\Releases\files\Nuspec_MajorBumped.nuspec";
        const string MinorFile = @".\Releases\files\Nuspec_MinorBumped.nuspec";
        const string PatchFile = @".\Releases\files\Nuspec_PatchBumped.nuspec";

        static void TestBump(BumpType bumpType, string fileContainingExpectedContents)
        {
            // Arrange
            File.Delete(AfterFile);
            File.Copy(BeforeFile, AfterFile);

            var bump = Task.New<BumpNuspec>();
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
