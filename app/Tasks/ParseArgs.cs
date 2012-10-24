using System;
using Simpler;

namespace Tasks
{
    public class ParseArgs : InOutTask<ParseArgs.Input, ParseArgs.Output>
    {
        public class Input
        {
            public string[] Args { get; set; }
        }

        public class Output
        {
            public BumpType BumpType { get; set; }
        }

        public override void Execute()
        {
            var bumpType = Enum.Parse(typeof(BumpType), In.Args[0], true);
            Out.BumpType = (BumpType) bumpType;
        }
    }
}
