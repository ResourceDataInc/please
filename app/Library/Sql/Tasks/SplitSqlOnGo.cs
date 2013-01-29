using System.Text.RegularExpressions;
using Simpler;

namespace Library.Sql.Tasks
{
    public class SplitSqlOnGo : InOutTask<SplitSqlOnGo.Input, SplitSqlOnGo.Output>
    {
        public class Input
        {
            public string Sql { get; set; }
        }

        public class Output
        {
            public string[] SqlStrings { get; set; }
        }

        public override void Execute()
        {
            var sql = In.Sql;
            var lastGo = new Regex("^GO$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            sql = lastGo.Replace(sql, "");

            var inlineGo = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Out.SqlStrings = inlineGo.Split(sql);

            for (var i = 0; i < Out.SqlStrings.Length; i++)
            {
                Out.SqlStrings[i] = Out.SqlStrings[i].Trim();
            }
        }
    }
}
