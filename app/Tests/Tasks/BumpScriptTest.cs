using System;
using System.Globalization;
using System.IO;
using Library;
using Library.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Tasks
{
    [TestFixture]
    public class BumpScriptTest
    {
        const string BeforeFile = @".\Files\Script_Before.txt";
        const string AfterFile = @".\Files\Script_After.txt";
        const string MajorFile = @".\Files\Script_MajorBumped.txt";
        const string MinorFile = @".\Files\Script_MinorBumped.txt";
        const string PatchFile = @".\Files\Script_PatchBumped.txt";

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
