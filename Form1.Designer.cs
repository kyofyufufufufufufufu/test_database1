namespace WinFormsApp1
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
            tabPage2 = new TabPage();
            label13 = new Label();
            gbEditQuestion = new GroupBox();
            label16 = new Label();
            btnDeleteQuestion = new Button();
            btnUpdateQuestion = new Button();
            clbEditLocations = new CheckedListBox();
            cmbEditModule = new ComboBox();
            cmbEditDifficulty = new ComboBox();
            label14 = new Label();
            label15 = new Label();
            txtEditOption4 = new TextBox();
            txtEditOption3 = new TextBox();
            txtEditOption2 = new TextBox();
            txtEditOption1 = new TextBox();
            txtEditQuestion = new TextBox();
            lblEditOption4 = new Label();
            lblEditOption3 = new Label();
            lblEditOption2 = new Label();
            lblEditOption1 = new Label();
            lblEditQuestion = new Label();
            listView1 = new ListView();
            tabPage1 = new TabPage();
            checkedListBox1 = new CheckedListBox();
            comboBox2 = new ComboBox();
            comboBox1 = new ComboBox();
            label12 = new Label();
            label11 = new Label();
            textBox5 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            label10 = new Label();
            label9 = new Label();
            button8 = new Button();
            button9 = new Button();
            button10 = new Button();
            button11 = new Button();
            button6 = new Button();
            button7 = new Button();
            button4 = new Button();
            button5 = new Button();
            button3 = new Button();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            button2 = new Button();
            button1 = new Button();
            tabControl1 = new TabControl();
            label17 = new Label();
            button12 = new Button();
            button13 = new Button();
            label18 = new Label();
            button14 = new Button();
            textBox6 = new TextBox();
            tabPage2.SuspendLayout();
            gbEditQuestion.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(label13);
            tabPage2.Controls.Add(gbEditQuestion);
            tabPage2.Controls.Add(listView1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1051, 751);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "View/Edit/Delete Questions";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(48, 25);
            label13.Name = "label13";
            label13.Size = new Size(111, 15);
            label13.TabIndex = 1;
            label13.Text = "Database Questions";
            // 
            // gbEditQuestion
            // 
            gbEditQuestion.Controls.Add(label16);
            gbEditQuestion.Controls.Add(btnDeleteQuestion);
            gbEditQuestion.Controls.Add(btnUpdateQuestion);
            gbEditQuestion.Controls.Add(clbEditLocations);
            gbEditQuestion.Controls.Add(cmbEditModule);
            gbEditQuestion.Controls.Add(cmbEditDifficulty);
            gbEditQuestion.Controls.Add(label14);
            gbEditQuestion.Controls.Add(label15);
            gbEditQuestion.Controls.Add(txtEditOption4);
            gbEditQuestion.Controls.Add(txtEditOption3);
            gbEditQuestion.Controls.Add(txtEditOption2);
            gbEditQuestion.Controls.Add(txtEditOption1);
            gbEditQuestion.Controls.Add(txtEditQuestion);
            gbEditQuestion.Controls.Add(lblEditOption4);
            gbEditQuestion.Controls.Add(lblEditOption3);
            gbEditQuestion.Controls.Add(lblEditOption2);
            gbEditQuestion.Controls.Add(lblEditOption1);
            gbEditQuestion.Controls.Add(lblEditQuestion);
            gbEditQuestion.Location = new Point(487, 40);
            gbEditQuestion.Name = "gbEditQuestion";
            gbEditQuestion.Size = new Size(546, 649);
            gbEditQuestion.TabIndex = 3;
            gbEditQuestion.TabStop = false;
            gbEditQuestion.Text = "Edit Selected Question";
            gbEditQuestion.Visible = false;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(16, 274);
            label16.Name = "label16";
            label16.Size = new Size(64, 15);
            label16.TabIndex = 68;
            label16.Text = "Locations: ";
            // 
            // btnDeleteQuestion
            // 
            btnDeleteQuestion.BackColor = Color.WhiteSmoke;
            btnDeleteQuestion.Location = new Point(410, 593);
            btnDeleteQuestion.Name = "btnDeleteQuestion";
            btnDeleteQuestion.Size = new Size(106, 33);
            btnDeleteQuestion.TabIndex = 67;
            btnDeleteQuestion.Text = "Delete Question";
            btnDeleteQuestion.UseVisualStyleBackColor = false;
            // 
            // btnUpdateQuestion
            // 
            btnUpdateQuestion.BackColor = Color.WhiteSmoke;
            btnUpdateQuestion.Location = new Point(286, 593);
            btnUpdateQuestion.Name = "btnUpdateQuestion";
            btnUpdateQuestion.Size = new Size(106, 33);
            btnUpdateQuestion.TabIndex = 66;
            btnUpdateQuestion.Text = "Update Question";
            btnUpdateQuestion.UseVisualStyleBackColor = false;
            // 
            // clbEditLocations
            // 
            clbEditLocations.FormattingEnabled = true;
            clbEditLocations.Items.AddRange(new object[] { "Bladder", "Brain", "Eyes", "GI Tract", "Heart", "Lungs", "Smooth Muscle", "Other" });
            clbEditLocations.Location = new Point(111, 274);
            clbEditLocations.Name = "clbEditLocations";
            clbEditLocations.Size = new Size(125, 130);
            clbEditLocations.TabIndex = 65;
            // 
            // cmbEditModule
            // 
            cmbEditModule.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEditModule.FormattingEnabled = true;
            cmbEditModule.Items.AddRange(new object[] { "0", "1", "2", "3", "4" });
            cmbEditModule.Location = new Point(286, 236);
            cmbEditModule.Name = "cmbEditModule";
            cmbEditModule.Size = new Size(69, 23);
            cmbEditModule.TabIndex = 64;
            // 
            // cmbEditDifficulty
            // 
            cmbEditDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEditDifficulty.FormattingEnabled = true;
            cmbEditDifficulty.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            cmbEditDifficulty.Location = new Point(111, 236);
            cmbEditDifficulty.Name = "cmbEditDifficulty";
            cmbEditDifficulty.Size = new Size(69, 23);
            cmbEditDifficulty.TabIndex = 63;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(198, 239);
            label14.Name = "label14";
            label14.Size = new Size(82, 15);
            label14.TabIndex = 62;
            label14.Text = "Module (0-4): ";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(16, 239);
            label15.Name = "label15";
            label15.Size = new Size(89, 15);
            label15.TabIndex = 61;
            label15.Text = "Difficulty (1-5): ";
            // 
            // txtEditOption4
            // 
            txtEditOption4.Location = new Point(81, 189);
            txtEditOption4.Name = "txtEditOption4";
            txtEditOption4.Size = new Size(351, 23);
            txtEditOption4.TabIndex = 44;
            // 
            // txtEditOption3
            // 
            txtEditOption3.Location = new Point(81, 150);
            txtEditOption3.Name = "txtEditOption3";
            txtEditOption3.Size = new Size(351, 23);
            txtEditOption3.TabIndex = 43;
            // 
            // txtEditOption2
            // 
            txtEditOption2.Location = new Point(81, 111);
            txtEditOption2.Name = "txtEditOption2";
            txtEditOption2.Size = new Size(351, 23);
            txtEditOption2.TabIndex = 42;
            // 
            // txtEditOption1
            // 
            txtEditOption1.Location = new Point(81, 72);
            txtEditOption1.Name = "txtEditOption1";
            txtEditOption1.Size = new Size(351, 23);
            txtEditOption1.TabIndex = 41;
            // 
            // txtEditQuestion
            // 
            txtEditQuestion.Location = new Point(81, 33);
            txtEditQuestion.Name = "txtEditQuestion";
            txtEditQuestion.Size = new Size(351, 23);
            txtEditQuestion.TabIndex = 40;
            // 
            // lblEditOption4
            // 
            lblEditOption4.AutoSize = true;
            lblEditOption4.Location = new Point(16, 192);
            lblEditOption4.Name = "lblEditOption4";
            lblEditOption4.Size = new Size(59, 15);
            lblEditOption4.TabIndex = 39;
            lblEditOption4.Text = "Option 4: ";
            // 
            // lblEditOption3
            // 
            lblEditOption3.AutoSize = true;
            lblEditOption3.Location = new Point(16, 153);
            lblEditOption3.Name = "lblEditOption3";
            lblEditOption3.Size = new Size(59, 15);
            lblEditOption3.TabIndex = 38;
            lblEditOption3.Text = "Option 3: ";
            // 
            // lblEditOption2
            // 
            lblEditOption2.AutoSize = true;
            lblEditOption2.Location = new Point(16, 114);
            lblEditOption2.Name = "lblEditOption2";
            lblEditOption2.Size = new Size(59, 15);
            lblEditOption2.TabIndex = 37;
            lblEditOption2.Text = "Option 2: ";
            // 
            // lblEditOption1
            // 
            lblEditOption1.AutoSize = true;
            lblEditOption1.Location = new Point(16, 75);
            lblEditOption1.Name = "lblEditOption1";
            lblEditOption1.Size = new Size(59, 15);
            lblEditOption1.TabIndex = 36;
            lblEditOption1.Text = "Option 1: ";
            // 
            // lblEditQuestion
            // 
            lblEditQuestion.AutoSize = true;
            lblEditQuestion.Location = new Point(16, 36);
            lblEditQuestion.Name = "lblEditQuestion";
            lblEditQuestion.Size = new Size(61, 15);
            lblEditQuestion.TabIndex = 35;
            lblEditQuestion.Text = "Question: ";
            // 
            // listView1
            // 
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(17, 49);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new Size(450, 640);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(textBox6);
            tabPage1.Controls.Add(button14);
            tabPage1.Controls.Add(label18);
            tabPage1.Controls.Add(button13);
            tabPage1.Controls.Add(button12);
            tabPage1.Controls.Add(label17);
            tabPage1.Controls.Add(checkedListBox1);
            tabPage1.Controls.Add(comboBox2);
            tabPage1.Controls.Add(comboBox1);
            tabPage1.Controls.Add(label12);
            tabPage1.Controls.Add(label11);
            tabPage1.Controls.Add(textBox5);
            tabPage1.Controls.Add(textBox3);
            tabPage1.Controls.Add(textBox4);
            tabPage1.Controls.Add(textBox2);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Controls.Add(label10);
            tabPage1.Controls.Add(label9);
            tabPage1.Controls.Add(button8);
            tabPage1.Controls.Add(button9);
            tabPage1.Controls.Add(button10);
            tabPage1.Controls.Add(button11);
            tabPage1.Controls.Add(button6);
            tabPage1.Controls.Add(button7);
            tabPage1.Controls.Add(button4);
            tabPage1.Controls.Add(button5);
            tabPage1.Controls.Add(button3);
            tabPage1.Controls.Add(label8);
            tabPage1.Controls.Add(label7);
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(button1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1051, 751);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Create Question";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "Bladder", "Brain", "Eyes", "GI Tract", "Heart", "Lungs", "Smooth Muscle", "Other" });
            checkedListBox1.Location = new Point(148, 489);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(123, 148);
            checkedListBox1.TabIndex = 63;
            // 
            // comboBox2
            // 
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "0", "1", "2", "3", "4" });
            comboBox2.Location = new Point(339, 432);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(69, 23);
            comboBox2.TabIndex = 62;
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            comboBox1.Location = new Point(148, 432);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(69, 23);
            comboBox1.TabIndex = 61;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(40, 370);
            label12.Name = "label12";
            label12.Size = new Size(62, 15);
            label12.TabIndex = 60;
            label12.Text = "(Incorrect)";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(40, 312);
            label11.Name = "label11";
            label11.Size = new Size(62, 15);
            label11.TabIndex = 59;
            label11.Text = "(Incorrect)";
            // 
            // textBox5
            // 
            textBox5.Location = new Point(120, 362);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(448, 23);
            textBox5.TabIndex = 58;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(120, 304);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(448, 23);
            textBox3.TabIndex = 57;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(120, 240);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(448, 23);
            textBox4.TabIndex = 56;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(120, 182);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(448, 23);
            textBox2.TabIndex = 55;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(120, 127);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(448, 23);
            textBox1.TabIndex = 35;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(40, 248);
            label10.Name = "label10";
            label10.Size = new Size(62, 15);
            label10.TabIndex = 54;
            label10.Text = "(Incorrect)";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(40, 190);
            label9.Name = "label9";
            label9.Size = new Size(54, 15);
            label9.TabIndex = 53;
            label9.Text = "(Correct)";
            // 
            // button8
            // 
            button8.Location = new Point(698, 362);
            button8.Name = "button8";
            button8.Size = new Size(75, 23);
            button8.TabIndex = 52;
            button8.Text = "Clear";
            button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            button9.Location = new Point(586, 362);
            button9.Name = "button9";
            button9.Size = new Size(106, 23);
            button9.TabIndex = 51;
            button9.Text = "Select Image";
            button9.UseVisualStyleBackColor = true;
            button9.Click += ImageButton_Click;
            // 
            // button10
            // 
            button10.Location = new Point(698, 304);
            button10.Name = "button10";
            button10.Size = new Size(75, 23);
            button10.TabIndex = 50;
            button10.Text = "Clear";
            button10.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            button11.Location = new Point(586, 304);
            button11.Name = "button11";
            button11.Size = new Size(106, 23);
            button11.TabIndex = 49;
            button11.Text = "Select Image";
            button11.UseVisualStyleBackColor = true;
            button11.Click += ImageButton_Click;
            // 
            // button6
            // 
            button6.Location = new Point(698, 240);
            button6.Name = "button6";
            button6.Size = new Size(75, 23);
            button6.TabIndex = 48;
            button6.Text = "Clear";
            button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.Location = new Point(586, 240);
            button7.Name = "button7";
            button7.Size = new Size(106, 23);
            button7.TabIndex = 47;
            button7.Text = "Select Image";
            button7.UseVisualStyleBackColor = true;
            button7.Click += ImageButton_Click;
            // 
            // button4
            // 
            button4.Location = new Point(698, 182);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 46;
            button4.Text = "Clear";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(586, 182);
            button5.Name = "button5";
            button5.Size = new Size(106, 23);
            button5.TabIndex = 45;
            button5.Text = "Select Image";
            button5.UseVisualStyleBackColor = true;
            button5.Click += ImageButton_Click;
            // 
            // button3
            // 
            button3.Location = new Point(42, 665);
            button3.Name = "button3";
            button3.Size = new Size(119, 23);
            button3.TabIndex = 44;
            button3.Text = "Create Question";
            button3.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(40, 493);
            label8.Name = "label8";
            label8.Size = new Size(64, 15);
            label8.TabIndex = 43;
            label8.Text = "Locations: ";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(251, 434);
            label7.Name = "label7";
            label7.Size = new Size(82, 15);
            label7.TabIndex = 42;
            label7.Text = "Module (0-4): ";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(40, 434);
            label6.Name = "label6";
            label6.Size = new Size(89, 15);
            label6.TabIndex = 41;
            label6.Text = "Difficulty (1-5): ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(40, 355);
            label5.Name = "label5";
            label5.Size = new Size(59, 15);
            label5.TabIndex = 40;
            label5.Text = "Option 4: ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(42, 297);
            label4.Name = "label4";
            label4.Size = new Size(59, 15);
            label4.TabIndex = 39;
            label4.Text = "Option 3: ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(40, 233);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 38;
            label3.Text = "Option2: ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(40, 175);
            label2.Name = "label2";
            label2.Size = new Size(59, 15);
            label2.TabIndex = 37;
            label2.Text = "Option 1: ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(40, 130);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 36;
            label1.Text = "Question: ";
            // 
            // button2
            // 
            button2.Location = new Point(698, 127);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 34;
            button2.Text = "Clear";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(586, 127);
            button1.Name = "button1";
            button1.Size = new Size(106, 23);
            button1.TabIndex = 33;
            button1.Text = "Select Image";
            button1.UseVisualStyleBackColor = true;
            button1.Click += ImageButton_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1059, 779);
            tabControl1.TabIndex = 33;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(42, 14);
            label17.Name = "label17";
            label17.Size = new Size(74, 15);
            label17.TabIndex = 64;
            label17.Text = "Bulk Upload:";
            label17.Click += label17_Click;
            // 
            // button12
            // 
            button12.Location = new Point(457, 39);
            button12.Name = "button12";
            button12.Size = new Size(97, 23);
            button12.TabIndex = 65;
            button12.Text = "Select File";
            button12.UseVisualStyleBackColor = true;
            // 
            // button13
            // 
            button13.Location = new Point(563, 39);
            button13.Name = "button13";
            button13.Size = new Size(97, 23);
            button13.TabIndex = 66;
            button13.Text = "Clear";
            button13.UseVisualStyleBackColor = true;
            button13.Click += button13_Click;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label18.Location = new Point(42, 67);
            label18.Name = "label18";
            label18.Size = new Size(131, 15);
            label18.TabIndex = 67;
            label18.Text = "(Accepted formats: .csv)";
            label18.Click += label18_Click;
            // 
            // button14
            // 
            button14.Location = new Point(170, 665);
            button14.Name = "button14";
            button14.Size = new Size(128, 23);
            button14.TabIndex = 68;
            button14.Text = "Duplicate Question";
            button14.UseVisualStyleBackColor = true;
            // 
            // textBox6
            // 
            textBox6.Location = new Point(42, 39);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(409, 23);
            textBox6.TabIndex = 69;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1083, 812);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "PharmD Database Interface";
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            gbEditQuestion.ResumeLayout(false);
            gbEditQuestion.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabPage tabPage2;
        private TabPage tabPage1;
        private CheckedListBox checkedListBox1;
        private ComboBox comboBox2;
        private ComboBox comboBox1;
        private Label label12;
        private Label label11;
        private TextBox textBox5;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox textBox2;
        private TextBox textBox1;
        private Label label10;
        private Label label9;
        private Button button8;
        private Button button9;
        private Button button10;
        private Button button11;
        private Button button6;
        private Button button7;
        private Button button4;
        private Button button5;
        private Button button3;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Button button2;
        private Button button1;
        private TabControl tabControl1;
        private Label label13;
        private ListView listView1;
        private GroupBox gbEditQuestion;
        private TextBox txtEditOption4;
        private TextBox txtEditOption3;
        private TextBox txtEditOption2;
        private TextBox txtEditOption1;
        private TextBox txtEditQuestion;
        private Label lblEditOption4;
        private Label lblEditOption3;
        private Label lblEditOption2;
        private Label lblEditOption1;
        private Label lblEditQuestion;
        private CheckedListBox clbEditLocations;
        private ComboBox cmbEditModule;
        private ComboBox cmbEditDifficulty;
        private Label label14;
        private Label label15;
        private Button btnDeleteQuestion;
        private Button btnUpdateQuestion;
        private Label label16;
        private Label label17;
        private Button button13;
        private Button button12;
        private Label label18;
        private Button button14;
        private TextBox textBox6;
    }
}