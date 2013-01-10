using System;
using System.IO;
using Library.Bump;
using Library.Bump.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Bump.Tasks
{
    [TestFixture]
    public class BumpScriptTest
    {
        const string BeforeFile = @".\Bump\files\Script_Before.txt";
        const string AfterFile = @".\Bump\files\Script_After.txt";
        const string MajorFile = @".\Bump\files\Script_MajorBumped.txt";
        const string MinorFile = @".\Bump\files\Script_MinorBumped.txt";
        const string PatchFile = @".\Bump\files\Script_PatchBumped.txt";

        static void TestBump(BumpType bumpType, string fileContainingExpectedContents)
        {
            // Arrange
            File.Delete(AfterFile);
            File.Copy(BeforeFile, AfterFile);

            var bump = Task.New<BumpScript>();
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
