using Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Tasks
{
    [TestFixture]
    public class ParseArgsTest
    {
        readonly string[] _testArgs =
            new[]
                {
                    "SomeRelease",
                    "patch",
                    @"scriptA",
                    @"C:\Somewhere\AssemblyInfo.cs",
                    @"C:\Somewhere\Foo.nuspec",
                    @"\.scriptB.bat",
                    @".\AssemblyInfo.cs",
                    @"C:\SomewhereElse\scriptC.exe",
                    @"Bar.nuspec"
                };

        [Test]
        public void should_find_release_id()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = new[] { "SomeRelease", "patch" };

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.ReleaseId == "SomeRelease", "Should have found SomeRelease.");
        }

        [Test]
        public void should_find_major_bump_type_in_first_arg()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = new[] { "SomeRelease", "major" };

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.BumpType == BumpType.Major, "Should have found Major.");
        }

        [Test]
        public void should_find_minor_bump_type_in_first_arg()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = new[] { "SomeRelease", "minor" };

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.BumpType == BumpType.Minor, "Should have found Minor.");
        }

        [Test]
        public void should_find_patch_bump_type_in_first_arg()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = new[] { "SomeRelease", "patch" };

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.BumpType == BumpType.Patch, "Should have found Patch.");
        }

        [Test]
        public void should_find_assembly_info_files_in_subsequent_args()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = _testArgs;

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.AssemblyInfoFiles.Length == 2, "Should have found 2 AssemblyInfo.cs files.");
            Check.That(parse.Out.AssemblyInfoFiles[0] == @"C:\Somewhere\AssemblyInfo.cs", "Should have found first AssemblyInfo.cs file.");
            Check.That(parse.Out.AssemblyInfoFiles[1] == @".\AssemblyInfo.cs", "Should have found second AssemblyInfo.cs file.");
        }

        [Test]
        public void should_find_nuspec_files_in_subsequent_args()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = _testArgs;

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.NuspecFiles.Length == 2, "Should have found 2 .nuspec files.");
            Check.That(parse.Out.NuspecFiles[0] == @"C:\Somewhere\Foo.nuspec", "Should have found first .nuspec file.");
            Check.That(parse.Out.NuspecFiles[1] == @"Bar.nuspec", "Should have found second .nuspec file.");
        }

        [Test]
        public void should_find_other_files_in_subsequent_args()
        {
            // Arrange
            var parse = Task.New<ParseArgs>();
            parse.In.Args = _testArgs;

            // Act
            parse.Execute();

            // Assert
            Check.That(parse.Out.OtherFiles.Length == 3, "Should have found 3 other files.");
            Check.That(parse.Out.OtherFiles[0] == @"scriptA", "Should have found first .nuspec file.");
            Check.That(parse.Out.OtherFiles[1] == @"\.scriptB.bat", "Should have found second .nuspec file.");
            Check.That(parse.Out.OtherFiles[2] == @"C:\SomewhereElse\scriptC.exe", "Should have found second .nuspec file.");
        }
    }
}
