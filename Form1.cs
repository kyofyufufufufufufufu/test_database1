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
        private QuestionSet? database;
        private GitHubService? gitService;

        // Note: For clarity, the edit controls are usually declared in Form1.Designer.cs.
        // Assuming their existence for the logic below:
        // private GroupBox gbEditQuestion;
        // private TextBox txtEditQuestion;
        // private ComboBox cmbEditDifficulty;
        // private ComboBox cmbEditModule;
        // private CheckedListBox clbEditLocations;
        // private TextBox txtEditOption1;
        // private TextBox txtEditOption2;
        // private TextBox txtEditOption3;
        // private TextBox txtEditOption4;

        public Form1()
        {
            InitializeComponent();

            // Configure ListView to behave like a list
            listView1.View = View.List;
            listView1.MultiSelect = false;

            // Initialized to a non-null object to satisfy the constructor warning
            database = new QuestionSet();

            button3.Click += CreateQuestion_Click;
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            this.Load += Form1_Load;
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
                database = await gitService.GetDatabaseAsync();
                RefreshQuestionList();

                // Clear fields for new input after loading
                ClearInputFields();

                // Ensure the edit panel is hidden initially
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
            // Used null-conditional access
            if (database?.questions != null)
            {
                foreach (var q in database.questions)
                {
                    listView1.Items.Add(q.question);
                }
            }
        }

        private async void CreateQuestion_Click(object? sender, EventArgs e)
        {
            // Explicit null checks before calling members
            if (database == null || gitService == null)
            {
                 MessageBox.Show("Database service is not loaded. Cannot create question.");
                 return;
            }

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

            // Safely iterate and check for null key
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
                // Use null-forgiving operator (!) as we confirmed gitService is non-null above
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

        // CORRECTED: Now toggles gbEditQuestion visibility and populates the dedicated edit fields.
        private void ListView1_SelectedIndexChanged(object? sender, EventArgs e) 
        {
            if (database == null) return;

            // 1. If no item is selected (e.g., list is deselected)
            if (listView1.SelectedIndices.Count == 0)
            {
                ClearInputFields();
                // FIX: Hide the edit box when nothing is selected
                if (gbEditQuestion != null) gbEditQuestion.Visible = false;
                return; 
            }

            // 2. An item is selected: Show the edit box
            if (gbEditQuestion != null) gbEditQuestion.Visible = true;
            
            int index = listView1.SelectedIndices[0];

            // Use null-forgiving operator as database is checked and questions is initialized
            if (index < 0 || database.questions!.Count <= index) return;

            var q = database.questions![index];

            // 3. Populate the DEDICATED EDIT controls, NOT the CREATE controls.
            txtEditQuestion.Text = q.question;
            if (q.options.Count >= 4)
            {
                txtEditOption1.Text = q.options[0].text;
                txtEditOption2.Text = q.options[1].text;
                txtEditOption3.Text = q.options[2].text;
                txtEditOption4.Text = q.options[3].text;
            }

            // Use null-forgiving operator (!)
            cmbEditDifficulty.SelectedItem = q.difficulty.ToString()!; 

            // Decodes Locations for UI
            // Bit Unpacking Logic
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
            // Use null-forgiving operator (!)
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
                string? name = clbEditLocations.Items[i]?.ToString(); // Safe conversion to string
                if (name != null && locationMap.ContainsKey(name))
                {
                    int val = locationMap[name];
                    bool isChecked = (locationVal & val) == val;
                    clbEditLocations.SetItemChecked(i, isChecked);
                }
            }
        }

        // UPDATED: Now clears both Create and Edit fields for a clean slate.
        private void ClearInputFields()
        {
            // Clear 'Create Question' fields (tabPage1 controls)
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

            // Clear 'Edit Question' fields
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