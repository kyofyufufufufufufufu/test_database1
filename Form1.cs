// Form1 logic for reading, writing and editing WinForms questions, answers and other details

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private QuestionSet database;
        private GitHubService gitService;

        public Form1()
        {
            InitializeComponent();

            // Configure ListView to behave like a list
            listView1.View = View.List;
            listView1.MultiSelect = false;

            database = new QuestionSet();

            button3.Click += CreateQuestion_Click;
            // The subscription to ListView1_SelectedIndexChanged is REMOVED to prevent 
            // the list view selection from updating the input fields on any tab.
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
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
                database = await gitService.GetDatabaseAsync();
                RefreshQuestionList();

                // Clear fields for new input after loading
                ClearInputFields();

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
            if (database != null && database.questions != null)
            {
                foreach (var q in database.questions)
                {
                    listView1.Items.Add(q.question);
                }
            }
        }

        private async void CreateQuestion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please fill in the question text.");
                return;
            }

            // Calculate Body Location for lower 8 digits when converting to Unity project's Database.cs
            int bodyPartSum = 0;
            var locationMap = new Dictionary<string, int>
            {
                { "Bladder", 1 }, { "Brain", 2 }, { "Eyes", 4 }, { "GI Tract", 8 },
                { "Heart", 16 }, { "Lungs", 32 }, { "Smooth Muscle", 64 }, { "Other", 128 }
            };

            foreach (var item in checkedListBox1.CheckedItems)
            {
                if (locationMap.ContainsKey(item.ToString()))
                {
                    bodyPartSum += locationMap[item.ToString()];
                }
            }

            // Encode Module for upper 5 digits when converting to Unity project's Database.cs
            int moduleIndex = 0;
            if (comboBox2.SelectedItem != null)
            {
                // Ensure a valid number is parsed
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

            // Used fixed-size array/string to represent the 5-bit module part
            // The value is encoded by setting the bit at the index corresponding to the module number
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
                question = textBox1.Text,
                imageLink = "",
                options = new List<Option>
                {
                    new Option { text = textBox2.Text, imageLink = "", useImage = false },
                    new Option { text = textBox4.Text, imageLink = "", useImage = false },
                    new Option { text = textBox3.Text, imageLink = "", useImage = false },
                    new Option { text = textBox5.Text, imageLink = "", useImage = false }
                },
                answerIndex = 0, // Assumines the correct answer is always option 1/textBox2
                difficulty = difficultyLevel,
                locations = finalLocationInt
            };

            database.questions.Add(newQ);

            // Upload
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

        // The ListView1_SelectedIndexChanged method was removed because its sole purpose
        // was to update the input fields (which you no longer want).

        private void ClearInputFields()
        {
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