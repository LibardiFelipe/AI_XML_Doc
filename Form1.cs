using System.Text.RegularExpressions;
using AI_XML_Doc.Helpers;
using AI_XML_Doc.Models;

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
            string regexPattern = @"((public|private|protected|internal)\s+(static\s+)?(\w+\s+)?(\w+)\s*(<\w+>)?\s*\(([^)]*)\)\s*(\n|\r|\r\n)\s*{(?:[^{}]+|\s*(?<o>{)|\s*(?<c-o>})|(?<c>;))*\s*})";
            var matches = Regex.Matches(fileContent, regexPattern);

            foreach (var match in matches.Cast<Match>())
            {
                var oaiHelper = new OaiHelper(apiKeyTextBox.Text);
                var function = match.Value;
                var xmlComment = await oaiHelper.GenerateXmlDocComment(function, _language);

                fileContent = fileContent.Replace(function, $"{xmlComment}\n{function}");
            }

            return fileContent;
        }

        private void languageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLanguage();
        }
    }
}