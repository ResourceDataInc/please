using System.IO;
using Library.Migrate.Model;
using Simpler;
using Simpler.Data;

namespace Library.Migrate.Tasks
{
    public class RunMigration : InOutTask<RunMigration.Input, RunMigration.Output>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public Migration Migration { get; set; }
        }

        public class Output
        {
            public int RowsAffected { get; set; }
        }

        public override void Execute()
        {
            var sql = File.ReadAllText(In.Migration.FileNameWithPath);

            using (var connection = Db.Connect(In.ConnectionName))
            {
                Out.RowsAffected = Db.GetResult(connection, sql);
            }
        }
    }
}
