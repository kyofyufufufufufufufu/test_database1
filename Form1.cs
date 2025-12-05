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

        // New dictionary to store image file path separately from the text box
        // 0=Question, 1=Opt1, 2=Opt2, 3=Opt3, 4=Opt4
        private Dictionary<int, string> pendingImagePaths = new Dictionary<int, string>();

        // Array to hold references to labels so we can update visually
        private Label[]? targetLabels;

        public Form1()
        {
            InitializeComponent();

            imageTargetTextBoxes = new TextBox[] { textBox1, textBox2, textBox4, textBox3, textBox5 };

            targetLabels = new Label[] { label19, label20, label21, label22, label23 };

            listView1.View = View.List;
            listView1.MultiSelect = false;

            database = new QuestionSet();

            // Handlers
            button3.Click += CreateQuestion_Click;
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            this.Load += Form1_Load;
            btnUpdateQuestion.Click += UpdateQuestion_Click;
            btnDeleteQuestion.Click += DeleteQuestion_Click;
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

        // Uploads the content if a local file path exists, returns tuple with both the text and image link if they are used simultaneously
        // Accepts text and file path
        private async Task<(string text, string imageLink, bool useImage)> ProcessContentAsync(string textInput, string? imagePath)
        {
            // Check if imagePath is valid
            if (IsLocalFilePath(imagePath!))
            {
                if (gitService == null)
                {
                    return (textInput, string.Empty, false);
                }

                try
                {
                    // Upload file
                    string publicUrl = await gitService.UploadImageAsync(imagePath!);

                    return (textInput, publicUrl, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading image from path: {imagePath}. Image link will not be saved. Error: {ex.Message}", "Image Upload Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return (textInput, string.Empty, false);
                }
            }
            return (textInput, string.Empty, false);
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
                        // Store path in dictionary
                        pendingImagePaths[buttonIndex] = openFileDialog.FileName;

                        // Extract just the filename (e.g., "heart.png") from the full path
                        string filename = System.IO.Path.GetFileName(openFileDialog.FileName);

                        if (targetLabels != null && targetLabels.Length > buttonIndex)
                        {
                            // Set the dedicated status label to display the filename
                            targetLabels[buttonIndex].Text = filename;

                            // Set the desired ForestGreen color
                            targetLabels[buttonIndex].ForeColor = Color.ForestGreen;
                        }
                    }
                }
            }
        }

        // Encodes the location and module into the single int
        private int EncodeLocations(int moduleIndex, CheckedListBox locationChecklist)
        {
            int bodyPartSum = 0;
            var locationMap = new Dictionary<string, int>
            {
                { "Bladder", 1 }, { "Brain", 2 }, { "Eyes", 4 }, { "GI Tract", 8 },
                { "Heart", 16 }, { "Lungs", 32 }, { "Smooth Muscle", 64 }, { "Other", 128 }
            };

            foreach (var item in locationChecklist.CheckedItems)
            {
                string? key = item?.ToString();
                if (key != null && locationMap.ContainsKey(key))
                {
                    bodyPartSum += locationMap[key];
                }
            }

            // Convert to binary string for the 8-bit body locations
            string binLocations = Convert.ToString(bodyPartSum, 2).PadLeft(8, '0');

            // Encode Module for upper 5 digits
            char[] moduleBits = new char[5] { '0', '0', '0', '0', '0' };
            if (moduleIndex >= 0 && moduleIndex < 5)
            {
                moduleBits[moduleIndex] = '1';
            }
            Array.Reverse(moduleBits);
            string binModule = new string(moduleBits);

            // Module on the left, Locations on the right
            string totalBinary = binModule + binLocations;

            return Convert.ToInt32(totalBinary, 2);
        }

        // Upload logic
        private async void CreateQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null)
            {
                MessageBox.Show("Database service is not loaded. Cannot create question.");
                return;
            }

            // Get paths from dictionary
            string? qPath = pendingImagePaths.ContainsKey(0) ? pendingImagePaths[0] : null;
            string? o1Path = pendingImagePaths.ContainsKey(1) ? pendingImagePaths[1] : null;
            string? o2Path = pendingImagePaths.ContainsKey(2) ? pendingImagePaths[2] : null;
            string? o3Path = pendingImagePaths.ContainsKey(3) ? pendingImagePaths[3] : null;
            string? o4Path = pendingImagePaths.ContainsKey(4) ? pendingImagePaths[4] : null;

            // Passes both the textbox and image's file path
            (string qText, string qImageLink, bool qUseImage) = await ProcessContentAsync(textBox1.Text, qPath);

            if (string.IsNullOrWhiteSpace(qText) && string.IsNullOrWhiteSpace(qImageLink))
            {
                MessageBox.Show("Please fill in the question text or select a valid image.", "Validation Error");
                return;
            }

            // Updates calls for options/answers
            (string o1Text, string o1Link, bool o1Use) = await ProcessContentAsync(textBox2.Text, o1Path);
            (string o2Text, string o2Link, bool o2Use) = await ProcessContentAsync(textBox4.Text, o2Path);
            (string o3Text, string o3Link, bool o3Use) = await ProcessContentAsync(textBox3.Text, o3Path);
            (string o4Text, string o4Link, bool o4Use) = await ProcessContentAsync(textBox5.Text, o4Path);

            if (string.IsNullOrWhiteSpace(o1Text) && string.IsNullOrWhiteSpace(o1Link) ||
                string.IsNullOrWhiteSpace(o2Text) && string.IsNullOrWhiteSpace(o2Link))
            {
                MessageBox.Show("Please provide at least two options.", "Validation Error");
                return;
            }

            int moduleIndex = -1;
            if (comboBox2.SelectedItem != null)
            {
                if (!int.TryParse(comboBox2.SelectedItem.ToString(), out moduleIndex) || moduleIndex < 0 || moduleIndex > 4)
                {
                    MessageBox.Show("Invalid Module selected (must be 0-4).");
                    return;
                }
            }

            // Validation for difficulty selection
            if (comboBox1.SelectedItem == null || !int.TryParse(comboBox1.SelectedItem.ToString(), out int difficultyLevel))
            {
                MessageBox.Show("Please select a Difficulty level (1-5).");
                return;
            }

            int finalLocationInt = EncodeLocations(moduleIndex, checkedListBox1);

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

        // Updating question in form
        private async void UpdateQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null)
            {
                MessageBox.Show("Database service is not loaded. Cannot update question.");
                return;
            }

            if (listView1.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Please select a question to update.", "Validation Error");
                return;
            }

            int index = listView1.SelectedIndices[0];
            var q = database.questions![index];

            if (cmbEditDifficulty.SelectedItem == null || !int.TryParse(cmbEditDifficulty.SelectedItem.ToString(), out int difficultyLevel))
            {
                MessageBox.Show("Please select a valid Difficulty level (1-5).", "Validation Error");
                return;
            }

            int moduleIndex = -1;
            if (cmbEditModule.SelectedItem != null)
            {
                if (!int.TryParse(cmbEditModule.SelectedItem.ToString(), out moduleIndex) || moduleIndex < 0 || moduleIndex > 4)
                {
                    MessageBox.Show("Invalid Module selected (must be 0-4).", "Validation Error");
                    return;
                }
            }

            // Will add 'Select Image' button at a later date for the edit question container
            string newQText = txtEditQuestion.Text.Trim();
            if (string.IsNullOrWhiteSpace(newQText) && string.IsNullOrWhiteSpace(q.imageLink))
            {
                MessageBox.Show("Question text cannot be empty if no image link exists.", "Validation Error");
                return;
            }

            string[] newOptionTexts = new string[]
            {
                txtEditOption1.Text.Trim(),
                txtEditOption2.Text.Trim(),
                txtEditOption3.Text.Trim(),
                txtEditOption4.Text.Trim()
            };

            if (newOptionTexts.Take(2).Any(string.IsNullOrWhiteSpace))
            {
                MessageBox.Show("Option 1 and Option 2 cannot be empty.", "Validation Error");
                return;
            }

            q.question = newQText;
            q.difficulty = difficultyLevel;
            q.locations = EncodeLocations(moduleIndex, clbEditLocations);

            for (int i = 0; i < q.options.Count; i++)
            {
                if (i < newOptionTexts.Length)
                {
                    q.options[i].text = newOptionTexts[i];
                }
            }

            while (q.options.Count < newOptionTexts.Length)
            {
                q.options.Add(new Option { text = newOptionTexts[q.options.Count] });
            }


            try
            {
                await gitService!.SaveDatabaseAsync(database);

                listView1.Items[index].Text = q.question;

                MessageBox.Show("Question updated and saved to GitHub!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving update to GitHub: {ex.Message}");
            }
        }

        // Delete question in database
        private async void DeleteQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null)
            {
                MessageBox.Show("Database service is not loaded. Cannot delete question.");
                return;
            }

            if (listView1.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Please select a question to delete.", "Validation Error");
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to permanently delete the selected question from the database?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }

            int index = listView1.SelectedIndices[0];

            try
            {
                database.questions!.RemoveAt(index);

                await gitService!.SaveDatabaseAsync(database);

                RefreshQuestionList();
                ClearInputFields();

                MessageBox.Show("Question successfully deleted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting question: {ex.Message}");
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

            txtEditQuestion.Text = q.question;

            txtEditOption1.Text = q.options.Count > 0 ? q.options[0].text : string.Empty;
            txtEditOption2.Text = q.options.Count > 1 ? q.options[1].text : string.Empty;
            txtEditOption3.Text = q.options.Count > 2 ? q.options[2].text : string.Empty;
            txtEditOption4.Text = q.options.Count > 3 ? q.options[3].text : string.Empty;


            cmbEditDifficulty.SelectedItem = q.difficulty.ToString();

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
            cmbEditModule.SelectedItem = module.ToString();

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

            // Clear the dictionary of images
            pendingImagePaths.Clear();

            // Reset labels to default text and color
            if (targetLabels != null)
            {
                targetLabels[0].Text = ""; // Label 19
                targetLabels[1].Text = ""; // Label 20
                targetLabels[2].Text = ""; // Label 21
                targetLabels[3].Text = ""; // Label 22
                targetLabels[4].Text = ""; // Label 23

                foreach (var lbl in targetLabels) lbl.ForeColor = Color.Black;
            }

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

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }
    }
}