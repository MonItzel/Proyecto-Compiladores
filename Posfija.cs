using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    internal class Posfija
    {
        //variables
        private int contador =0;
        public int TokensAux = 0;

        //listas
        public List<string> posfija = new List<string>();
        public List<string> prefija = new List<string>();
        public List<string> cola = new List<string>();
        public List<string> posfijaAux = new List<string>();
        public List<string> prefijaAux = new List<string>();
        public List<string> colaAux = new List<string>();

        //constructures
        public Posfija() { }

        //Funciones
        public string AfterConversion(string exp)
        {
            if (!CheckExpression(exp))
                return "";
            Stack<char> stack = new Stack<char>();
            exp = exp.Replace(" ", String.Empty);
            char[] charArray = exp.ToCharArray();
            int i = 0;

            if (charArray[i] != '[')
            {
                stack.Push(charArray[i]);
                i++;
                contador++;
            }
            while (i < charArray.Length)
            {
                if (charArray[i] == '[')
                {
                    if (stack.Count() > 0 && stack.Peek() != '|' && stack.Peek() != '(')
                        stack.Push('&');
                    i = ChangeCharacterInterval(stack, charArray, i);
                }
                else if (IsConcatenation(charArray[i]) && stack.Peek() != '(' && stack.Peek() != '|')
                {
                    TokensAux = stack.Count;
                    stack.Push('&');
                    stack.Push(charArray[i]);
                }
                else
                {
                    stack.Push(charArray[i]);
                    if (charArray[i].Equals('|') && (!charArray[i + 1].Equals('[') && !charArray[i + 1].Equals('(')))
                    {
                        stack.Push(charArray[++i]);
                    }
                }
                i++;
            }
            return ChangeExpression(stack);
        }
        public string Convierte_Posfija(string str)
        {
            //Variable para almacenar la cadena despues de convertir
            str = AfterConversion(str);
            //Pila de insersión
            Stack<char> stk = new Stack<char>();
            int i = 0; int k = 0;
            char[] r = new char[str.Length];
            char[] arr = str.ToArray<char>();

            string s;
            while (i < arr.Length)
            {
                s = arr[i].ToString();
                switch (s)
                {
                    //Insertar en la pila
                    case "(":
                        stk.Push(arr[i]);
                        break;
                    //Extraer de la pila y desplegar en posfija hasta encontrar “paréntesis izquierdo” (no desplegarlo)
                    case ")":
                        while (stk.Peek() != '(')
                        {
                            r[k++] = stk.Pop();
                        }
                        stk.Pop();
                        break;
                    //Desplegar en posfija.
                    case var c when new Regex(@"^[a-z]+$").IsMatch(c):
                    case var c1 when new Regex(@"^[A-Z]+$").IsMatch(c1):
                    case var c2 when new Regex(@"^[0-9]+$").IsMatch(c2):
                        r[k++] = arr[i];
                        break;
                    case "*":
                    case "+":
                    case "?":
                    case "|":
                    case "&":
                        bool b = true;
                        while (b)
                        {
                            if (stk.Count == 0 || stk.Peek().Equals('(') || p(arr[i], stk.Peek()))
                            {
                                stk.Push(arr[i]);
                                b = false;
                            }
                            else
                            {
                                r[k++] = stk.Pop();
                            }
                        }
                        break;
                }
                i++;
            }
            while (stk.Count > 0)
            {
                if (stk.Peek() != '(')
                    r[k++] = stk.Pop();
                else
                    stk.Pop();
            }
            return new String(r);
        }
        //stk una pila de caracteres (despues los convertimos a string)
        private string ChangeExpression(Stack<char> stk)
        {
            char[] obj = stk.ToArray();
            IQueryable<char> reversed = obj.AsQueryable().Reverse();
            return new String(reversed.ToArray());
        }

        private int ChangeCharacterInterval(Stack<char> stk, char[] charArray, int i)
        {
            int j = i;
            stk.Push('(');
            j++;

            Stack<char> stkAux = new Stack<char>();
            while (charArray[j] != ']')
            {
                if (charArray[j] != '-')
                {
                    stkAux.Push(charArray[j]);
                }
                else
                {
                    j++;
                    ChangeInterval(stkAux.Pop(), charArray[j], stkAux);
                }
                j++;
            }
            IQueryable<char> reversed = stkAux.AsQueryable().Reverse();
            char[] arr = reversed.ToArray<char>();
            int it;
            for (it = 0; it < arr.Length - 1; it++)
            {
                stk.Push(arr[it]);
                stk.Push('|');
            }
            stk.Push(arr[it]);
            stk.Push(')');

            return j;
        }

        private void ChangeInterval(char a, char b, Stack<char> stk)
        {
            for (char c = a; c <= b; c++)
            {
                stk.Push(c);
            }
        }

        private bool IsConcatenation(char chacracter)
        {
            string character = chacracter.ToString();
            switch (character)
            {
                case var c when new Regex(@"^[a-z]+$").IsMatch(c):
                case var c1 when new Regex(@"^[A-Z]+$").IsMatch(c1):
                case var c2 when new Regex(@"^[0-9]+$").IsMatch(c2):
                case "(":
                case "[":
                    return true;
                    // break;
            }
            return false;
        }

       
        private bool CheckExpression(string exp)
        {
            exp = exp.Replace(" ", String.Empty);
            string c = exp.Substring(0, 1);
            string c1 = exp.Substring(exp.Length - 1, 1);
            if (
                c != "*" && c != "+" && c != "?" && c != "|"
                && c1 != "|"
                && CheckCorrespondence(exp, '(', ')')
                && Empty(exp, '(', ')')
                && CheckCorrespondence(exp, '[', ']')
                && Empty(exp, '[', ']')
                )
            {
                return true;
            }


            return false;
        }

        //Revisa si existe una correspondencia () [] {}
        public bool CheckCorrespondence(string expRegular, char leftKey, char rightKey)
        {
            int g = 0;
            foreach (char c in expRegular)
            {
                g += c.Equals(leftKey) ? 1 : c.Equals(rightKey) ? -1 : 0;
                if (g < 0)
                    break;
            }
            return g == 0;
        }

        //Revisa si no existen parentesis vacios
        private bool Empty(string exp, char left, char right)
        {
            char var = ' ';
            foreach (char s in exp)
            {
                if (var.Equals(left) && s.Equals(right))
                    return false;
                var = s;

            }
            return true;
        }

        //Banderas
        private bool p(char a, char b)
        {
            return pp(a) > pp(b);
        }


        //Auxiliar switch para la bandera
        private int pp(char c)
        {
            switch (c)
            {
                case '*':
                case '+':
                case '?':
                    return 2;
                case '|':
                    return 0;
                case '&':
                    return 1;
            }
            return -1;
        }
    }
}
