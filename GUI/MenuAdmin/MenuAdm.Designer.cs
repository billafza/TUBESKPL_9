﻿namespace GUI
{
    partial class MenuAdm
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
            button1 = new Button();
            pictureBox1 = new PictureBox();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(315, 197);
            label1.Name = "label1";
            label1.Size = new Size(0, 25);
            label1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(12, 112);
            button1.Name = "button1";
            button1.Size = new Size(776, 57);
            button1.TabIndex = 1;
            button1.Text = "Kelola Pengguna";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.Logo_BukuKitaAdmin_removebg_preview;
            pictureBox1.Location = new Point(315, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(158, 94);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // button2
            // 
            button2.Location = new Point(12, 175);
            button2.Name = "button2";
            button2.Size = new Size(776, 57);
            button2.TabIndex = 3;
            button2.Text = "Kelola Buku";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(12, 381);
            button3.Name = "button3";
            button3.Size = new Size(776, 57);
            button3.TabIndex = 4;
            button3.Text = "Keluar";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.LightBlue;
            button4.Location = new Point(12, 238);
            button4.Name = "button4";
            button4.Size = new Size(776, 57);
            button4.TabIndex = 5;
            button4.Text = "Kelola Approval";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // MenuAdm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.bg;
            ClientSize = new Size(800, 450);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(pictureBox1);
            Controls.Add(button1);
            Controls.Add(label1);
            Name = "MenuAdm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MenuAdm";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button1;
        private PictureBox pictureBox1;
        private Button button2;
        private Button button3;
        private Button button4;
    }
}