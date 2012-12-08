﻿using System;
using System.IO;
using Library.Bump;
using Library.Bump.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Bump.Tasks
{
    [TestFixture]
    public class BumpAssemblyInfoTest
    {
        const string BeforeFile = @".\Bump\Files\AssemblyInfo_Before.cs";
        const string AfterFile = @".\Bump\Files\AssemblyInfo_After.cs";
        const string MajorFile = @".\Bump\Files\AssemblyInfo_MajorBumped.cs";
        const string MinorFile = @".\Bump\Files\AssemblyInfo_MinorBumped.cs";
        const string PatchFile = @".\Bump\Files\AssemblyInfo_PatchBumped.cs";

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
