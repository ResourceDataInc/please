using System.Data.Common;

namespace Library.Migrate
{
    public class MigrationException : DbException
    {
        public MigrationException(string message) : base (message) {}
    }
}
