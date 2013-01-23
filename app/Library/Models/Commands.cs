using Library.Migrate.Tasks;

namespace Library.Models
{
    public class Commands
    {
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
                                        Action = (task, match) =>
                                                     {
                                                         var connectionName = match.Groups["ConnectionName"].Value;
                                                         task.In.Args = new[] {connectionName};
                                                     }
                                    },
                                new Option<RunSql>
                                    {
                                        // TODO - this pattern will need to allow \, /, -, _, etc.
                                        Pattern = @"in (?<Directory>\w+)",
                                        Action = (task, match) =>
                                                     {
                                                         var directory = match.Groups["Directory"].Value;
                                                         var previousArgs = task.In.Args;
                                                         task.In.Args = new[] {previousArgs[0], directory};
                                                     }
                                    }
                            }
                };

        public static ICommand[] All = {RunSql};
    }
}