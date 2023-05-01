namespace AI_XML_Doc
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            apiKeyTextBox = new TextBox();
            label1 = new Label();
            groupBox1 = new GroupBox();
            checkBox1 = new CheckBox();
            languageComboBox = new ComboBox();
            label5 = new Label();
            btnSearch = new Button();
            projectPathTextBox = new TextBox();
            label2 = new Label();
            groupBox2 = new GroupBox();
            btnGenerateDocs = new Button();
            btnMoveToLeft = new Button();
            btnMoveToRight = new Button();
            label4 = new Label();
            label3 = new Label();
            filesToGenerateListBox = new ListBox();
            projectFilesListBox = new ListBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // apiKeyTextBox
            // 
            apiKeyTextBox.Location = new Point(56, 41);
            apiKeyTextBox.Name = "apiKeyTextBox";
            apiKeyTextBox.PasswordChar = '*';
            apiKeyTextBox.Size = new Size(400, 25);
            apiKeyTextBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(56, 21);
            label1.Name = "label1";
            label1.Size = new Size(98, 17);
            label1.TabIndex = 1;
            label1.Text = "OpenAI API Key";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(languageComboBox);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(btnSearch);
            groupBox1.Controls.Add(projectPathTextBox);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(apiKeyTextBox);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(11, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(512, 204);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Settings";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(56, 176);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(159, 21);
            checkBox1.TabIndex = 7;
            checkBox1.Text = "Replace old XML Docs";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // languageComboBox
            // 
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.FormattingEnabled = true;
            languageComboBox.Items.AddRange(new object[] { "ENGLISH", "BRAZILIAN PORTUGUESE", "JAPANESE" });
            languageComboBox.Location = new Point(56, 145);
            languageComboBox.Name = "languageComboBox";
            languageComboBox.Size = new Size(400, 25);
            languageComboBox.TabIndex = 6;
            languageComboBox.SelectedIndexChanged += languageComboBox_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(56, 125);
            label5.Name = "label5";
            label5.Size = new Size(128, 17);
            label5.TabIndex = 5;
            label5.Text = "Comments language";
            // 
            // btnSearch
            // 
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Location = new Point(431, 95);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(25, 25);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "S";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // projectPathTextBox
            // 
            projectPathTextBox.Location = new Point(56, 93);
            projectPathTextBox.Name = "projectPathTextBox";
            projectPathTextBox.Size = new Size(369, 25);
            projectPathTextBox.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(56, 73);
            label2.Name = "label2";
            label2.Size = new Size(78, 17);
            label2.TabIndex = 3;
            label2.Text = "Project path";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnGenerateDocs);
            groupBox2.Controls.Add(btnMoveToLeft);
            groupBox2.Controls.Add(btnMoveToRight);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(filesToGenerateListBox);
            groupBox2.Controls.Add(projectFilesListBox);
            groupBox2.Location = new Point(11, 215);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(512, 322);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            // 
            // btnGenerateDocs
            // 
            btnGenerateDocs.FlatStyle = FlatStyle.Flat;
            btnGenerateDocs.Location = new Point(106, 280);
            btnGenerateDocs.Name = "btnGenerateDocs";
            btnGenerateDocs.Size = new Size(300, 33);
            btnGenerateDocs.TabIndex = 15;
            btnGenerateDocs.Text = "GENERATE";
            btnGenerateDocs.UseVisualStyleBackColor = true;
            btnGenerateDocs.Click += btnGenerateDocs_Click;
            // 
            // btnMoveToLeft
            // 
            btnMoveToLeft.FlatStyle = FlatStyle.Flat;
            btnMoveToLeft.Location = new Point(241, 151);
            btnMoveToLeft.Name = "btnMoveToLeft";
            btnMoveToLeft.Size = new Size(30, 30);
            btnMoveToLeft.TabIndex = 14;
            btnMoveToLeft.Text = "<";
            btnMoveToLeft.UseVisualStyleBackColor = true;
            btnMoveToLeft.Click += btnMoveToLeft_Click;
            // 
            // btnMoveToRight
            // 
            btnMoveToRight.FlatStyle = FlatStyle.Flat;
            btnMoveToRight.Location = new Point(241, 115);
            btnMoveToRight.Name = "btnMoveToRight";
            btnMoveToRight.Size = new Size(30, 30);
            btnMoveToRight.TabIndex = 11;
            btnMoveToRight.Text = ">";
            btnMoveToRight.UseVisualStyleBackColor = true;
            btnMoveToRight.Click += btnMoveToRight_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(286, 12);
            label4.Name = "label4";
            label4.Size = new Size(131, 17);
            label4.TabIndex = 13;
            label4.Text = "Files to generate doc";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 12);
            label3.Name = "label3";
            label3.Size = new Size(127, 17);
            label3.TabIndex = 12;
            label3.Text = "Files on project path";
            // 
            // filesToGenerateListBox
            // 
            filesToGenerateListBox.FormattingEnabled = true;
            filesToGenerateListBox.ItemHeight = 17;
            filesToGenerateListBox.Location = new Point(286, 32);
            filesToGenerateListBox.Name = "filesToGenerateListBox";
            filesToGenerateListBox.ScrollAlwaysVisible = true;
            filesToGenerateListBox.SelectionMode = SelectionMode.MultiExtended;
            filesToGenerateListBox.Size = new Size(220, 242);
            filesToGenerateListBox.TabIndex = 10;
            // 
            // projectFilesListBox
            // 
            projectFilesListBox.FormattingEnabled = true;
            projectFilesListBox.ItemHeight = 17;
            projectFilesListBox.Location = new Point(6, 32);
            projectFilesListBox.Name = "projectFilesListBox";
            projectFilesListBox.ScrollAlwaysVisible = true;
            projectFilesListBox.SelectionMode = SelectionMode.MultiExtended;
            projectFilesListBox.Size = new Size(220, 242);
            projectFilesListBox.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(534, 546);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AI XML Doc";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox apiKeyTextBox;
        private Label label1;
        private GroupBox groupBox1;
        private Button btnSearch;
        private TextBox projectPathTextBox;
        private Label label2;
        private GroupBox groupBox2;
        private Button btnGenerateDocs;
        private Button btnMoveToLeft;
        private Button btnMoveToRight;
        private Label label4;
        private Label label3;
        private ListBox filesToGenerateListBox;
        private ListBox projectFilesListBox;
        private Label label5;
        private ComboBox languageComboBox;
        private CheckBox checkBox1;
    }
}