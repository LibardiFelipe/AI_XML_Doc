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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            languageComboBox.SelectedIndex = 0;
            UpdateLanguage();
        }

        private void UpdateLanguage() =>
            _language = languageComboBox.SelectedItem.ToString();

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                projectPathTextBox.Text = fbd.SelectedPath;
                GetFilesFromProjectPath();
            }
        }

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

        private void UpdateListBoxItems()
        {
            projectFilesListBox.Items.Clear();
            filesToGenerateListBox.Items.Clear();
            _projectFiles.ForEach(file => projectFilesListBox.Items.Add(file.DisplayName));
            _toBeGeneratedFiles.ForEach(file => filesToGenerateListBox.Items.Add(file.DisplayName));
        }

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

        public async ValueTask<string> ProcessFunctions(string fileContent)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
            var root = syntaxTree.GetCompilationUnitRoot();

            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

            foreach (var method in methods)
            {
                var oaiHelper = new OaiHelper(apiKeyTextBox.Text);
                var methodSignature = method.ToString();
                var xmlComment = await oaiHelper.GenerateXmlDocComment(methodSignature, _language);

                // Prepend the XML documentation comment to the method
                var triviaList = new SyntaxTriviaList();
                triviaList = triviaList.Add(SyntaxFactory.ParseLeadingTrivia(xmlComment).First());
                var newMethod = method.WithLeadingTrivia(triviaList);

                // Replace the original method with the new one in the syntax tree
                root = root.ReplaceNode(method, newMethod);
            }

            return root.ToFullString();
        }

        private void languageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLanguage();
        }
    }
}