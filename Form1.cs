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
        /// This constructor is called when a new instance of the Form1 class is created. It initializes the components of the form by calling the InitializeComponent method.
        /// </remarks>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the form and sets the default language to the first item in the languageComboBox. 
        /// Calls the UpdateLanguage() function to update the form's language.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>
        /// This function is called when the form is loaded. It sets the default language to the first item in the languageComboBox and updates the form's language by calling the UpdateLanguage() function.
        /// </remarks>
        private void Form1_Load(object sender, EventArgs e)
        {
            languageComboBox.SelectedIndex = 0;
            UpdateLanguage();
        }

        /// <summary>
        /// Updates the selected language in the languageComboBox and sets it as the current language.
        /// </summary>
        /// <param name="languageComboBox">The ComboBox that contains the available languages.</param>
        /// <returns>Void.</returns>
        private void UpdateLanguage() =>
                            _language = languageComboBox.SelectedItem.ToString();

        /// <summary>
        /// Handles the click event of the search button and opens a folder browser dialog to select a project path. 
        /// Sets the selected project path to the projectPathTextBox and calls the GetFilesFromProjectPath method.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
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
        /// Retrieves all C# files from the specified project path and creates a list of ProjectFile objects for each file.
        /// </summary>
        /// <remarks>
        /// The function uses the projectPathTextBox.Text property to get the project path. If the path is null, empty, or consists only of white-space characters, the function returns without doing anything. The function then uses the Directory.GetFiles method to retrieve all C# files in the specified path and its subdirectories. For each file, the function creates a new ProjectFile object and adds it to the _projectFiles list. Finally, the function calls the UpdateListBoxItems method to update the items in the list box.
        /// </remarks>
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
        /// <remarks>
        /// This function is called whenever there is a change in the _projectFiles or _toBeGeneratedFiles lists, and is responsible for updating the display of the files in the corresponding list boxes.
        /// </remarks>
        private void UpdateListBoxItems()
        {
            projectFilesListBox.Items.Clear();
            filesToGenerateListBox.Items.Clear();
            _projectFiles.ForEach(file => projectFilesListBox.Items.Add(file.DisplayName));
            _toBeGeneratedFiles.ForEach(file => filesToGenerateListBox.Items.Add(file.DisplayName));
        }

        /// <summary>
        /// Moves selected project files from the left list box to the right list box and updates the list box items.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
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
        /// Moves the selected files from the filesToGenerateListBox to the _projectFiles list.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// If no files are selected, the function returns without doing anything.
        /// </remarks>
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
        /// Generates documentation for a list of files by processing their functions and updating their content.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>Void.</returns>
        /// <remarks>
        /// This function requires a list of files to be generated and an API key to be provided. If either of these conditions are not met, the function will return without generating any documentation. The function processes each file's content by calling the ProcessFunctions method and updates the file's content if it has been modified. Once all files have been processed, the function re-enables the UI.
        /// </remarks>
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
        /// Processes the functions in a C# file and generates XML documentation comments for each method.
        /// </summary>
        /// <param name="fileContent">The content of the C# file to process.</param>
        /// <returns>A string representing the updated content of the C# file with XML documentation comments added to each method.</returns>
        /// <remarks>
        /// This method uses the Roslyn compiler to parse the syntax tree of the C# file and identify all the methods in each class. It then checks if each method already has an XML documentation comment and generates one if it does not. If the method already has a comment, it can either replace it with a new one or skip it. The method also checks if each class inherits from an interface and generates an <inheritdoc /> tag if it does.
        /// </remarks>
        public async ValueTask<string> ProcessFunctions(string fileContent)
        {
            var oaiHelper = new OaiHelper(apiKeyTextBox.Text);

            var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
            var root = await syntaxTree.GetRootAsync();

            var classes = root.DescendantNodes().OfType<TypeDeclarationSyntax>();
            foreach (var @class in classes)
            {
                var className = @class.Identifier.ValueText;
                var inheritsFromInterface = @class.BaseList?.Types
                    .Any(type => type.ToString() == $"I{className}") ?? false;

                var methods = @class.Members.OfType<BaseMethodDeclarationSyntax>();
                foreach (var method in methods)
                {
                    var oldFullMethod = method.ToFullString();
                    var methodSignature = method.ToString();

                    // Check if the method already has an XML documentation comment
                    var xmlTrivia = method.GetLeadingTrivia()
                        .FirstOrDefault(trivia =>
                            trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                            || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));

                    if (xmlTrivia != default)
                    {
                        // Already has a comment and we shouldn't replace it.
                        if (_replaceOldDocs is false)
                            continue;

                        var xmlComment = inheritsFromInterface
                            ? "/// <inheritdoc />"
                            : await oaiHelper.GenerateXmlDocComment(methodSignature, _language);

                        var newTrivia = SyntaxFactory.ParseLeadingTrivia($"{xmlComment}\n");
                        var newMethod = method.WithLeadingTrivia(newTrivia);

                        fileContent = fileContent.Replace(oldFullMethod,
                            newMethod.ToFullString());
                    }
                    else
                    {
                        var xmlComment = inheritsFromInterface
                            ? "/// <inheritdoc />"
                            : await oaiHelper.GenerateXmlDocComment(methodSignature, _language);

                        var newTrivia = SyntaxFactory.ParseLeadingTrivia($"{xmlComment}\n");
                        var newMethod = method.WithLeadingTrivia(newTrivia);

                        fileContent = fileContent.Replace(methodSignature,
                            newMethod.ToFullString());
                    }
                }
            }

            return fileContent;
        }

        /// <summary>
        /// Updates the language based on the selected item in the languageComboBox.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>Void.</returns>
        /// <remarks>
        /// This function is called when the user selects a new language from the languageComboBox. It updates the language used in the application based on the selected item. The UpdateLanguage() function is called to perform the actual update.
        /// </remarks>
        private void languageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLanguage();
        }

        /// <summary>
        /// Sets the value of the _replaceOldDocs variable to the state of the checkBox1 control.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _replaceOldDocs = checkBox1.Checked;
        }

    }
}