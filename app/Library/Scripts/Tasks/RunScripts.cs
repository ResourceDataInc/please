using System;
using System.IO;
using Simpler;

namespace Library.Scripts.Tasks
{
    public class RunScripts : InTask<RunScripts.Input>
    {
        public class Input
        {
            public string ConnectionName { get; set; }
            public Script[] Scripts { get; set; }
        }

        public RunSql RunSql { get; set; }
        public RunProcess RunProcess { get; set; }

        public override void Execute()
        {
            foreach (var script in In.Scripts)
            {
                var fileName = Path.GetFileName(script.FileName);
                var fileNameWithoutPath = Path.GetFileName(fileName);
                if (fileNameWithoutPath == null) throw new RunException(String.Format("{0} is not a file.", fileName));

                var extension = Path.GetExtension(fileNameWithoutPath);
                if (String.Compare(extension, ".sql", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    RunSql.In.ConnectionName = In.ConnectionName;
                    RunSql.In.Sql = script;
                    RunSql.Execute();
                }
                else if (String.Compare(extension, ".py", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    RunProcess.In.FileName = "python";
                    RunProcess.In.Arguments = script.FileName;
                    RunProcess.Execute();
                }
                else
                {
                    throw new RunException(String.Format("Don't know how to run {0}.", fileName));
                }
            }
        }
    }
}
