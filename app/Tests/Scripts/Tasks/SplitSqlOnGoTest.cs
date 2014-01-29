using System.IO;
using Library.Scripts.Tasks;
using NUnit.Framework;
using Simpler;

namespace Tests.Scripts.Tasks
{
    [TestFixture]
    public class SplitSqlOnGoTest
    {
        [Test]
        public void should_split_script_into_multiple_strings_based_on_occurrences_of_the_GO_keyword()
        {
            // Arrange
            var splitScriptOnGo = Task.New<SplitSqlOnGo>();
            splitScriptOnGo.In.Sql = File.ReadAllText(@"Scripts\files\sql\repeatable\create-four-tables.sql");

            // Act
            splitScriptOnGo.Execute();

            // Assert
            Assert.That(splitScriptOnGo.Out.SqlStrings.Length, Is.EqualTo(4));
            Assert.That(splitScriptOnGo.Out.SqlStrings[0].Contains("table1"));
            Assert.That(splitScriptOnGo.Out.SqlStrings[1].Contains("table2"));
            Assert.That(splitScriptOnGo.Out.SqlStrings[2].Contains("table3"));
            Assert.That(splitScriptOnGo.Out.SqlStrings[3].Contains("table4"));
        }
    }
}
