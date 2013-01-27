using Library.Releases;
using Library.Releases.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Releases.Tasks
{
    [TestFixture]
    public class BumpFileTest
    {
        [Test]
        public void should_bump_given_nuspec_file()
        {
            // Arrange
            var bump = Task.New<Bump>();
            bump.In.BumpType = BumpType.Patch;
            bump.In.FileType = FileType.Nuspec;
            bump.In.FileName = @"C:\Somewhere\Foo.nuspec";
            bump.BumpAssemblyInfo = Fake.Task<BumpAssemblyInfo>();
            bump.BumpNuspec = Fake.Task<BumpNuspec>();
            bump.BumpScript = Fake.Task<BumpScript>();

            // Act
            bump.Execute();

            // Assert
            Check.That(bump.BumpNuspec.Stats.ExecuteCount == 1, "Should have bumped the .nuspec file.");
            Check.That(bump.BumpAssemblyInfo.Stats.ExecuteCount == 0, "AssemblyInfo file was not provided.");
            Check.That(bump.BumpScript.Stats.ExecuteCount == 0, "Script file was not provided.");
        }

        [Test]
        public void should_bump_given_AssemblyInfo_file()
        {
            // Arrange
            var bump = Task.New<Bump>();
            bump.In.BumpType = BumpType.Patch;
            bump.In.FileType = FileType.AssemblyInfo;
            bump.In.FileName = @"C:\Somewhere\AssemblyInfo.cs";
            bump.BumpAssemblyInfo = Fake.Task<BumpAssemblyInfo>();
            bump.BumpNuspec = Fake.Task<BumpNuspec>();
            bump.BumpScript = Fake.Task<BumpScript>();

            // Act
            bump.Execute();

            // Assert
            Check.That(bump.BumpAssemblyInfo.Stats.ExecuteCount == 1, "Should have bumped 2 AssemblyInfo files.");
            Check.That(bump.BumpNuspec.Stats.ExecuteCount == 0, ".nuspec files was not provided.");
            Check.That(bump.BumpScript.Stats.ExecuteCount == 0, "Script files was not provided.");
        }

        [Test]
        public void should_bump_given_script_file()
        {
            // Arrange
            var bump = Task.New<Bump>();
            bump.In.BumpType = BumpType.Patch;
            bump.In.FileType = FileType.Script;
            bump.In.FileName = @"C:\SomewhereElse\scriptC.exe";
            bump.BumpAssemblyInfo = Fake.Task<BumpAssemblyInfo>();
            bump.BumpNuspec = Fake.Task<BumpNuspec>();
            bump.BumpScript = Fake.Task<BumpScript>();

            // Act
            bump.Execute();

            // Assert
            Check.That(bump.BumpScript.Stats.ExecuteCount == 1, "Should have bumped the script file.");
            Check.That(bump.BumpNuspec.Stats.ExecuteCount == 0, ".nuspec files was not provided.");
            Check.That(bump.BumpAssemblyInfo.Stats.ExecuteCount == 0, "AssemblyInfo files was not provided.");
        }
    }
}
