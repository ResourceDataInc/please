using System.Data.Common;

namespace Library.Migrate
{
    public class RunSqlException : DbException
    {
        public RunSqlException(string message) : base (message) {}
    }
}
