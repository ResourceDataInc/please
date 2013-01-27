using Library.Releases;
using Library.Releases.Tasks;
using Library.Sql.Tasks;

namespace Library
{
    public class Commands
    {
        static readonly Command<Bump> Bump =
            new Command<Bump>
            {
                Name = "bump",
                Options =
                    new[]
                            {
                                new Option<Bump>
                                    {
                                        Pattern = @"major\s",
                                        Action = (task, match) => task.In.BumpType = BumpType.Major
                                    },
                                new Option<Bump>
                                    {
                                        Pattern = @"minor\s",
                                        Action = (task, match) => task.In.BumpType = BumpType.Minor
                                    },
                                new Option<Bump>
                                    {
                                        Pattern = @"patch\s",
                                        Action = (task, match) => task.In.BumpType = BumpType.Patch
                                    },
                                new Option<Bump>
                                    {
                                        // TODO - this pattern will need to allow \, /, -, _, etc.
                                        Pattern = @"in (?<File>AssemblyInfo\.cs)",
                                        Action = (task, match) =>
                                                     {
                                                         task.In.FileType = FileType.AssemblyInfo;
                                                         task.In.FileName = match.Groups["File"].Value;
                                                     }
                                    },
                                new Option<Bump>
                                    {
                                        // TODO - this pattern will need to allow \, /, -, _, etc.
                                        Pattern = @"in (?<File>\w+\.nuspec)",
                                        Action = (task, match) =>
                                                     {
                                                         task.In.FileType = FileType.Nuspec;
                                                         task.In.FileName = match.Groups["File"].Value;
                                                     }
                                    },
                                new Option<Bump>
                                    {
                                        // TODO - this pattern will need to allow \, /, -, _, etc.
                                        Pattern = @"in (?<File>\w+)",
                                        Action = (task, match) =>
                                                     {
                                                         task.In.FileType = FileType.Script;
                                                         task.In.FileName = match.Groups["File"].Value;
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
                                        // TODO - this pattern will need to allow \, /, -, _, etc.
                                        Pattern = @"in (?<Directory>\w+)",
                                        Action = (task, match) => task.In.Directory = match.Groups["Directory"].Value
                                    }
                            }
            };

        public static ICommand[] All = {Bump, RunSql};
    }
}