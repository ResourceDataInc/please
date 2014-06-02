using System.IO;

namespace Tests.Config
{
    public static class Releases
    {
        public static class Files
        {
            public static string AssemblyInfoFiles
            {
                get { return Path.Combine("Releases", "files", "AssemblyInfo"); }
            }

            public static string NuspecFiles
            {
                get { return Path.Combine("Releases", "files", "nuspec"); }
            }

            public static string ScriptFiles
            {
                get { return Path.Combine("Releases", "files", "script"); }
            }

            public static class AssemblyInfo
            {
                public static string Before
                {
                    get { return Path.Combine(AssemblyInfoFiles, "AssemblyInfo_Before_cs"); }
                }

                public static string After
                {
                    get { return Path.Combine(AssemblyInfoFiles, "AssemblyInfo_After_cs"); }
                }

                public static string MajorBumped
                {
                    get { return Path.Combine(AssemblyInfoFiles, "AssemblyInfo_MajorBumped_cs"); }
                }

                public static string MinorBumped
                {
                    get { return Path.Combine(AssemblyInfoFiles, "AssemblyInfo_MinorBumped_cs"); }
                }

                public static string PatchBumped
                {
                    get { return Path.Combine(AssemblyInfoFiles, "AssemblyInfo_PatchBumped_cs"); }
                }
            }

            public static class Nuspec
            {
                public static string Before
                {
                    get { return Path.Combine(NuspecFiles, "Nuspec_Before.nuspec"); }
                }

                public static string After
                {
                    get { return Path.Combine(NuspecFiles, "Nuspec_After.nuspec"); }
                }

                public static string MajorBumped
                {
                    get { return Path.Combine(NuspecFiles, "Nuspec_MajorBumped.nuspec"); }
                }

                public static string MinorBumped
                {
                    get { return Path.Combine(NuspecFiles, "Nuspec_MinorBumped.nuspec"); }
                }

                public static string PatchBumped
                {
                    get { return Path.Combine(NuspecFiles, "Nuspec_PatchBumped.nuspec"); }
                }
            }

            public static class Script
            {
                public static string Before
                {
                    get { return Path.Combine(ScriptFiles, "Script_Before.txt"); }
                }

                public static string After
                {
                    get { return Path.Combine(ScriptFiles, "Script_After.txt"); }
                }

                public static string MajorBumped
                {
                    get { return Path.Combine(ScriptFiles, "Script_MajorBumped.txt"); }
                }

                public static string MinorBumped
                {
                    get { return Path.Combine(ScriptFiles, "Script_MinorBumped.txt"); }
                }

                public static string PatchBumped
                {
                    get { return Path.Combine(ScriptFiles, "Script_PatchBumped.txt"); }
                }
            }
        }
    }
}

