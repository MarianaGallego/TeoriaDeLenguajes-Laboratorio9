using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace teoriaDeLenguajesLaboratorio9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            string entrada = txtEntrada.Text.Trim();
            var parser = new Parser();

            bool resultado = parser.Analizar(entrada);
            txtResultado.Text = resultado ? "Cadena VÁLIDA." : "Cadena inválida.";
        }
    }
}
