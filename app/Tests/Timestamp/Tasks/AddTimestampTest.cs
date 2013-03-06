﻿using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Timestamp.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Timestamp.Tasks
{
    class AddTimestampTest
    {
        const string testDirectory = @"Timestamp\files\test";
        const string timestampPattern = "^\\d{14}_";

        public enum TimestampState
        {
            Timestamped,
            Untimestamped,
            All
        }

        public void RefreshTestDirectory(TimestampState state)
        {
            if (Directory.Exists(testDirectory))
                Directory.Delete(testDirectory, true);
            Directory.CreateDirectory(testDirectory);
            
            foreach (var file in Directory.GetFiles(@"Timestamp\files\"))
            {
                string filename = Path.GetFileName(file);

                if (Regex.IsMatch(filename, timestampPattern) && state == TimestampState.Untimestamped)
                    continue;
                if (!Regex.IsMatch(filename, timestampPattern) && state == TimestampState.Timestamped)
                    continue;

                File.Copy(file, testDirectory + @"\" + filename);
            }
        }

        public bool FileIsTimestamped(string filename)
        {
            return Regex.IsMatch(filename, timestampPattern);
        }

        [Test]
        public void should_timestamp_new_files()
        {
            RefreshTestDirectory(TimestampState.Untimestamped);

            var addTimestamp = Task.New<AddTimestamp>();
            addTimestamp.In.Directory = testDirectory;
            addTimestamp.Execute();

            bool allTimestamped = true;

            foreach (var file in Directory.GetFiles(testDirectory))
            {
                if (!FileIsTimestamped(Path.GetFileName(file)))
                {
                    allTimestamped = false;
                    break;
                }
            }

            Assert.True(allTimestamped);
            Assert.That(Directory.GetFiles(testDirectory).Count(), Is.EqualTo(2));
        }

        [Test]
        public void should_not_retimestamp_files()
        {
            RefreshTestDirectory(TimestampState.Timestamped);

            var originalFiles = Directory.GetFiles(testDirectory);

            var addTimestamp = Task.New<AddTimestamp>();
            addTimestamp.In.Directory = testDirectory;
            addTimestamp.Execute();

            var updatedFiles = Directory.GetFiles(testDirectory);

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