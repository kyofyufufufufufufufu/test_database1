// Form1 logic for reading, writing and editing WinForms questions, answers that are saved to the json database
// Includes a duplicate Question button for adding multipe similar questions/options
// Includes a bulk upload feature for adding large numbers of questions at once via CSV, with image support as well

using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private QuestionSet? database;
        private GitHubService? gitService;
        private TextBox[]? imageTargetTextBoxes;

        // 0-4 = Create Tab, 5-9 = Edit Tab
        private Dictionary<int, string> pendingImagePaths = new Dictionary<int, string>();
        private Label[]? targetLabels;

        public Form1()
        {
            InitializeComponent();

            imageTargetTextBoxes = new TextBox[] { textBox1, textBox2, textBox4, textBox3, textBox5, textBox7 };
            targetLabels = new Label[] { label19, label20, label21, label22, label23, label32 };

            listView1.View = View.List;
            listView1.MultiSelect = false;

            database = new QuestionSet();

            // Handlers
            button3.Click += CreateQuestion_Click;
            button14.Click += DuplicateQuestion_Click;
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            this.Load += Form1_Load;
            btnUpdateQuestion.Click += UpdateQuestion_Click;
            btnDeleteQuestion.Click += DeleteQuestion_Click;

            // Select Image buttons for the Edit tab
            button16.Click += ImageButton_Click;
            button18.Click += ImageButton_Click;
            button20.Click += ImageButton_Click;
            button22.Click += ImageButton_Click;
            button24.Click += ImageButton_Click;
            button26.Click += ImageButton_Click;
            button28.Click += ImageButton_Click;

            // Clear buttons for the Edit tab
            button15.Click += ClearImage_Click;
            button17.Click += ClearImage_Click;
            button19.Click += ClearImage_Click;
            button21.Click += ClearImage_Click;
            button23.Click += ClearImage_Click;
            button25.Click += ClearImage_Click;
            button27.Click += ClearImage_Click;

            // Bulk upload for Create tab
            button12.Click += button12_Click;
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
            // A PAT is required in order for the user to access the form
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
                database = await gitService.GetDatabaseAsync(); //
                RefreshQuestionList();

                ClearInputFields();

                if (gbEditQuestion != null)
                {
                    gbEditQuestion.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load: {ex.Message}");
                this.Close();
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

        private bool IsLocalFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return false;
            // Updated to catch cross-platform path strings
            return Path.IsPathRooted(path) || path.Contains("\\");
        }

        private async Task<(string text, string imageLink, bool useImage)> ProcessContentAsync(string textInput, string? imagePath)
        {
            // if the path exists or looks like a local path, process it through the universal uploader
            if (!string.IsNullOrEmpty(imagePath))
            {
                string uploadedUrl = await HandleImageUpload(imagePath);
                return (textInput, uploadedUrl, !string.IsNullOrEmpty(uploadedUrl));
            }
            return (textInput, string.Empty, false);
        }

        private void ImageButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button clickedButton)
            {
                int buttonIndex = -1;
                if (clickedButton == button1) buttonIndex = 0;
                else if (clickedButton == button5) buttonIndex = 1;
                else if (clickedButton == button7) buttonIndex = 2;
                else if (clickedButton == button11) buttonIndex = 3;
                else if (clickedButton == button9) buttonIndex = 4;
                else if (clickedButton == button16) buttonIndex = 5;
                else if (clickedButton == button18) buttonIndex = 6;
                else if (clickedButton == button20) buttonIndex = 7;
                else if (clickedButton == button22) buttonIndex = 8;
                else if (clickedButton == button24) buttonIndex = 9;
                else if (clickedButton == button26) buttonIndex = 10;
                else if (clickedButton == button28) buttonIndex = 11;

                if (buttonIndex == -1) return;

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pendingImagePaths[buttonIndex] = openFileDialog.FileName;
                        Label? targetLabel = GetLabelByIndex(buttonIndex);
                        if (targetLabel != null)
                        {
                            targetLabel.Text = Path.GetFileName(openFileDialog.FileName);
                            targetLabel.ForeColor = Color.ForestGreen;
                        }
                    }
                }
            }
        }

        private void ClearImage_Click(object? sender, EventArgs e)
        {
            if (sender is Button clickedButton)
            {
                int index = -1;
                if (clickedButton == button15) index = 5;
                else if (clickedButton == button17) index = 6;
                else if (clickedButton == button19) index = 7;
                else if (clickedButton == button21) index = 8;
                else if (clickedButton == button23) index = 9;
                else if (clickedButton == button27) index = 10;

                if (index != -1)
                {
                    pendingImagePaths.Remove(index);
                    Label? targetLabel = GetLabelByIndex(index);
                    if (targetLabel != null)
                    {
                        targetLabel.Text = "";
                        targetLabel.ForeColor = Color.Black;
                    }
                }
            }
        }

        private Label? GetLabelByIndex(int index)
        {
            return index switch
            {
                0 => label19,
                1 => label20,
                2 => label21,
                3 => label22,
                4 => label23,
                5 => label24,
                6 => label25,
                7 => label26,
                8 => label27,
                9 => label28,
                10 => label34,
                11 => label34,
                _ => null
            };
        }

        private int EncodeLocations(int moduleIndex, CheckedListBox locationChecklist)
        {
            // Body Parts (8 bits)
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
                    bodyPartSum += locationMap[key];
            }

            // Module (5 bits) 
            int moduleValue = 0;
            if (moduleIndex >= 0 && moduleIndex < 5)
            {
                moduleValue = (1 << moduleIndex);
            }

            // Combine Module in high bits, Locations in low bits
            return (moduleValue << 8) | bodyPartSum;
        }

        private async void CreateQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null) return;

            // All image paths now pass through ProcessContentAsync which uses the passive HandleImageUpload logic
            string? qPath = pendingImagePaths.ContainsKey(0) ? pendingImagePaths[0] : null;
            var (qText, qLink, qUse) = await ProcessContentAsync(textBox1.Text, qPath);

            var o1 = await ProcessContentAsync(textBox2.Text, pendingImagePaths.ContainsKey(1) ? pendingImagePaths[1] : null);
            var o2 = await ProcessContentAsync(textBox4.Text, pendingImagePaths.ContainsKey(2) ? pendingImagePaths[2] : null);
            var o3 = await ProcessContentAsync(textBox3.Text, pendingImagePaths.ContainsKey(3) ? pendingImagePaths[3] : null);
            var o4 = await ProcessContentAsync(textBox5.Text, pendingImagePaths.ContainsKey(4) ? pendingImagePaths[4] : null);
            var o5 = await ProcessContentAsync(textBox7.Text, pendingImagePaths.ContainsKey(5) ? pendingImagePaths[5] : null);

            // Option 1 is marked as the correct answer, this will always be the case for each question
            // Will be randomized in the Unity game, so no need for user input on this part
            int correctIdx = 0;

            var newQ = new Question
            {
                question = qText,
                imageLink = qLink,
                difficulty = comboBox1.SelectedIndex + 1,
                locations = EncodeLocations(comboBox2.SelectedIndex, checkedListBox1),
                answerIndex = correctIdx,
                minigameType = comboBox3.Text == "" ? "None" : comboBox3.Text,
                options = new List<Option>
                {
                    new Option { text = o1.text, imageLink = o1.imageLink, useImage = o1.useImage },
                    new Option { text = o2.text, imageLink = o2.imageLink, useImage = o2.useImage },
                    new Option { text = o3.text, imageLink = o3.imageLink, useImage = o3.useImage },
                    new Option { text = o4.text, imageLink = o4.imageLink, useImage = o4.useImage },

                }
            };

            if (!string.IsNullOrWhiteSpace(textBox7.Text))
            {
                newQ.options.Add(new Option
                {
                    text = o5.text,
                    imageLink = o5.imageLink,
                    useImage = o5.useImage
                });
            }

            database.questions.Add(newQ);
            await gitService.SaveDatabaseAsync(database);
            RefreshQuestionList();
            MessageBox.Show("Question successfully added to database.");
            ClearInputFields();
        }

        private async void DuplicateQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null) return;

            // Ask how many copies
            string input = ShowInputDialog("How many copies of this current form would you like to create?", "Mass Create Questions");

            if (int.TryParse(input, out int count) && count > 0)
            {
                // Hard limit is 50 to prevent issues
                if (count > 50) count = 50;

                // Process images first, upload once
                string? qPath = pendingImagePaths.ContainsKey(0) ? pendingImagePaths[0] : null;
                var (qText, qLink, qUse) = await ProcessContentAsync(textBox1.Text, qPath);

                var o1 = await ProcessContentAsync(textBox2.Text, pendingImagePaths.ContainsKey(1) ? pendingImagePaths[1] : null);
                var o2 = await ProcessContentAsync(textBox4.Text, pendingImagePaths.ContainsKey(2) ? pendingImagePaths[2] : null);
                var o3 = await ProcessContentAsync(textBox3.Text, pendingImagePaths.ContainsKey(3) ? pendingImagePaths[3] : null);
                var o4 = await ProcessContentAsync(textBox5.Text, pendingImagePaths.ContainsKey(4) ? pendingImagePaths[4] : null);

                // Add number of copies to the database
                for (int i = 0; i < count; i++)
                {
                    var newQ = new Question
                    {
                        question = qText + (i > 0 ? $" ({i + 1})" : ""),
                        imageLink = qLink,
                        difficulty = comboBox1.SelectedIndex + 1,
                        locations = EncodeLocations(comboBox2.SelectedIndex, checkedListBox1),
                        answerIndex = 0,
                        minigameType = comboBox3.Text == "" ? "None" : comboBox3.Text,
                        options = new List<Option>
                {
                    new Option { text = o1.text, imageLink = o1.imageLink, useImage = o1.useImage },
                    new Option { text = o2.text, imageLink = o2.imageLink, useImage = o2.useImage },
                    new Option { text = o3.text, imageLink = o3.imageLink, useImage = o3.useImage },
                    new Option { text = o4.text, imageLink = o4.imageLink, useImage = o4.useImage }
                }
                    };

                    database.questions.Add(newQ);
                }

                await gitService.SaveDatabaseAsync(database);
                RefreshQuestionList();

                MessageBox.Show($"{count} questions successfully added to database.");
                ClearInputFields();
            }
        }

        // View container for live database
        private void ListView1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (database == null || listView1.SelectedIndices.Count == 0)
            {
                if (gbEditQuestion != null) gbEditQuestion.Visible = false;
                return;
            }

            gbEditQuestion.Visible = true;
            var q = database.questions[listView1.SelectedIndices[0]];

            // Questions and answers
            txtEditQuestion.Text = q.question;
            txtEditOption1.Text = q.options.Count > 0 ? q.options[0].text : "";
            txtEditOption2.Text = q.options.Count > 1 ? q.options[1].text : "";
            txtEditOption3.Text = q.options.Count > 2 ? q.options[2].text : "";
            txtEditOption4.Text = q.options.Count > 3 ? q.options[3].text : "";
            txtEditOption5.Text = q.options.Count > 4 ? q.options[4].text : "";

            // Image links
            label24.Text = Path.GetFileName(q.imageLink);
            label25.Text = q.options.Count > 0 ? Path.GetFileName(q.options[0].imageLink) : "";
            label26.Text = q.options.Count > 1 ? Path.GetFileName(q.options[1].imageLink) : "";
            label27.Text = q.options.Count > 2 ? Path.GetFileName(q.options[2].imageLink) : "";
            label28.Text = q.options.Count > 3 ? Path.GetFileName(q.options[3].imageLink) : "";
            label34.Text = q.options.Count > 4 ? Path.GetFileName(q.options[4].imageLink) : "";

            foreach (var lbl in new[] { label24, label25, label26, label27, label28, label34 })
                lbl.ForeColor = Color.Black;

            // Minigame
            comboBox4.Text = string.IsNullOrEmpty(q.minigameType) ? "None" : q.minigameType;

            // Decode locations for Module and Locations
            int packed = q.locations;
            int bodyPartSum = packed & 0xFF;
            int moduleValue = packed >> 8;

            int modIdx = 0;
            if (moduleValue > 0)
            {
                int temp = moduleValue;
                while (temp > 1)
                {
                    temp >>= 1;
                    modIdx++;
                }
            }
            cmbEditModule.SelectedIndex = modIdx;

            // Difficulty
            if (q.difficulty >= 1 && q.difficulty <= 5)
                cmbEditDifficulty.SelectedIndex = q.difficulty - 1;
            else
                cmbEditDifficulty.SelectedIndex = -1;

            // Body Part checklist
            for (int i = 0; i < clbEditLocations.Items.Count; i++)
            {
                int flag = (1 << i);
                clbEditLocations.SetItemChecked(i, (bodyPartSum & flag) != 0);
            }
        }

        // Updates question to live database
        private async void UpdateQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null || listView1.SelectedIndices.Count == 0) return;

            var q = database.questions[listView1.SelectedIndices[0]];

            // Passive upload handling for editing existing questions
            // If the user selects a new image, it will upload and replace the link
            // If the user leaves the image as is, it will keep the existing link
            if (pendingImagePaths.ContainsKey(5))
                (_, q.imageLink, _) = await ProcessContentAsync(txtEditQuestion.Text, pendingImagePaths[5]);
            else if (string.IsNullOrEmpty(label24.Text)) q.imageLink = "";

            q.question = txtEditQuestion.Text;
            q.difficulty = cmbEditDifficulty.SelectedIndex + 1;
            q.locations = EncodeLocations(cmbEditModule.SelectedIndex, clbEditLocations);
            q.minigameType = comboBox4.Text == "" ? "None" : comboBox4.Text;

            for (int i = 0; i < 4; i++)
            {
                // If for some reason an old question has < 4 options, add blank ones to prevent crashes
                if (q.options.Count <= i) q.options.Add(new Option());

                TextBox tb = i == 0 ? txtEditOption1 : i == 1 ? txtEditOption2 : i == 2 ? txtEditOption3 : txtEditOption4;
                Label lbl = i == 0 ? label25 : i == 1 ? label26 : i == 2 ? label27 : label28;

                q.options[i].text = tb.Text;
                if (pendingImagePaths.ContainsKey(i + 6))
                    (_, q.options[i].imageLink, q.options[i].useImage) = await ProcessContentAsync(tb.Text, pendingImagePaths[i + 6]);
                else if (string.IsNullOrEmpty(lbl.Text)) { q.options[i].imageLink = ""; q.options[i].useImage = false; }
            }

            //  Handle the 5th Option
            if (!string.IsNullOrWhiteSpace(txtEditOption5.Text))
            {
                // If Option 5 doesn't exist, create it and make it null/empty string
                if (q.options.Count < 5) q.options.Add(new Option());

                q.options[4].text = txtEditOption5.Text;

                // Check for new image
                if (pendingImagePaths.ContainsKey(11))
                    (_, q.options[4].imageLink, q.options[4].useImage) = await ProcessContentAsync(txtEditOption5.Text, pendingImagePaths[11]);
                else if (string.IsNullOrEmpty(label34.Text)) { q.options[4].imageLink = ""; q.options[4].useImage = false; }
            }
            else if (q.options.Count >= 5)
            {
                // If the text box is empty but the option exists in the list, remove it
                q.options.RemoveAt(4);
            }

            await gitService.SaveDatabaseAsync(database);
            for (int i = 5; i <= 11; i++) pendingImagePaths.Remove(i);
            RefreshQuestionList();
            MessageBox.Show("Successfully updated question to database.");
        }

        private async void DeleteQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null || listView1.SelectedIndices.Count == 0) return;

            var result = MessageBox.Show("Are you sure you want to delete this question? This will also remove its images from GitHub.",
                                         "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                int selectedIndex = listView1.SelectedIndices[0];
                var questionToDelete = database.questions[selectedIndex];

                // Delete main question image
                if (!string.IsNullOrEmpty(questionToDelete.imageLink))
                {
                    await gitService.DeleteImageAsync(questionToDelete.imageLink);
                }

                // Delete images for each option
                foreach (var option in questionToDelete.options)
                {
                    if (!string.IsNullOrEmpty(option.imageLink))
                    {
                        await gitService.DeleteImageAsync(option.imageLink);
                    }
                }

                database.questions.RemoveAt(selectedIndex);
                await gitService.SaveDatabaseAsync(database);

                RefreshQuestionList();
                gbEditQuestion.Visible = false;
                MessageBox.Show("Question and associated images deleted successfully.");
            }
        }

        // Clear all fields after submission
        private void ClearInputFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox7.Clear();

            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;

            if (targetLabels != null)
            {
                foreach (var lbl in targetLabels)
                {
                    lbl.Text = "";
                    lbl.ForeColor = Color.Black;
                }
            }

            // Clear only the Create tab's images
            for (int i = 0; i <= 5; i++)
            {
                pendingImagePaths.Remove(i);
            }

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }

        // REQUIRED PLACEHOLDERS FOR DESIGNER EVENTS
        private void label17_Click(object sender, EventArgs e) { }
        private void button13_Click(object sender, EventArgs e) { }
        private void label18_Click(object sender, EventArgs e) { }
        private void label19_Click(object sender, EventArgs e) { }

        public static string ShowInputDialog(string text, string caption)
        {
            Form prompt = new Form() { Width = 500, Height = 150, Text = caption, StartPosition = FormStartPosition.CenterScreen };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 400 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            prompt.Controls.Add(textBox); prompt.Controls.Add(confirmation); prompt.Controls.Add(textLabel);
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        // Bulk Upload Section

        // Button logic for Bulk Upload
        private async void button12_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null)
            {
                MessageBox.Show("Please wait for the database to load first.");
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog { Filter = "CSV Files|*.csv" };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(ofd.FileName))
                {
                    parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.HasFieldsEnclosedInQuotes = true;

                    if (!parser.EndOfData) parser.ReadFields(); // Skip Header

                    while (!parser.EndOfData)
                    {
                        string[]? columns = parser.ReadFields();
                        if (columns == null || columns.Length == 0) continue;

                        // --- THE SAFETY NET ---
                        // This local function prevents IndexOutOfRangeException forever.
                        string GetValue(int index) => (columns.Length > index) ? columns[index]?.Trim() ?? "" : "";

                        string questionText = GetValue(0);
                        if (string.IsNullOrWhiteSpace(questionText)) continue;

                        // Pre-process images and build options before the duplicate check
                        // so that local paths are already resolved to GitHub URLs for comparison
                        string resolvedQuestionImage = await HandleImageUpload(GetValue(1));

                        var resolvedOptions = new List<Option>();
                        for (int i = 0; i < 5; i++)
                        {
                            string txt = GetValue(2 + (i * 2));
                            string img = GetValue(3 + (i * 2));
                            if (!string.IsNullOrWhiteSpace(txt) || !string.IsNullOrWhiteSpace(img))
                            {
                                resolvedOptions.Add(new Option
                                {
                                    text = txt,
                                    imageLink = await HandleImageUpload(img),
                                    useImage = !string.IsNullOrWhiteSpace(img)
                                });
                            }
                        }

                        // Duplicate Check compares resolved URLs so local paths and GitHub URLs always match
                        var incomingOptions = resolvedOptions
                            .Select(o => (o.text?.ToLowerInvariant() ?? "") + "|" + (o.imageLink?.ToLowerInvariant() ?? ""))
                            .ToList();

                        bool isDuplicate = database.questions.Any(q =>
                        {
                            if (!q.question.Equals(questionText, StringComparison.OrdinalIgnoreCase))
                                return false;

                            var existingOptions = q.options
                                .Select(o => (o.text?.ToLowerInvariant() ?? "") + "|" + (o.imageLink?.ToLowerInvariant() ?? ""))
                                .ToList();

                            return incomingOptions.Count == existingOptions.Count &&
                                   incomingOptions.All(o => existingOptions.Contains(o));
                        });

                        if (isDuplicate) continue;

                        var newQuestion = new Question { question = questionText };
                        newQuestion.imageLink = resolvedQuestionImage;
                        newQuestion.options.AddRange(resolvedOptions);

                        // Metadata - TryParse handles "None" or empty strings without crashing
                        int.TryParse(GetValue(12), out int ansIdx);
                        newQuestion.answerIndex = ansIdx;

                        int.TryParse(GetValue(13), out int diff);
                        newQuestion.difficulty = diff;

                        string mini = GetValue(14);
                        newQuestion.minigameType = string.IsNullOrWhiteSpace(mini) ? "MultipleChoice" : mini;

                        // These were the crash-prone lines! Now they use GetValue and TryParse.
                        int.TryParse(GetValue(15), out int mod);
                        string locs = GetValue(16);

                        newQuestion.locations = QuestionSet.EncodeLocations(mod, locs);
                        database.questions.Add(newQuestion);
                    }
                }

                // Refresh the JSON file's SHA before saving. Image uploads during the loop
                await gitService.RefreshShaAsync();
                await gitService.SaveDatabaseAsync(database);
                RefreshQuestionList();
                MessageBox.Show("Bulk Upload Successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\nTrace: {ex.StackTrace}");
            }
        }

        // Helper to decide if we need to upload to GitHub or just use a URL
        private async Task<string> HandleImageUpload(string pathOrUrl)
        {
            // Added null check and URL bypass
            if (string.IsNullOrWhiteSpace(pathOrUrl)) return "";
            if (pathOrUrl.StartsWith("http")) return pathOrUrl;

            // Check if the file exists on the CURRENT machine to attempt an upload
            if (File.Exists(pathOrUrl))
            {
                return await gitService.UploadImageAsync(pathOrUrl);
            }
            // If file isn't found but has backslashes, it's a local path from another PC.
            // We strip the path to just the filename and point it to the GitHub images folder.
            else if (pathOrUrl.Contains("\\"))
            {
                string fileName = Path.GetFileName(pathOrUrl);
                return $"https://raw.githubusercontent.com/kyofyufufufufufufufu/test_database1/main/images/{fileName}";
            }

            return pathOrUrl;
        }
    }
}