using System;
using System.IO;
using Library;
using Library.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Tasks
{
    [TestFixture]
    public class BumpAssemblyInfoTest
    {
        const string BeforeFile = @".\Files\AssemblyInfo_Before.cs";
        const string AfterFile = @".\Files\AssemblyInfo_After.cs";
        const string MajorFile = @".\Files\AssemblyInfo_MajorBumped.cs";
        const string MinorFile = @".\Files\AssemblyInfo_MinorBumped.cs";
        const string PatchFile = @".\Files\AssemblyInfo_PatchBumped.cs";

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
