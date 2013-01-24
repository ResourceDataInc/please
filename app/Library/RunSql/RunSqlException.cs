using System.Data.Common;

namespace Library.RunSql
{
    public class RunSqlException : DbException
    {
        public RunSqlException(string message) : base (message) {}
    }
}
