using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace Proyecto_Compiladores
{
    public partial class Form2 : Form
    {
        public string Errores { get; set; }

        public string TreeV { get; set; }
        public Form2()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            txtErrores.Text = Errores;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
               
        }
    }
}
