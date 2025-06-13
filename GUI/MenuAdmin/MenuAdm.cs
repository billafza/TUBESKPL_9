using GUI.MenuAdmin.KelolaBuku;
using GUI.MenuAdmin.KelolaPengguna;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class MenuAdm : Form
    {
        public MenuAdm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult DR = MessageBox.Show("Apakah anda yakin untuk keluar?", "Peringatan", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (DR == DialogResult.OK)
            {
                this.Hide();
                Form1 login = new Form1();
                login.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            KelolaBuku kelola = new KelolaBuku();
            kelola.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KelolaPengguna kelola = new KelolaPengguna();
            kelola.Show();
        }
    }
}
