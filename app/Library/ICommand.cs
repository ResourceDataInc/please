using Simpler;

namespace Library
{
    public interface ICommand
    {
        string Name { get; set; }
        Task Task { get; set; }
        void Run(string options);
    }
}