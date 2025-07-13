using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teoriaDeLenguajesLaboratorio9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnAnalizar_Click(object sender, EventArgs e)
        {
            string input = txtEntrada.Text.Trim();
            txtResultado.Text = "Analizando...";
            btnAnalizar.Enabled = false;

            var resultado = await Task.Run(() =>
            {
                List<string> tokens = Tokenizar(input);
                if (tokens == null)
                    return "Error léxico: entrada inválida.";

                bool esValida = AnalisisAscendente(tokens);
                return esValida ? "Cadena válida según la gramática." : "Cadena inválida.";
            });

            txtResultado.Text = resultado;
            btnAnalizar.Enabled = true;
        }

        private async void label1_Click(object sender, EventArgs e)
        {
            string input = txtEntrada.Text.Trim();
            txtResultado.Text = "Analizando...";
            btnAnalizar.Enabled = false;

            var resultado = await Task.Run(() =>
            {
                List<string> tokens = Tokenizar(input);
                if (tokens == null)
                    return "Error léxico: entrada inválida.";

                bool esValida = AnalisisAscendente(tokens);
                return esValida ? "Cadena válida según la gramática." : "Cadena inválida.";
            });

            txtResultado.Text = resultado;
            btnAnalizar.Enabled = true;
        }

        private List<string> Tokenizar(string input)
        {
            List<string> tokens = new List<string>();
            string patron = @"\s*(var|[0-9]+|[=+\-*/()])\s*";
            MatchCollection matches = Regex.Matches(input, patron);

            int longitudTokens = 0;
            foreach (Match match in matches)
            {
                string token = match.Groups[1].Value;
                tokens.Add(token);
                longitudTokens += match.Length;
            }

            if (longitudTokens != input.Length)
                return null;

            return tokens;
        }

        private bool AnalisisAscendente(List<string> tokens)
        {
            Stack<string> pila = new Stack<string>();
            int i = 0;

            while (i <= tokens.Count)
            {
                string vista = i < tokens.Count ? tokens[i] : "$";

                // Shift
                if (vista != "$")
                {
                    pila.Push(vista);
                    i++;
                }

                // Intentamos reducir
                bool seRedujo = true;
                int reducciones = 0;

                while (seRedujo && reducciones < 1000)
                {
                    seRedujo = false;
                    reducciones++;

                    // F -> num
                    if (MatchTop(pila, "num"))
                    {
                        ReplaceTop(pila, 1, "F");
                        seRedujo = true;
                    }
                    // F -> ( E )
                    else if (MatchTop(pila, ")") && MatchSecond(pila, "E") && MatchThird(pila, "("))
                    {
                        ReplaceTop(pila, 3, "F");
                        seRedujo = true;
                    }
                    // T -> T * F
                    else if (MatchTop(pila, "F") && MatchSecond(pila, "*") && MatchThird(pila, "T"))
                    {
                        ReplaceTop(pila, 3, "T");
                        seRedujo = true;
                    }
                    // T -> T / F
                    else if (MatchTop(pila, "F") && MatchSecond(pila, "/") && MatchThird(pila, "T"))
                    {
                        ReplaceTop(pila, 3, "T");
                        seRedujo = true;
                    }
                    // T -> F
                    else if (MatchTop(pila, "F"))
                    {
                        ReplaceTop(pila, 1, "T");
                        seRedujo = true;
                    }
                    // E -> E + T
                    else if (MatchTop(pila, "T") && MatchSecond(pila, "+") && MatchThird(pila, "E"))
                    {
                        ReplaceTop(pila, 3, "E");
                        seRedujo = true;
                    }
                    // E -> E - T
                    else if (MatchTop(pila, "T") && MatchSecond(pila, "-") && MatchThird(pila, "E"))
                    {
                        ReplaceTop(pila, 3, "E");
                        seRedujo = true;
                    }
                    // E -> T
                    else if (MatchTop(pila, "T"))
                    {
                        ReplaceTop(pila, 1, "E");
                        seRedujo = true;
                    }
                    // S -> var = E
                    else if (MatchTop(pila, "E") && MatchSecond(pila, "=") && MatchThird(pila, "var"))
                    {
                        ReplaceTop(pila, 3, "S");
                        seRedujo = true;
                    }
                }

                if (reducciones >= 1000)
                    return false;

                if (pila.Count == 1 && pila.Peek() == "S" && i == tokens.Count)
                    return true;

                if (i >= tokens.Count && pila.Count > 1)
                    return false;
            }

            return false;
        }

        // Métodos auxiliares

        private string GetFromTop(Stack<string> pila, int pos)
        {
            if (pila.Count <= pos) return null;
            return pila.ToArray()[pos];
        }

        private bool MatchTop(Stack<string> pila, string esperado)
        {
            string real = GetFromTop(pila, 0);
            return esperado == "num" ? int.TryParse(real, out _) : real == esperado;
        }

        private bool MatchSecond(Stack<string> pila, string esperado)
        {
            string real = GetFromTop(pila, 1);
            return esperado == "num" ? int.TryParse(real, out _) : real == esperado;
        }

        private bool MatchThird(Stack<string> pila, string esperado)
        {
            string real = GetFromTop(pila, 2);
            return esperado == "num" ? int.TryParse(real, out _) : real == esperado;
        }

        private void ReplaceTop(Stack<string> pila, int n, string nuevo)
        {
            for (int j = 0; j < n; j++) pila.Pop();
            pila.Push(nuevo);
        }
    }
}
