using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace teoriaDeLenguajesLaboratorio9
{
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
