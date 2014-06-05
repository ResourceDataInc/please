using System;
using System.IO;

namespace Tests
{
    public static class Database
    {
        static Database()
        {
            Name = Type.GetType("Mono.Runtime") != null ? "Mono" : ".NET";
        }

        public static string Name;

        public static void Restore()
        {
            File.Delete(Config.Scripts.Files.TestDatabase);
            File.Copy(Config.Scripts.Files.EmptyDatabase, Config.Scripts.Files.TestDatabase);
        }
    }
}
