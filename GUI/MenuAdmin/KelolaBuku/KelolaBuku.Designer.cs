namespace GUI.MenuAdmin.KelolaBuku
{
    partial class KelolaBuku
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
            pictureBox1 = new PictureBox();
            dataGridView1 = new DataGridView();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            textBox1 = new TextBox();
            button5 = new Button();
            button6 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.Logo_BukuKitaAdmin_removebg_preview;
            pictureBox1.Location = new Point(345, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(178, 97);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 297);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(811, 270);
            dataGridView1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(12, 117);
            button1.Name = "button1";
            button1.Size = new Size(200, 47);
            button1.TabIndex = 2;
            button1.Text = "Tampilkan Buku";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(218, 117);
            button2.Name = "button2";
            button2.Size = new Size(200, 47);
            button2.TabIndex = 3;
            button2.Text = "Tambah Buku";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(424, 117);
            button3.Name = "button3";
            button3.Size = new Size(200, 47);
            button3.TabIndex = 4;
            button3.Text = "Hapus Buku";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(696, 117);
            button4.Name = "button4";
            button4.Size = new Size(127, 47);
            button4.TabIndex = 5;
            button4.Text = "Kembali";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 260);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(678, 31);
            textBox1.TabIndex = 6;
            // 
            // button5
            // 
            button5.Location = new Point(696, 260);
            button5.Name = "button5";
            button5.Size = new Size(127, 31);
            button5.TabIndex = 7;
            button5.Text = "Cari";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Location = new Point(12, 185);
            button6.Name = "button6";
            button6.Size = new Size(612, 47);
            button6.TabIndex = 8;
            button6.Text = "Manajemen Peminjaman Buku";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // KelolaBuku
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.bg;
            ClientSize = new Size(834, 584);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(textBox1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Controls.Add(pictureBox1);
            Name = "KelolaBuku";
            Text = "KelolaBuku";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private DataGridView dataGridView1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private TextBox textBox1;
        private Button button5;
        private Button button6;
    }
}