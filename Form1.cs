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
            txtResultado.Text = resultado ? "Cadena válida según la gramática." : "Cadena inválida.";
        }
    }

    public class Parser
    {
        private List<string> tokens;
        private int pos = 0;

        public bool Analizar(string entrada)
        {
            tokens = Tokenizar(entrada);
            if (tokens == null) return false;

            pos = 0;
            return S() && pos == tokens.Count;
        }

        private List<string> Tokenizar(string input)
        {
            var resultado = new List<string>();
            var patron = new Regex(@"\s*(var|[0-9]+|[=+\-*/()])\s*");
            var matches = patron.Matches(input);
            int total = 0;

            foreach (Match m in matches)
            {
                resultado.Add(m.Groups[1].Value);
                total += m.Length;
            }

            return total == input.Length ? resultado : null;
        }

        private string Actual => pos < tokens.Count ? tokens[pos] : null;

        private bool Coincidir(string esperado)
        {
            if (Actual == esperado)
            {
                pos++;
                return true;
            }
            return false;
        }

        private bool S()
        {
            return Coincidir("var") && Coincidir("=") && E();
        }

        private bool E()
        {
            if (!T()) return false;
            while (Actual == "+" || Actual == "-")
            {
                pos++;
                if (!T()) return false;
            }
            return true;
        }

        private bool T()
        {
            if (!F()) return false;
            while (Actual == "*" || Actual == "/")
            {
                pos++;
                if (!F()) return false;
            }
            return true;
        }

        private bool F()
        {
            if (Actual == "(")
            {
                pos++;
                if (!E()) return false;
                return Coincidir(")");
            }
            else if (int.TryParse(Actual, out _))
            {
                pos++;
                return true;
            }
            return false;
        }
    }
}
