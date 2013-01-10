using System;
using System.IO;
using Library.Bump;
using Library.Bump.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Bump.Tasks
{
    [TestFixture]
    public class BumpNuspecTest
    {
        const string BeforeFile = @".\Bump\files\Nuspec_Before.nuspec";
        const string AfterFile = @".\Bump\files\Nuspec_After.nuspec";
        const string MajorFile = @".\Bump\files\Nuspec_MajorBumped.nuspec";
        const string MinorFile = @".\Bump\files\Nuspec_MinorBumped.nuspec";
        const string PatchFile = @".\Bump\files\Nuspec_PatchBumped.nuspec";

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
            Check.That(String.Compare(afterContents, expectedContents) == 0, "Version was not bumped correctly.");
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
