using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.Timestamp.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Timestamp.Tasks
{
    class GetFilesTest
    {
        const string directory = @"Timestamp\files";

        [Test]
        public void should_find_three_files()
        {
            var GetFiles = Task.New<GetFiles>();
            GetFiles.In.Directory = directory;
            GetFiles.Execute();

            Assert.That(GetFiles.Out.Files.Count(), Is.EqualTo(3));
        }

        [Test]
        public void should_find_two_nontimestamped_files()
        {
            var GetFiles = Task.New<GetFiles>();
            GetFiles.In.Directory = directory;
            GetFiles.Execute();

            Assert.That(GetFiles.Out.Files.Count(f => !f.IsTimestamped), Is.EqualTo(2));
        }

        [Test]
        public void should_find_one_timestamped_file()
        {
            var GetFiles = Task.New<GetFiles>();
            GetFiles.In.Directory = directory;
            GetFiles.Execute();

            Assert.That(GetFiles.Out.Files.Count(f => f.IsTimestamped), Is.EqualTo(1));
        }
    }
}
