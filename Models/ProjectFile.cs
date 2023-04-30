namespace AI_XML_Doc.Models
{
    public class ProjectFile
    {
        /// <summary>
        /// Initializes a new instance of the ProjectFile class with the specified display name and path.
        /// </summary>
        /// <param name="displayName">The display name of the project file.</param>
        /// <param name="path">The path of the project file.</param>
        /// <returns>A new instance of the ProjectFile class.</returns>
        public ProjectFile(string displayName, string path)
        {
            DisplayName = displayName;
            Path = path;
        }

        public string DisplayName { get; set; }
        public string Path { get; set; }
    }
}
