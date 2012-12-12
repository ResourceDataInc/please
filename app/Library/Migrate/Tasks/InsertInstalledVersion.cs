using System;
using Simpler;
using Simpler.Data;

namespace Library.Migrate.Tasks
{
    public class InsertInstalledVersion : InOutTask<InsertInstalledVersion.Input, InsertInstalledVersion.Output>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public Version Version { get; set; }
        }

        public class Output
        {
            public int RowsAffected { get; set; }
        }

        public override void Execute()
        {
            var sql = @"INSERT INTO schema_migrations (version) VALUES ({0});";

            sql = String.Format(sql, In.Version.Id);

            using (var connection = Db.Connect(In.ConnectionName))
            {
                Out.RowsAffected = Db.GetResult(connection, sql);
            }
        }
    }
}
