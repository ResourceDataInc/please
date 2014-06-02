using System.IO;

namespace Tests.Config
{
    public static class Scripts
    {
        public static class Files
        {
            public static string PyFiles
            {
                get { return Path.Combine("Scripts", "files", "py"); }
            }

            public static string SqlFiles
            {
                get { return Path.Combine("Scripts", "files", "sql"); }
            }

            public static string EmptyDatabase
            {
                get { return Path.Combine("Scripts", "files", "empty.db"); }
            }

            public static string TestDatabase
            {
                get { return Path.Combine("Scripts", "files", "test.db"); }
            }

            public static class Sql
            {
                public static string GoFiles
                {
                    get { return Path.Combine(SqlFiles, "GO"); }
                }

                public static string VersionedFiles
                {
                    get { return Path.Combine(SqlFiles, "versioned"); }
                }

                public static string InsertVersion
                {
                    get { return Path.Combine(SqlFiles, "insert-version.sql"); }
                }

                public static string Error
                {
                    get { return Path.Combine(SqlFiles, "error.sql"); }
                }

                public static class Go
                {
                    public static string CreateFourTables
                    {
                        get { return Path.Combine(GoFiles, "create-four-tables.sql"); }
                    }
                }

                public static class Versioned
                {
                    public static string First
                    {
                        get { return Path.Combine(VersionedFiles, "000001_create-testing-table.sql"); }
                    }

                    public static string Second
                    {
                        get { return Path.Combine(VersionedFiles, "000002_001.sql"); }
                    }

                    public static string Third
                    {
                        get { return Path.Combine(VersionedFiles, "000002_002.sql"); }
                    }

                    public static string Whitelist
                    {
                        get { return Path.Combine(VersionedFiles, "whitelist.txt"); }
                    }
                }
            }

            public static class Py
            {
                public static string Hello
                {
                    get { return Path.Combine(PyFiles, "1_hello.py"); }
                }

                public static string Error
                {
                    get { return Path.Combine(PyFiles, "2_error.py"); }
                }
            }
        }
    }
}

