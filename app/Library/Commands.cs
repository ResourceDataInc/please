using Library.Releases;
using Library.Releases.Tasks;
using Library.Sql.Tasks;
using Library.Timestamp.Tasks;

namespace Library
{
    public class Commands
    {
        const string Path = @"(?:[a-zA-Z]\:\\|\\\\|\.\\)*([^/?*:;{}\\]+\\)*";
        const string FileOrDirectory = @"(?:[^/?*:;{}\\]+)";

        static readonly Command<Bump> Bump =
            new Command<Bump>
            {
                Name = "bump",
                Options =
                    new[]
                            {
                                new Option<Bump>
                                    {
                                        Pattern = @"major version\s",
                                        Action = (task, match) => task.In.BumpType = BumpType.Major
                                    },
                                new Option<Bump>
                                    {
                                        Pattern = @"minor version\s",
                                        Action = (task, match) => task.In.BumpType = BumpType.Minor
                                    },
                                new Option<Bump>
                                    {
                                        Pattern = @"patch version\s",
                                        Action = (task, match) => task.In.BumpType = BumpType.Patch
                                    },
                                new Option<Bump>
                                    {
                                        Pattern = @"in (?<File>" + Path + @"AssemblyInfo\.cs)",
                                        Action = (task, match) =>
                                                     {
                                                         task.In.FileType = FileType.AssemblyInfo;
                                                         task.In.FileName = match.Groups["File"].Value.Trim();
                                                     }
                                    },
                                new Option<Bump>
                                    {
                                        Pattern = @"in (?<File>" + Path + FileOrDirectory + @"\.nuspec)",
                                        Action = (task, match) =>
                                                     {
                                                         task.In.FileType = FileType.Nuspec;
                                                         task.In.FileName = match.Groups["File"].Value.Trim();
                                                     }
                                    },
                                new Option<Bump>
                                    {
                                        Pattern = @"in (?<File>" + Path + FileOrDirectory + ")",
                                        Action = (task, match) =>
                                                     {
                                                         task.In.FileType = FileType.Script;
                                                         task.In.FileName = match.Groups["File"].Value.Trim();
                                                     }
                                    }
                            }
            };

        static readonly Command<RunSql> RunSql =
            new Command<RunSql>
            {
                Name = "run sql",
                Options =
                    new[]
                            {
                                new Option<RunSql>
                                    {
                                        Pattern = "with versioning",
                                        Action = (task, match) => task.In.WithVersioning = true
                                    },
                                new Option<RunSql>
                                    {
                                        Pattern = @"on (?<ConnectionName>\w+)",
                                        Action = (task, match) => task.In.ConnectionName = match.Groups["ConnectionName"].Value
                                    },
                                new Option<RunSql>
                                    {
                                        Pattern = @"in (?<Directory>" + Path + FileOrDirectory + ")",
                                        Action = (task, match) => task.In.Directory = match.Groups["Directory"].Value.Trim()
                                    },
                                new Option<RunSql>
                                    {
                                        Pattern = @"include (?<Whitelist>" + Path + FileOrDirectory + ")",
                                        Action = (task, match) => task.In.WhitelistFile = match.Groups["Whitelist"].Value.Trim()
                                    }
                            }
            };

        private static readonly Command<Timestamp.Tasks.Timestamp> Timestamp =
            new Command<Timestamp.Tasks.Timestamp>
                {
                    Name = "add timestamps",
                    Options =
                    new []
                        {
                            new Option<Timestamp.Tasks.Timestamp>
                                {
                                    Pattern = "in (?<Directory>" + Path + FileOrDirectory + ")",
                                    Action =  (task, match) => task.In.Directory = match.Groups["Directory"].Value.Trim()
                                }
                        }
                };

        public static ICommand[] All = {Bump, RunSql, Timestamp};
    }
}