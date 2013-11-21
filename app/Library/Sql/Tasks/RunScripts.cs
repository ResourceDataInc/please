using System;
using System.IO;
using Simpler;
using Simpler.Data;

namespace Library.Sql.Tasks
{
    public class RunScripts : InOutTask<RunScripts.Input, RunScripts.Output>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public Script[] Scripts { get; set; }
        }

        public class Output
        {
            public int RowsAffected { get; set; }
        }

        public SplitSqlOnGo SplitSqlOnGo { get; set; }

        public override void Execute()
        {
            foreach (var script in In.Scripts)
            {
                var fileName = Path.GetFileName(script.FileName);
                try
                {
                    var sql = File.ReadAllText(script.FileName);

                    SplitSqlOnGo.In.Sql = sql;
                    SplitSqlOnGo.Execute();
                    var sqlStrings = SplitSqlOnGo.Out.SqlStrings;

                    using (var connection = Db.Connect(In.ConnectionName))
                    {
                        foreach (var sqlString in sqlStrings)
                        {
                            Out.RowsAffected += Db.GetResult(connection, sqlString, null, Config.RunSqlTimeout);
                        }
                    }
                    Console.WriteLine("  {0} ran successfully.", fileName);
                }
                catch (Exception e)
                {
                    throw new RunSqlException(String.Format("{0} failed.\n  Message: {1}", fileName, e.Message));
                }
            }
        }
    }
}
