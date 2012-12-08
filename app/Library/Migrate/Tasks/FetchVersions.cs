﻿using Library.Migrate.Model;
using Simpler;
using Simpler.Data;

namespace Library.Migrate.Tasks
{
    public class FetchVersions : InOutTask<FetchVersions.Input, FetchVersions.Output>
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
            const string sql = @"SELECT version as Number FROM schema_migrations;";

            using (var connection = Db.Connect(In.ConnectionName))
            {
                Out.Versions = Db.GetMany<Version>(connection, sql);
            }
        }
    }
}
