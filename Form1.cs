// Form1 logic for reading, writing and editing WinForms questions, answers and other details

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; // ADDED for file operations

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private QuestionSet? database;
        private GitHubService? gitService;
        // Array to hold references to the textboxes 
        private TextBox[]? imageTargetTextBoxes;

        public Form1()
        {
            InitializeComponent();

            imageTargetTextBoxes = new TextBox[] { textBox1, textBox2, textBox4, textBox3, textBox5 };

            listView1.View = View.List;
            listView1.MultiSelect = false;

            database = new QuestionSet();

            // Handlers
            button3.Click += CreateQuestion_Click;
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
            // This is just one option
            // I could also create a config file that can read the PAT, but I really don't want anything that's saved to
            // our group's local save
            // I'd prefer to keep it this way for now
            string token = ShowInputDialog("Please enter your GitHub Personal Access Token (PAT):", "GitHub Auth");

            if (string.IsNullOrWhiteSpace(token))
            {
                MessageBox.Show("Token is required to edit the database.");
                this.Close();
                return;
            }

            try
            {
                gitService = new GitHubService(token);
                database = await gitService!.GetDatabaseAsync();
                RefreshQuestionList();

                // Clear fields for new input after loading
                ClearInputFields();

                // Edit panel is hidden until question selected
                if (gbEditQuestion != null)
                {
                    gbEditQuestion.Visible = false;
                }

                MessageBox.Show("Connected to GitHub and loaded database! Ready to create new questions.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load: {ex.Message}");
            }
        }

        private void RefreshQuestionList()
        {
            listView1.Items.Clear();
            if (database?.questions != null)
            {
                foreach (var q in database.questions)
                {
                    listView1.Items.Add(q.question);
                }
            }
        }

        // Checks if a path is a local file that actually exists
        private bool IsLocalFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            // Checks for URL
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return false;

            return Path.IsPathRooted(path) && File.Exists(path);
        }

        // Uploads the content if it's a local file path, returns tuple
        private async Task<(string text, string imageLink, bool useImage)> ProcessContentAsync(string input)
        {
            if (IsLocalFilePath(input))
            {
                if (gitService == null)
                {
                    return (string.Empty, string.Empty, false);
                }

                try
                {
                    // Upload file
                    string publicUrl = await gitService.UploadImageAsync(input);
                    // Text is empty, imageLink is the URL, and useImage is true
                    return (string.Empty, publicUrl, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading image from path: {input}. Image link will not be saved. Error: {ex.Message}", "Image Upload Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return (input, string.Empty, false);
                }
            }
            return (input, string.Empty, false);
        }

        private void ImageButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button clickedButton && imageTargetTextBoxes != null)
            {
                int buttonIndex = -1;

                if (clickedButton == button1) buttonIndex = 0; // Question Text
                else if (clickedButton == button5) buttonIndex = 1; // Option 1 Text
                else if (clickedButton == button7) buttonIndex = 2; // Option 2 Text
                else if (clickedButton == button11) buttonIndex = 3; // Option 3 Text
                else if (clickedButton == button9) buttonIndex = 4; // Option 4 Text

                if (buttonIndex == -1) return;

                // Open upload dialog box
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*";
                    openFileDialog.Title = $"Select Image for Option {buttonIndex + 1}";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Stores full path in the textbox
                        imageTargetTextBoxes[buttonIndex].Text = openFileDialog.FileName;
                    }
                }
            }
        }

        // Upload logic
        private async void CreateQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null)
            {
                MessageBox.Show("Database service is not loaded. Cannot create question.");
                return;
            }

            (string qText, string qImageLink, bool qUseImage) = await ProcessContentAsync(textBox1.Text);

            if (string.IsNullOrWhiteSpace(qText) && string.IsNullOrWhiteSpace(qImageLink))
            {
                MessageBox.Show("Please fill in the question text or select a valid image.", "Validation Error");
                return;
            }

            (string o1Text, string o1Link, bool o1Use) = await ProcessContentAsync(textBox2.Text);
            (string o2Text, string o2Link, bool o2Use) = await ProcessContentAsync(textBox4.Text);
            (string o3Text, string o3Link, bool o3Use) = await ProcessContentAsync(textBox3.Text);
            (string o4Text, string o4Link, bool o4Use) = await ProcessContentAsync(textBox5.Text);

            if (string.IsNullOrWhiteSpace(o1Text) && string.IsNullOrWhiteSpace(o1Link) ||
                string.IsNullOrWhiteSpace(o2Text) && string.IsNullOrWhiteSpace(o2Link))
            {
                MessageBox.Show("Please provide at least two options.", "Validation Error");
                return;
            }

            int bodyPartSum = 0;
            var locationMap = new Dictionary<string, int>
            {
                { "Bladder", 1 }, { "Brain", 2 }, { "Eyes", 4 }, { "GI Tract", 8 },
                { "Heart", 16 }, { "Lungs", 32 }, { "Smooth Muscle", 64 }, { "Other", 128 }
            };

            foreach (var item in checkedListBox1.CheckedItems)
            {
                string? key = item?.ToString();
                if (key != null && locationMap.ContainsKey(key))
                {
                    bodyPartSum += locationMap[key];
                }
            }

            // Encode Module for upper 5 digits when converting to Unity project's Database.cs
            int moduleIndex = 0;
            if (comboBox2.SelectedItem != null)
            {
                if (!int.TryParse(comboBox2.SelectedItem.ToString(), out moduleIndex))
                {
                    MessageBox.Show("Invalid Module selected.");
                    return;
                }
            }

            // Validation for difficulty selection
            if (comboBox1.SelectedItem == null || !int.TryParse(comboBox1.SelectedItem.ToString(), out int difficultyLevel))
            {
                MessageBox.Show("Please select a Difficulty level (1-5).");
                return;
            }

            string binLocations = Convert.ToString(bodyPartSum, 2).PadLeft(8, '0');

            // Used fixed-size string to represent the 5-bit module part
            char[] moduleBits = new char[5] { '0', '0', '0', '0', '0' };
            if (moduleIndex >= 0 && moduleIndex < 5)
            {
                moduleBits[moduleIndex] = '1';
            }
            Array.Reverse(moduleBits);
            string binModule = new string(moduleBits);

            // Combine/Convert to 13 bits total
            string totalBinary = binModule + binLocations;
            int finalLocationInt = Convert.ToInt32(totalBinary, 2);

            // Build Object
            Question newQ = new Question
            {
                question = qText,
                imageLink = qImageLink,
                options = new List<Option>
                {
                    new Option { text = o1Text, imageLink = o1Link, useImage = o1Use },
                    new Option { text = o2Text, imageLink = o2Link, useImage = o2Use },
                    new Option { text = o3Text, imageLink = o3Link, useImage = o3Use },
                    new Option { text = o4Text, imageLink = o4Link, useImage = o4Use }
                },
                answerIndex = 0, // Assumes the correct answer is always option 1/textBox2
                difficulty = difficultyLevel,
                locations = finalLocationInt
            };

            database.questions.Add(newQ);

            // Upload to JSON database
            try
            {
                await gitService.SaveDatabaseAsync(database);
                RefreshQuestionList();
                MessageBox.Show("Question saved to GitHub!");
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving to GitHub: {ex.Message}");
            }
        }

        // Handles displaying question data when an item is selected from the list
        private void ListView1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (database == null) return;

            if (listView1.SelectedIndices.Count == 0)
            {
                ClearInputFields();
                if (gbEditQuestion != null) gbEditQuestion.Visible = false;
                return;
            }

            if (gbEditQuestion != null) gbEditQuestion.Visible = true;

            int index = listView1.SelectedIndices[0];

            if (index < 0 || database.questions!.Count <= index) return;

            var q = database.questions![index];

            // Populate controls
            txtEditQuestion.Text = q.question;
            if (q.options.Count >= 4)
            {
                txtEditOption1.Text = q.options[0].text;
                txtEditOption2.Text = q.options[1].text;
                txtEditOption3.Text = q.options[2].text;
                txtEditOption4.Text = q.options[3].text;
            }

            cmbEditDifficulty.SelectedItem = q.difficulty.ToString()!;

            // Decodes Locations for UI
            string bin = Convert.ToString(q.locations, 2).PadLeft(13, '0');
            string binModule = bin.Substring(0, 5);
            string binLocations = bin.Substring(5);

            // Decodes Module
            char[] charArray = binModule.ToCharArray();
            Array.Reverse(charArray);
            string reversedBinModule = new string(charArray);

            int module = 0;
            for (int i = 0; i < reversedBinModule.Length; i++)
            {
                if (reversedBinModule[i] == '1')
                {
                    module = i;
                    break;
                }
            }
            cmbEditModule.SelectedItem = module.ToString()!;

            // Decodes Body Parts
            int locationVal = Convert.ToInt32(binLocations, 2);
            var locationMap = new Dictionary<string, int>
            {
                { "Bladder", 1 }, { "Brain", 2 }, { "Eyes", 4 }, { "GI Tract", 8 },
                { "Heart", 16 }, { "Lungs", 32 }, { "Smooth Muscle", 64 }, { "Other", 128 }
            };

            for (int i = 0; i < clbEditLocations.Items.Count; i++)
            {
                string? name = clbEditLocations.Items[i]?.ToString();
                if (name != null && locationMap.ContainsKey(name))
                {
                    int val = locationMap[name];
                    bool isChecked = (locationVal & val) == val;
                    clbEditLocations.SetItemChecked(i, isChecked);
                }
            }
        }

        // Clears both Create and Edit fields
        private void ClearInputFields()
        {
            // Clear Create Question fields
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }

            // Clear Edit Question fields
            if (gbEditQuestion != null)
            {
                txtEditQuestion.Text = "";
                txtEditOption1.Text = "";
                txtEditOption2.Text = "";
                txtEditOption3.Text = "";
                txtEditOption4.Text = "";
                cmbEditDifficulty.SelectedIndex = -1;
                cmbEditModule.SelectedIndex = -1;

                for (int i = 0; i < clbEditLocations.Items.Count; i++)
                {
                    clbEditLocations.SetItemChecked(i, false);
                }
            }
        }

        // Custom Input Box helper
        public static string ShowInputDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 400 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}