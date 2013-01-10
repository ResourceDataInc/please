using Simpler;
using Simpler.Data;

namespace Library.Migrate.Tasks
{
    public class FetchInstalledVersions : InOutTask<FetchInstalledVersions.Input, FetchInstalledVersions.Output>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
        }

        public class Output
        {
            public Version[] Versions { get; set; }
        }

        public override void Execute()
        {
            const string sql = @"SELECT version as Id FROM db_version;";

            using (var connection = Db.Connect(In.ConnectionName))
            {
                Out.Versions = Db.GetMany<Version>(connection, sql);
            }
        }
    }
}
