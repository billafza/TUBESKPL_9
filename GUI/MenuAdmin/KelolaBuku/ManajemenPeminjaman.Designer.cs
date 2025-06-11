namespace GUI.MenuAdmin.KelolaBuku
{
    partial class ManajemenPeminjaman
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            textBox1 = new TextBox();
            dataGridView1 = new DataGridView();
            button1 = new Button();
            button2 = new Button();
            label2 = new Label();
            textBox2 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(83, 25);
            label1.TabIndex = 0;
            label1.Text = "Nama    :";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(98, 6);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(150, 31);
            textBox1.TabIndex = 1;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 94);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(542, 344);
            dataGridView1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(442, 9);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 3;
            button1.Text = "Approve";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(442, 49);
            button2.Name = "button2";
            button2.Size = new Size(112, 34);
            button2.TabIndex = 4;
            button2.Text = "Reject";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Location = new Point(12, 49);
            label2.Name = "label2";
            label2.Size = new Size(83, 25);
            label2.TabIndex = 5;
            label2.Text = "ID Buku :";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(98, 49);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(150, 31);
            textBox2.TabIndex = 6;
            // 
            // ManajemenPeminjaman
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.bg;
            ClientSize = new Size(566, 450);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Name = "ManajemenPeminjaman";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ManajemenPeminjaman";
            Load += ManajemenPeminjaman_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private DataGridView dataGridView1;
        private Button button1;
        private Button button2;
        private Label label2;
        private TextBox textBox2;
    }
}