using System.Data.Common;

namespace Library.Sql
{
    public class RunException : DbException
    {
        public RunException(string message) : base (message) {}
    }
}
