using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Timestamp.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Timestamp.Tasks
{
    [TestFixture]
    public class AddTimestampTest
    {
        const string timestampPattern = "^\\d{14}_";

        public enum TimestampState
        {
            Timestamped,
            Untimestamped,
            All
        }

        public void RefreshTestDirectory(TimestampState state)
        {
            if (Directory.Exists(Config.Timestamp.Files.After))
            {
                Directory.Delete(Config.Timestamp.Files.After, true);
            }
            Directory.CreateDirectory(Config.Timestamp.Files.After);
            
            foreach (var file in Directory.GetFiles(Config.Timestamp.Files.Before))
            {
                string filename = Path.GetFileName(file);

                if (Regex.IsMatch(filename, timestampPattern) && state == TimestampState.Untimestamped)
                    continue;
                if (!Regex.IsMatch(filename, timestampPattern) && state == TimestampState.Timestamped)
                    continue;

                File.Copy(file, Path.Combine(Config.Timestamp.Files.After, filename));
            }
        }

        public bool FileIsTimestamped(string filename)
        {
            return Regex.IsMatch(filename, timestampPattern, RegexOptions.None);
        }

        [Test]
        public void should_timestamp_new_files()
        {
            RefreshTestDirectory(TimestampState.Untimestamped);

            var addTimestamp = Task.New<AddTimestamp>();
            addTimestamp.In.Directory = Config.Timestamp.Files.After;
            addTimestamp.Execute();

            bool allTimestamped = true;

            foreach (var file in Directory.GetFiles(Config.Timestamp.Files.After))
            {
                var fileName = Path.GetFileName(file);
                if (!FileIsTimestamped(fileName))
                {
                    allTimestamped = false;
                    break;
                }
            }

            Assert.That(allTimestamped, Is.True);
            Assert.That(Directory.GetFiles(Config.Timestamp.Files.After).Length, Is.EqualTo(2));
        }

        [Test]
        public void should_not_retimestamp_files()
        {
            RefreshTestDirectory(TimestampState.Timestamped);

            var originalFiles = Directory.GetFiles(Config.Timestamp.Files.After);

            var addTimestamp = Task.New<AddTimestamp>();
            addTimestamp.In.Directory = Config.Timestamp.Files.After;
            addTimestamp.Execute();

            var updatedFiles = Directory.GetFiles(Config.Timestamp.Files.After);

            bool filenamesMatch = true;

            for (int i = 0; i < updatedFiles.Count(); i++)
            {
                if (originalFiles[i] != updatedFiles[i])
                {
                    filenamesMatch = false;
                    break;
                }
            }

            Assert.True(filenamesMatch);
            Assert.That(updatedFiles.Count(), Is.EqualTo(1));
        }
    }
}
