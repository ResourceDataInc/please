using System.Data.Common;

namespace Library.Scripts
{
    public class RunException : DbException
    {
        public RunException(string message) : base (message) {}
    }
}
