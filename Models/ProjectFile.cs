namespace AI_XML_Doc.Models
{
    public class ProjectFile
    {
        public ProjectFile(string displayName, string path)
        {
            DisplayName = displayName;
            Path = path;
        }

        public string DisplayName { get; set; }
        public string Path { get; set; }
    }
}
