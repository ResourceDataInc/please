using Simpler;
using Simpler.Data;

namespace Library.RunSql.Tasks
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
            const string sql = @"CREATE TABLE db_version (version NVARCHAR(255) NOT NULL UNIQUE);";

            using (var connection = Db.Connect(In.ConnectionName))
            {
                Out.RowsAffected = Db.GetResult(connection, sql);
            }
        }
    }
}
