using System;
using Simpler;
using Simpler.Data;

namespace Library.Migrate.Tasks
{
    public class CheckForVersionTable : InOutTask<CheckForVersionTable.Input, CheckForVersionTable.Output>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
        }

        public class Output
        {
            public bool TableExists { get; set; }
        }

        public override void Execute()
        {
            const string sql = @"SELECT COUNT(1) FROM schema_migrations;";

            using (var connection = Db.Connect(In.ConnectionName))
            {
                try
                {
                    Db.GetScalar(connection, sql);
                    Out.TableExists = true;
                }
                catch (Exception)
                {
                    Out.TableExists = false;
                }
            }
        }
    }
}
