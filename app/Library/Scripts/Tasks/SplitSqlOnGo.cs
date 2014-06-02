using System.Linq;
using System.Text.RegularExpressions;
using Simpler;

namespace Library.Scripts.Tasks
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
            var goWord = new Regex("^GO(\\n|\\r|\\r\\n|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var sqlStrings = goWord.Split(In.Sql);

            Out.SqlStrings = sqlStrings.Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
        }
    }
}
