using System;
using System.IO;
using Simpler;
using Simpler.Data;

namespace Library.Scripts.Tasks
{
    public class RunSql : InTask<RunSql.Input>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public Script Sql { get; set; }
        }

        public SplitSqlOnGo SplitSqlOnGo { get; set; }

        public override void Execute()
        {
            var fileName = Path.GetFileName(In.Sql.FileName);
            try
            {
                var sql = File.ReadAllText(In.Sql.FileName);

                SplitSqlOnGo.In.Sql = sql;
                SplitSqlOnGo.Execute();
                var sqlStrings = SplitSqlOnGo.Out.SqlStrings;

                using (var connection = Db.Connect(In.ConnectionName))
                {
                    foreach (var sqlString in sqlStrings)
                    {
                        Db.GetResult(connection, sqlString, null, Config.RunSqlTimeout);
                    }
                }
                Console.WriteLine("  {0} ran successfully.", fileName);
            }
            catch (Exception e)
            {
                throw new RunException(String.Format("{0} failed.\n  Message: {1}", fileName, e.Message));
            }
        }
    }
}
