using AI_XML_Doc.Helpers;
using AI_XML_Doc.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AI_XML_Doc
{
    public partial class Form1 : Form
    {
        private List<ProjectFile> _projectFiles = new();
        private List<ProjectFile> _toBeGeneratedFiles = new();
        private string? _language;
        private bool _replaceOldDocs = false;

        /// <summary>
        /// Initializes a new instance of the Form1 class.
        /// </summary>
        /// <remarks>
        /// This constructor calls the InitializeComponent method to initialize the form's components.
        /// </remarks>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This function is called when the Form1 is loaded. It sets the default language in the languageComboBox and updates the language of the form.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The EventArgs associated with the event.</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            languageComboBox.SelectedIndex = 0;
            UpdateLanguage();
        }

        /// <summary>
        /// Updates the selected language based on the value of the languageComboBox.
        /// </summary>
        /// <returns>Void.</returns>
        /// <exception cref="System.NullReferenceException">Thrown when languageComboBox.SelectedItem is null.</exception>
        private void UpdateLanguage() =>
                    _language = languageComboBox.SelectedItem.ToString();

        /// <summary>
        /// Handles the click event of the search button to open a folder browser dialog and set the project path text box to the selected path.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>Void.</returns>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                projectPathTextBox.Text = fbd.SelectedPath;
                GetFilesFromProjectPath();
            }
        }

        /// <summary>
        /// Retrieves all C# files from the specified project path and populates the _projectFiles list with ProjectFile objects representing each file. Also updates the list box items with the retrieved files.
        /// </summary>
        /// <returns>Void</returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown when the specified project path is not found.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when the specified project path is inaccessible due to permission restrictions.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the specified project path is empty or consists only of white space characters.</exception>
        private void GetFilesFromProjectPath()
        {
            _projectFiles = new();
            _toBeGeneratedFiles = new();

            var path = projectPathTextBox.Text;
            if (string.IsNullOrWhiteSpace(path))
                return;

            _projectFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .Select(filePath => new ProjectFile(Path.GetFileNameWithoutExtension(filePath), filePath))
                .ToList();

            UpdateListBoxItems();
        }

        /// <summary>
        /// Clears and updates the items in the projectFilesListBox and filesToGenerateListBox based on the current state of the _projectFiles and _toBeGeneratedFiles lists.
        /// </summary>
        /// <returns>Void</returns>
        private void UpdateListBoxItems()
        {
            projectFilesListBox.Items.Clear();
            filesToGenerateListBox.Items.Clear();
            _projectFiles.ForEach(file => projectFilesListBox.Items.Add(file.DisplayName));
            _toBeGeneratedFiles.ForEach(file => filesToGenerateListBox.Items.Add(file.DisplayName));
        }

        /// <summary>
        /// Moves selected items from projectFilesListBox to _toBeGeneratedFiles list and removes them from _projectFiles list. 
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>Void.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when no items are selected in projectFilesListBox.</exception>
        private void btnMoveToRight_Click(object sender, EventArgs e)
        {
            var selectedItems = projectFilesListBox.SelectedItems.Cast<string>()
                .Select(x => _projectFiles.First(y => y.DisplayName == x))
                .ToList();

            if (selectedItems.Count <= 0)
                return;

            selectedItems.ForEach(item =>
            {
                if (_toBeGeneratedFiles.Any(x => x == item) is false)
                    _toBeGeneratedFiles.Add(item);

                _projectFiles.Remove(item);
            });

            UpdateListBoxItems();
        }

        /// <summary>
        /// Moves selected items from filesToGenerateListBox to _projectFiles list and removes them from _toBeGeneratedFiles list.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>Void.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when no items are selected in filesToGenerateListBox.</exception>
        private void btnMoveToLeft_Click(object sender, EventArgs e)
        {
            var selectedItems = filesToGenerateListBox.SelectedItems.Cast<string>()
                .Select(x => _toBeGeneratedFiles.First(y => y.DisplayName == x))
                .ToList();

            if (selectedItems.Count <= 0)
                return;

            selectedItems.ForEach(item =>
            {
                if (_projectFiles.Any(x => x == item) is false)
                    _projectFiles.Add(item);

                _toBeGeneratedFiles.Remove(item);
            });

            UpdateListBoxItems();
        }

        /// <summary>
        /// Generates documentation for a list of files based on their content and updates them if necessary.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>Void.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when one of the files to be generated is not found.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when the user does not have permission to access one of the files.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs while reading or writing a file.</exception>
        private async void btnGenerateDocs_Click(object sender, EventArgs e)
        {
            if (_toBeGeneratedFiles.Count <= 0
                || string.IsNullOrWhiteSpace(apiKeyTextBox.Text))
                return;

            Enabled = false;

            foreach (var file in _toBeGeneratedFiles)
            {
                var fileContent = File.ReadAllText(file.Path);
                var updatedFileContent = await ProcessFunctions(fileContent);

                if (updatedFileContent != fileContent)
                    File.WriteAllText(file.Path, updatedFileContent);
            }

            Enabled = true;
        }

        /// <summary>
        /// Processes the functions in a C# file and generates XML documentation comments for any functions that do not already have them.
        /// </summary>
        /// <param name="fileContent">The content of the C# file to process.</param>
        /// <returns>The updated content of the C# file with any newly generated XML documentation comments.</returns>
        /// <exception cref="ArgumentNullException">Thrown if fileContent is null.</exception>
        public async ValueTask<string> ProcessFunctions(string fileContent)
        {
            var oaiHelper = new OaiHelper(apiKeyTextBox.Text);

            var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
            var root = syntaxTree.GetCompilationUnitRoot();

            var classes = root.DescendantNodes().OfType<TypeDeclarationSyntax>();
            foreach (var @class in classes)
            {
                var className = @class.Identifier.ValueText;
                var inheritsFromInterface = @class.BaseList?.Types
                    .Any(type => type.ToString() == $"I{className}") ?? false;

                var methods = root.DescendantNodes().OfType<BaseMethodDeclarationSyntax>();
                foreach (var method in methods)
                {
                    // Check if the method already has an XML documentation comment
                    var xmlTrivia = method.GetLeadingTrivia()
                        .FirstOrDefault(trivia =>
                            trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                            || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));

                    var methodSignature = method.ToString();
                    if (xmlTrivia != default)
                    {
                        // Already has a comment and we shouldn't replace it.
                        if (_replaceOldDocs is false)
                            continue;

                        var xmlComment = inheritsFromInterface
                            ? "/// <inheritdoc />"
                            : await oaiHelper.GenerateXmlDocComment(methodSignature, _language);

                        fileContent = fileContent.Replace($"///{xmlTrivia}", $"{xmlComment}\n");
                    }
                    else
                    {
                        var xmlComment = inheritsFromInterface
                            ? "/// <inheritdoc />"
                            : await oaiHelper.GenerateXmlDocComment(methodSignature, _language);

                        fileContent = fileContent.Replace(methodSignature, $"{xmlComment}\n{methodSignature}");
                    }
                }
            }

            return fileContent;
        }

        /// <summary>
        /// Event handler for when the languageComboBox's selected index changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void languageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLanguage();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _replaceOldDocs = checkBox1.Checked;
        }
    }
}