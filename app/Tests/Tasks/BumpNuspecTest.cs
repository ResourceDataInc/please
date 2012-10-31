using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using Simpler;
using Tasks;

namespace Tests.Tasks
{
    [TestFixture]
    public class BumpNuspecTest
    {
        const string BeforeFile = @".\Files\Nuspec_Before.nuspec";
        const string AfterFile = @".\Files\Nuspec_After.nuspec";
        const string MajorFile = @".\Files\Nuspec_MajorBumped.nuspec";
        const string MinorFile = @".\Files\Nuspec_MinorBumped.nuspec";
        const string PatchFile = @".\Files\Nuspec_PatchBumped.nuspec";

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
