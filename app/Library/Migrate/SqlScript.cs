namespace Library.Migrate
{
    public class SqlScript
    {
        public string FileName { get; set; }
        public bool IsVersioned { get; set; }
        public string VersionId { get; set; }
    }
}
