using System.IO;

namespace Tests.Config
{
    public static class Timestamp
    {
        public static class Files
        {
            public static string Before
            {
                get { return Path.Combine("Timestamp", "files", "before"); }
            }

            public static string After
            {
                get { return Path.Combine("Timestamp", "files", "after"); }
            }
        }
    }
}

