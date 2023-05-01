namespace AI_XML_Doc
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application. Initializes the application configuration and runs the main form.
        /// </summary>
        /// <remarks>
        /// This function sets the application configuration by calling the <see cref="ApplicationConfiguration.Initialize"/> method and then runs the main form by calling the <see cref="Application.Run"/> method with an instance of <see cref="Form1"/>.
        /// </remarks>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}