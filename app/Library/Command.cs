using System.Text.RegularExpressions;
using Simpler;
using Simpler.Core.Tasks;

namespace Library
{
    public class Command<TTask> : ICommand where TTask : Task
    {
        public string Name { get; set; }
        public Option<TTask>[] Options { get; set; }
        public Task Task { get; set; }

        public void Run(string options)
        {
            if (Task == null)
            {
                var createTask = new CreateTask { In = { TaskType = typeof(TTask) } };
                createTask.Execute();
                Task = (TTask)createTask.Out.TaskInstance;
            }

            foreach (var option in Options)
            {
                var match = Regex.Match(options, option.Pattern);
                if (match.Success)
                {
                    option.Action((TTask)Task, match);
                    options = options.Replace(match.ToString(), "");
                }
            }

            Task.Execute();
        }
    }
}
