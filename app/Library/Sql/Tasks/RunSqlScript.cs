using System.IO;
using Simpler;
using Simpler.Data;

namespace Library.Sql.Tasks
{
    public class RunSqlScript : InOutTask<RunSqlScript.Input, RunSqlScript.Output>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public SqlScript SqlScript { get; set; }
        }

        public class Output
        {
            public int RowsAffected { get; set; }
        }

        public SplitSqlOnGo SplitSqlOnGo { get; set; }

        public override void Execute()
        {
            var sql = File.ReadAllText(In.SqlScript.FileName);

            SplitSqlOnGo.In.Sql = sql;
            SplitSqlOnGo.Execute();
            var sqlStrings = SplitSqlOnGo.Out.SqlStrings;

            using (var connection = Db.Connect(In.ConnectionName))
            {
                foreach (var sqlString in sqlStrings)
                {
                    Out.RowsAffected += Db.GetResult(connection, sqlString);
                }
            }
        }
    }
}
