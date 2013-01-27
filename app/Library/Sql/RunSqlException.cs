using System.Data.Common;

namespace Library.Sql
{
    public class RunSqlException : DbException
    {
        public RunSqlException(string message) : base (message) {}
    }
}
