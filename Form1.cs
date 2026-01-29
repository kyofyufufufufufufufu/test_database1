// Form1 logic for reading, writing and editing WinForms questions, answers

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
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            this.Load += Form1_Load;
            btnUpdateQuestion.Click += UpdateQuestion_Click;
            btnDeleteQuestion.Click += DeleteQuestion_Click;

            // Wire up Select Image buttons for the Edit tab
            button16.Click += ImageButton_Click;
            button18.Click += ImageButton_Click;
            button20.Click += ImageButton_Click;
            button22.Click += ImageButton_Click;
            button24.Click += ImageButton_Click;

            // Wire up Clear buttons for the Edit tab
            button15.Click += ClearImage_Click;
            button17.Click += ClearImage_Click;
            button19.Click += ClearImage_Click;
            button21.Click += ClearImage_Click;
            button23.Click += ClearImage_Click;
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
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
            return Path.IsPathRooted(path) && File.Exists(path);
        }

        private async Task<(string text, string imageLink, bool useImage)> ProcessContentAsync(string textInput, string? imagePath)
        {
            if (IsLocalFilePath(imagePath!))
            {
                if (gitService == null) return (textInput, string.Empty, false);
                try
                {
                    string publicUrl = await gitService.UploadImageAsync(imagePath!); //
                    return (textInput, publicUrl, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error uploading image: {ex.Message}");
                    return (textInput, string.Empty, false);
                }
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
                _ => null
            };
        }

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
            string binLocations = Convert.ToString(bodyPartSum, 2).PadLeft(8, '0');
            char[] moduleBits = new char[5] { '0', '0', '0', '0', '0' };
            if (moduleIndex >= 0 && moduleIndex < 5) moduleBits[moduleIndex] = '1';
            Array.Reverse(moduleBits);
            string binModule = new string(moduleBits);
            return Convert.ToInt32(binModule + binLocations, 2);
        }

        private async void CreateQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null) return;

            string? qPath = pendingImagePaths.ContainsKey(0) ? pendingImagePaths[0] : null;
            var (qText, qLink, qUse) = await ProcessContentAsync(textBox1.Text, qPath);

            var o1 = await ProcessContentAsync(textBox2.Text, pendingImagePaths.ContainsKey(1) ? pendingImagePaths[1] : null);
            var o2 = await ProcessContentAsync(textBox4.Text, pendingImagePaths.ContainsKey(2) ? pendingImagePaths[2] : null);
            var o3 = await ProcessContentAsync(textBox3.Text, pendingImagePaths.ContainsKey(3) ? pendingImagePaths[3] : null);
            var o4 = await ProcessContentAsync(textBox5.Text, pendingImagePaths.ContainsKey(4) ? pendingImagePaths[4] : null);
            var o5 = await ProcessContentAsync(textBox7.Text, pendingImagePaths.ContainsKey(5) ? pendingImagePaths[5] : null);

            // Based on current Designer labels, Option 1 is marked as "(Correct)"
            int correctIdx = 0;

            var newQ = new Question
            {
                question = qText,
                imageLink = qLink,
                difficulty = comboBox2.SelectedIndex + 1,
                locations = EncodeLocations(comboBox1.SelectedIndex, checkedListBox1),
                answerIndex = correctIdx, //
                options = new List<Option>
                {
                    new Option { text = o1.text, imageLink = o1.imageLink, useImage = o1.useImage },
                    new Option { text = o2.text, imageLink = o2.imageLink, useImage = o2.useImage },
                    new Option { text = o3.text, imageLink = o3.imageLink, useImage = o3.useImage },
                    new Option { text = o4.text, imageLink = o4.imageLink, useImage = o4.useImage }
                }
            };

            database.questions.Add(newQ);
            await gitService.SaveDatabaseAsync(database); //
            RefreshQuestionList();
            ClearInputFields();
        }

        private void ListView1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (database == null || listView1.SelectedIndices.Count == 0)
            {
                if (gbEditQuestion != null) gbEditQuestion.Visible = false;
                return;
            }

            gbEditQuestion.Visible = true;
            var q = database.questions[listView1.SelectedIndices[0]];

            txtEditQuestion.Text = q.question;
            txtEditOption1.Text = q.options.Count > 0 ? q.options[0].text : "";
            txtEditOption2.Text = q.options.Count > 1 ? q.options[1].text : "";
            txtEditOption3.Text = q.options.Count > 2 ? q.options[2].text : "";
            txtEditOption4.Text = q.options.Count > 3 ? q.options[3].text : "";
            txtEditOption5.Text = q.options.Count > 4 ? q.options[4].text : "";

            label24.Text = Path.GetFileName(q.imageLink);
            label25.Text = q.options.Count > 0 ? Path.GetFileName(q.options[0].imageLink) : "";
            label26.Text = q.options.Count > 1 ? Path.GetFileName(q.options[1].imageLink) : "";
            label27.Text = q.options.Count > 2 ? Path.GetFileName(q.options[2].imageLink) : "";
            label28.Text = q.options.Count > 3 ? Path.GetFileName(q.options[3].imageLink) : "";
            label34.Text = q.options.Count > 3 ? Path.GetFileName(q.options[4].imageLink) : "";

            Label[] editLabels = { label24, label25, label26, label27, label28, label34 };
            foreach (var lbl in editLabels) lbl.ForeColor = Color.Black;

            // Decode packed integer for Module and Locations
            string binary = Convert.ToString(q.locations, 2).PadLeft(13, '0');
            string modBits = binary.Substring(0, 5);
            string locBits = binary.Substring(5);

            char[] modArr = modBits.ToCharArray();
            Array.Reverse(modArr);
            int modIdx = new string(modArr).IndexOf('1');
            cmbEditModule.SelectedIndex = modIdx != -1 ? modIdx : 0;

            int locSum = Convert.ToInt32(locBits, 2);
            for (int i = 0; i < clbEditLocations.Items.Count; i++)
            {
                int flag = (int)Math.Pow(2, i);
                clbEditLocations.SetItemChecked(i, (locSum & flag) != 0);
            }
        }

        private async void UpdateQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null || listView1.SelectedIndices.Count == 0) return;

            var q = database.questions[listView1.SelectedIndices[0]];

            if (pendingImagePaths.ContainsKey(5))
                (_, q.imageLink, _) = await ProcessContentAsync(txtEditQuestion.Text, pendingImagePaths[5]);
            else if (string.IsNullOrEmpty(label24.Text)) q.imageLink = "";

            q.question = txtEditQuestion.Text;
            q.locations = EncodeLocations(cmbEditModule.SelectedIndex, clbEditLocations);

            for (int i = 0; i < q.options.Count && i < 4; i++)
            {
                TextBox tb = i == 0 ? txtEditOption1 : i == 1 ? txtEditOption2 : i == 2 ? txtEditOption3 : txtEditOption4;
                Label lbl = i == 0 ? label25 : i == 1 ? label26 : i == 2 ? label27 : label28;

                q.options[i].text = tb.Text;
                if (pendingImagePaths.ContainsKey(i + 6))
                    (_, q.options[i].imageLink, q.options[i].useImage) = await ProcessContentAsync(tb.Text, pendingImagePaths[i + 6]);
                else if (string.IsNullOrEmpty(lbl.Text)) { q.options[i].imageLink = ""; q.options[i].useImage = false; }
            }

            await gitService.SaveDatabaseAsync(database); //
            for (int i = 5; i <= 9; i++) pendingImagePaths.Remove(i);
            RefreshQuestionList();
            MessageBox.Show("Updated!");
        }

        private async void DeleteQuestion_Click(object? sender, EventArgs e)
        {
            if (database == null || gitService == null || listView1.SelectedIndices.Count == 0) return;
            if (MessageBox.Show("Delete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                database.questions.RemoveAt(listView1.SelectedIndices[0]);
                await gitService.SaveDatabaseAsync(database); //
                RefreshQuestionList();
                gbEditQuestion.Visible = false;
            }
        }

        private void ClearInputFields()
        {
            textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear(); textBox5.Clear(); textBox7.Clear();
            foreach (var lbl in targetLabels!) lbl.Text = "";
            pendingImagePaths.Clear();
            for (int i = 0; i < checkedListBox1.Items.Count; i++) checkedListBox1.SetItemChecked(i, false);
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
    }
}