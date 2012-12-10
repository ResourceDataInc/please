using System.IO;
using Simpler;
using Simpler.Data;

namespace Library.Migrate.Tasks
{
    public class RunMigration : InOutTask<RunMigration.Input, RunMigration.Output>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public string FileName { get; set; }
        }

        public class Output
        {
            public int RowsAffected { get; set; }
        }

        public override void Execute()
        {
            var sql = File.ReadAllText(In.FileName);

            using (var connection = Db.Connect(In.ConnectionName))
            {
                Out.RowsAffected = Db.GetResult(connection, sql);
            }
        }
    }
}
