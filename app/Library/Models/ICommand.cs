using Simpler;

namespace Library.Models
{
    public interface ICommand
    {
        string Name { get; set; }
        Task Task { get; set; }
        void Run(string options);
    }
}