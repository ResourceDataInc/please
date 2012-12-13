using System;
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
            public Migration Migration { get; set; }
        }

        public class Output
        {
            public int RowsAffected { get; set; }
        }

        public override void Execute()
        {
            var sql = File.ReadAllText(In.Migration.FileName);

            using (var connection = Db.Connect(In.ConnectionName))
            {
                try
                {
                    Out.RowsAffected = Db.GetResult(connection, sql);
                }
                catch (Exception e)
                {
                    var migration = Path.GetFileName(In.Migration.FileName);
                    throw new MigrationException(String.Format("{0} failed.\nMessage: {1}", migration, e.Message));
                }
            }
        }
    }
}
