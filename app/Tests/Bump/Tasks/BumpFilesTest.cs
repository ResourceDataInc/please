using Library.Bump.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Bump.Tasks
{
    [TestFixture]
    public class BumpFilesTest
    {
        [Test]
        public void should_bump_given_nuspec_files()
        {
            // Arrange
            var bump = Task.New<BumpFiles>();
            bump.In.Args =
                new[]
                    {
                        "patch",
                        @"C:\Somewhere\Foo.nuspec",
                        @"Bar.nuspec"
                    };
            bump.BumpAssemblyInfo = Fake.Task<BumpAssemblyInfo>();
            bump.BumpNuspec = Fake.Task<BumpNuspec>();
            bump.BumpScript = Fake.Task<BumpScript>();

            // Act
            bump.Execute();

            // Assert
            Check.That(bump.BumpNuspec.Stats.ExecuteCount == 2, "Should have bumped 2 .nuspec files.");
            Check.That(bump.BumpAssemblyInfo.Stats.ExecuteCount == 0, "No AssemblyInfo files were provided.");
            Check.That(bump.BumpScript.Stats.ExecuteCount == 0, "No script files were provided.");
        }

        [Test]
        public void should_bump_given_AssemblyInfo_files()
        {
            // Arrange
            var bump = Task.New<BumpFiles>();
            bump.In.Args =
                new[]
                    {
                        "patch",
                        @"C:\Somewhere\AssemblyInfo.cs",
                        @".\AssemblyInfo.cs"
                    };
            bump.BumpAssemblyInfo = Fake.Task<BumpAssemblyInfo>();
            bump.BumpNuspec = Fake.Task<BumpNuspec>();
            bump.BumpScript = Fake.Task<BumpScript>();

            // Act
            bump.Execute();

            // Assert
            Check.That(bump.BumpAssemblyInfo.Stats.ExecuteCount == 2, "Should have bumped 2 AssemblyInfo files.");
            Check.That(bump.BumpNuspec.Stats.ExecuteCount == 0, "No .nuspec files were provided.");
            Check.That(bump.BumpScript.Stats.ExecuteCount == 0, "No script files were provided.");
        }

        [Test]
        public void should_bump_given_script_files()
        {
            // Arrange
            var bump = Task.New<BumpFiles>();
            bump.In.Args =
                new[]
                    {
                        "patch",
                        @"\.scriptB.bat",
                        @"C:\SomewhereElse\scriptC.exe"
                    };
            bump.BumpAssemblyInfo = Fake.Task<BumpAssemblyInfo>();
            bump.BumpNuspec = Fake.Task<BumpNuspec>();
            bump.BumpScript = Fake.Task<BumpScript>();

            // Act
            bump.Execute();

            // Assert
            Check.That(bump.BumpScript.Stats.ExecuteCount == 2, "Should have bumped 2 script files.");
            Check.That(bump.BumpNuspec.Stats.ExecuteCount == 0, "No .nuspec files were provided.");
            Check.That(bump.BumpAssemblyInfo.Stats.ExecuteCount == 0, "No AssemblyInfo files were provided.");
        }
    }
}
