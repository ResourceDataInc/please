using System;
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
        const string BeforeFile = @".\Releases\files\Script_Before.txt";
        const string AfterFile = @".\Releases\files\Script_After.txt";
        const string MajorFile = @".\Releases\files\Script_MajorBumped.txt";
        const string MinorFile = @".\Releases\files\Script_MinorBumped.txt";
        const string PatchFile = @".\Releases\files\Script_PatchBumped.txt";

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
