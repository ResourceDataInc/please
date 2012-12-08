using Simpler;
using Simpler.Data;

namespace Library.Migrate.Tasks
{
    public class CreateVersionTable : InOutTask<CreateVersionTable.Input, CreateVersionTable.Output>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
        }

        public class Output
        {
            public int RowsAffected { get; set; }
        }

        public override void Execute()
        {
            const string sql = @"CREATE TABLE schema_migrations (version NVARCHAR(255) NOT NULL UNIQUE);";

            using (var connection = Db.Connect(In.ConnectionName))
            {
                Out.RowsAffected = Db.GetResult(connection, sql);
            }
        }
    }
}
