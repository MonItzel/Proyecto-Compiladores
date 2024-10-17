using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    class Expresion_Regular
    {
        public Dictionary<char, int> operadores;

        public Expresion_Regular()
        {
            operadores = new Dictionary<char, int>();
            operadores.Add('|', 1);
            operadores.Add('&', 2);
            operadores.Add('*', 3);
            operadores.Add('+', 3);
            operadores.Add('?', 3);

        }



        /*
        *Algoritmo que convierte de la notación infija a postfija
        *@Param: El agoritmo recibe una lista de char donde se encuentra la expresión en notación
        *        infija. El algoritmo asume que las concatenaciones vienen de forma explicita con
        *        el caracter '&'
        *@Return: regresaa una lista de char con la notación en postfija       
        */
        public List<char> Postfija(List<char> infija)
        {
            List<char> post = new List<char>();
            int cont = 0;
            Stack<char> pila = new Stack<char>();
            bool band;
            while (cont < infija.Count)
            {
                switch (infija[cont])
                {
                    case '(':
                        pila.Push(infija[cont]);
                        break;
                    case ')':
                        char valor;
                        do
                        {
                            valor = pila.Pop();
                            if (valor != '(')
                                post.Add(valor);

                        } while (valor != '(');
                        break;
                    default:
                        if (Es_operador(infija[cont]))
                        {
                            band = true;
                            while (band)
                            {
                                if (pila.Count == 0 || pila.Peek() == '('
                                    || operadores[infija[cont]] > operadores[pila.Peek()])
                                {


                                    pila.Push(infija[cont]);
                                    band = false;
                                }
                                else
                                {
                                    post.Add(pila.Pop());
                                }
                            }
                        }
                        else
                        {
                            post.Add(infija[cont]);
                        }
                        break;

                }
                cont++;
            }
            while (pila.Count != 0)
            {
                post.Add(pila.Pop());
            }
            return post;
        }

        /*
        *Método que realiza la concatenación de la expresión regular
        *@Param: Recibe Una lista de char con la expresión
        *@Return: regresa la expresión con las concatenaciones Explícitas
        * */
        public List<char> Concatena(List<char> expresion)
        {
            List<char> aux = new List<char>();
            for (int i = 0; i < expresion.Count - 1; i++)
            {
                aux.Add(expresion[i]);
                if ((expresion[i] == '*' || expresion[i] == '+' || expresion[i] == '?') && ((expresion[i + 1] == '(')
                      || (!(Es_operador(expresion[i + 1]))) && (expresion[i + 1] != ')')))
                {

                    aux.Add('&');
                }
                if (!(Es_operador(expresion[i])) && !(Es_operador(expresion[i + 1])))
                {
                    if ((expresion[i]) != '(' && (expresion[i]) != ')' && (expresion[i + 1]) != '(' && (expresion[i + 1]) != ')')
                        aux.Add('&');

                }
                if (!(Es_operador(expresion[i])) && !(Es_operador(expresion[i + 1])))
                {
                    if ((expresion[i]) != '(' && (expresion[i]) != ')' && expresion[i + 1] == '(')
                    {
                        aux.Add('&');

                    }
                }
                if (!(Es_operador(expresion[i])) && !(Es_operador(expresion[i + 1])))
                {
                    if ((expresion[i]) == ')' && (expresion[i + 1] == '(' || expresion[i + 1] != ')'))
                    {
                        aux.Add('&');
                    }
                }


            }
            aux.Add(expresion[expresion.Count - 1]);
            return aux;
        }


        /*
        *Funcion auxiliar que determina si un caracter es operador o no*
        */
        public bool Es_operador(char op)
        {
            if (op == '|' || op == '&' || op == '*' || op == '+' || op == '?')
            {
                return true;
            }
            return false;
        }


        /*
        *Método que realiza la concatenación de la expresión regular
        *@Param: Recibe Una lista de char con la expresión
        *@Return: regresa la expresión con las concatenaciones Explícitas
        * */
        public List<char> Recibo(string Expresion)
        {

            // Creando una matriz de longitud de cadena             
            char[] ch = new char[Expresion.Length];


            //matriz que recibe el resultado del metodo 1
            string[] StrRes;
            //lista donde se sustituye [] por el metodo 1
            List<char> CadenaFinal = new List<char>();
            int llenanueva = 0, guion = 0, iantigua = 0;



            // Copie carácter por carácter en la matriz
            for (int i = 0; i < Expresion.Length; i++)
            {
                ch[i] = Expresion[i];
            }

            //recorre la cadena buscando los corchetes
            for (int i = 0; i < ch.Length; i++)
            {

                if (ch[i] == '[') //encuentra corchete
                {
                    iantigua = i;
                    while (ch[i] != ']')
                    {
                        if (ch[i] == '-')
                        {
                            guion = 1;
                        }
                        llenanueva++;
                        i++;
                    }

                    //matriz para crear la cadena que se manda al metodo 1
                    char[] ch2 = new char[llenanueva];
                    for (int k = 0; k < llenanueva; k++) //crea la nueva cadena para mandar al metodo
                    {
                        ch2[k] = ch[iantigua];
                        iantigua++;
                    }
                    if (guion == 1)
                    {
                        StrRes = Metodo1(ch2);//regresa la cadena resultante
                    }
                    else
                    {
                        StrRes = Metodo3(ch2);//regresa la cadena resultante
                    }


                    //sustituye los corchetes por el resultado del metodo 1
                    /** --------------Método 2 -------------*/
                    /*
                    * Este método con ayuda del método anterior debe de sustituir
                    * en la expresión regular la secuencia de caracteres.
                    * Por ejemplo, se recibe la siguiente expresión --- " (a|b)?[1-4]* "
                    * la función deberia regresar " (a|b)?(1|2|3|4)* "
                    */
                    int cont2 = 0;
                    for (int m = 0; m < ch.Length; m++)
                    {
                        if (ch[m] == '[')
                        {
                            for (int l = 0; l < StrRes.Length; l++)
                            {
                                CadenaFinal.Add(char.Parse(StrRes[cont2]));
                                cont2++;
                            }
                            m = ch.Length;
                        }
                    }
                    guion = 0;
                    llenanueva = 0;
                }
                else //inserta el elemento en la cadena ya que no es necesario realizar el metodo
                {
                    CadenaFinal.Add(ch[i]);
                }
            }
            return CadenaFinal;
        }

        /** -------------- Método 1 -------------**/
        /*Escribir método para crear una secuencia de caracteres
        * tanto números, letras en mayúscula y minúscula*
        * Por ejemplo, la función recibe el seguiente parametro
        * "[A-D]" y la funcion regresaría un string con la siguiente cadena
        * "(A|B|C|D)"
        */
        public string[] Metodo1(char[] cadena)
        {
            int ban1, ban2;
            ban1 = ConvierteLetra(cadena[1].ToString());
            if (ban1 != -1) //el rango es de letras
            {
                ban2 = ConvierteLetra(cadena[3].ToString());
                string[] rango = new string[(ban2 - ban1 + 1) + (ban2 - ban1) + 2];
                int cont = 0;
                rango[0] = "(";
                for (int i = 1; i <= rango.Length - 1; i = i + 2)
                {
                    rango[i] = ConvierteNumero(ban1 + cont);
                    cont++;
                    if (i == rango.Length - 1)
                    {
                    }
                    else
                    {
                        rango[i + 1] = "|";
                    }
                }
                rango[rango.Length - 1] = ")";

                return rango;
            }
            else //el rango es de numeros
            {
                ban1 = cadena[1] - 48;
                ban2 = cadena[3] - 48;
                string[] rango = new string[(ban2 - ban1 + 1) + (ban2 - ban1) + 2];
                int cont = 0;
                rango[0] = "(";
                for (int i = 1; i <= rango.Length - 1; i = i + 2)
                {
                    rango[i] = ConvierteNumAStr(ban1 + cont);
                    cont++;
                    if (i == rango.Length - 1)
                    {
                    }
                    else
                    {
                        rango[i + 1] = "|";
                    }
                }
                rango[rango.Length - 1] = ")";

                return rango;
            }
        }

        public string[] Metodo3(char[] cadena)
        {

            int ban1, ban2, carac;
            carac = cadena.Length - 2;
            ban1 = ConvierteLetra(cadena[1].ToString());
            if (ban1 != -1) //el rango es de letras
            {
                string[] rango = new string[carac + 2 + carac + 1];
                int cont = 1;
                rango[0] = "(";
                for (int i = 1; i <= rango.Length - 1; i = i + 2)
                {
                    ban2 = ConvierteLetra(cadena[cont].ToString());
                    rango[i] = ConvierteNumero(ban2);
                    cont++;
                    if (i == rango.Length - 1)
                    {
                    }
                    else
                    {
                        rango[i + 1] = "|";
                    }
                }
                rango[rango.Length - 1] = ")";

                return rango;
            }
            else //el rango es de numeros
            {
                string[] rango = new string[carac + 2 + carac + 1];
                int cont = 1;
                rango[0] = "(";
                for (int i = 1; i <= rango.Length - 1; i = i + 2)
                {
                    rango[i] = ConvierteNumAStr(cadena[cont] - 48);
                    cont++;
                    if (i == rango.Length - 1)
                    {
                    }
                    else
                    {
                        rango[i + 1] = "|";
                    }
                }
                rango[rango.Length - 1] = ")";

                return rango;
            }
        }



        public int ConvierteLetra(string letra)
        {
            int Num = -1;

            if (letra == "a")
            {
                Num = 1;
            }
            else if (letra == "b")
            {
                Num = 2;
            }
            else if (letra == "c")
            {
                Num = 3;
            }
            else if (letra == "d")
            {
                Num = 4;
            }
            else if (letra == "e")
            {
                Num = 5;
            }
            else if (letra == "f")
            {
                Num = 6;
            }
            else if (letra == "g")
            {
                Num = 7;
            }
            else if (letra == "h")
            {
                Num = 8;
            }
            else if (letra == "i")
            {
                Num = 9;
            }
            else if (letra == "j")
            {
                Num = 10;
            }
            else if (letra == "k")
            {
                Num = 11;
            }
            else if (letra == "l")
            {
                Num = 12;
            }
            else if (letra == "m")
            {
                Num = 13;
            }
            else if (letra == "n")
            {
                Num = 14;
            }
            else if (letra == "o")
            {
                Num = 15;
            }
            else if (letra == "p")
            {
                Num = 16;
            }
            else if (letra == "q")
            {
                Num = 17;
            }
            else if (letra == "r")
            {
                Num = 18;
            }
            else if (letra == "s")
            {
                Num = 19;
            }
            else if (letra == "t")
            {
                Num = 20;
            }
            else if (letra == "u")
            {
                Num = 21;
            }
            else if (letra == "v")
            {
                Num = 22;
            }
            else if (letra == "w")
            {
                Num = 23;
            }
            else if (letra == "x")
            {
                Num = 24;
            }
            else if (letra == "y")
            {
                Num = 25;
            }
            else if (letra == "z")
            {
                Num = 26;
            }
            else if (letra == "A")
            {
                Num = 27;
            }
            else if (letra == "B")
            {
                Num = 28;
            }
            else if (letra == "C")
            {
                Num = 29;
            }
            else if (letra == "D")
            {
                Num = 30;
            }
            else if (letra == "E")
            {
                Num = 31;
            }
            else if (letra == "F")
            {
                Num = 32;
            }
            else if (letra == "G")
            {
                Num = 33;
            }
            else if (letra == "H")
            {
                Num = 34;
            }
            else if (letra == "I")
            {
                Num = 35;
            }
            else if (letra == "J")
            {
                Num = 36;
            }
            else if (letra == "K")
            {
                Num = 37;
            }
            else if (letra == "L")
            {
                Num = 38;
            }
            else if (letra == "M")
            {
                Num = 39;
            }
            else if (letra == "N")
            {
                Num = 40;
            }
            else if (letra == "O")
            {
                Num = 41;
            }
            else if (letra == "P")
            {
                Num = 42;
            }
            else if (letra == "Q")
            {
                Num = 43;
            }
            else if (letra == "R")
            {
                Num = 44;
            }
            else if (letra == "S")
            {
                Num = 45;
            }
            else if (letra == "T")
            {
                Num = 46;
            }
            else if (letra == "U")
            {
                Num = 47;
            }
            else if (letra == "V")
            {
                Num = 48;
            }
            else if (letra == "W")
            {
                Num = 49;
            }
            else if (letra == "X")
            {
                Num = 50;
            }
            else if (letra == "Y")
            {
                Num = 51;
            }
            else if (letra == "Z")
            {
                Num = 52;
            }
            return Num;

        }

        public string ConvierteNumero(int letra)
        {
            string Num = "A";

            if (letra == 1)
            {
                Num = "a";
            }
            else if (letra == 2)
            {
                Num = "b";
            }
            else if (letra == 3)
            {
                Num = "c";
            }
            else if (letra == 4)
            {
                Num = "d";
            }
            else if (letra == 5)
            {
                Num = "e";
            }
            else if (letra == 6)
            {
                Num = "f";
            }
            else if (letra == 7)
            {
                Num = "g";
            }
            else if (letra == 8)
            {
                Num = "h";
            }
            else if (letra == 9)
            {
                Num = "i";
            }
            else if (letra == 10)
            {
                Num = "j";
            }
            else if (letra == 11)
            {
                Num = "k";
            }
            else if (letra == 12)
            {
                Num = "l";
            }
            else if (letra == 13)
            {
                Num = "m";
            }
            else if (letra == 14)
            {
                Num = "n";
            }
            else if (letra == 15)
            {
                Num = "o";
            }
            else if (letra == 16)
            {
                Num = "p";
            }
            else if (letra == 17)
            {
                Num = "q";
            }
            else if (letra == 18)
            {
                Num = "r";
            }
            else if (letra == 19)
            {
                Num = "s";
            }
            else if (letra == 20)
            {
                Num = "t";
            }
            else if (letra == 21)
            {
                Num = "u";
            }
            else if (letra == 22)
            {
                Num = "v";
            }
            else if (letra == 23)
            {
                Num = "w";
            }
            else if (letra == 24)
            {
                Num = "x";
            }
            else if (letra == 25)
            {
                Num = "y";
            }
            else if (letra == 26)
            {
                Num = "z";
            }
            else if (letra == 27)
            {
                Num = "A";
            }
            else if (letra == 28)
            {
                Num = "B";
            }
            else if (letra == 29)
            {
                Num = "C";
            }
            else if (letra == 30)
            {
                Num = "D";
            }
            else if (letra == 31)
            {
                Num = "E";
            }
            else if (letra == 32)
            {
                Num = "F";
            }
            else if (letra == 33)
            {
                Num = "G";
            }
            else if (letra == 34)
            {
                Num = "H";
            }
            else if (letra == 35)
            {
                Num = "I";
            }
            else if (letra == 36)
            {
                Num = "J";
            }
            else if (letra == 37)
            {
                Num = "K";
            }
            else if (letra == 38)
            {
                Num = "L";
            }
            else if (letra == 39)
            {
                Num = "M";
            }
            else if (letra == 40)
            {
                Num = "N";
            }
            else if (letra == 41)
            {
                Num = "O";
            }
            else if (letra == 42)
            {
                Num = "P";
            }
            else if (letra == 43)
            {
                Num = "Q";
            }
            else if (letra == 44)
            {
                Num = "R";
            }
            else if (letra == 45)
            {
                Num = "S";
            }
            else if (letra == 46)
            {
                Num = "T";
            }
            else if (letra == 47)
            {
                Num = "U";
            }
            else if (letra == 48)
            {
                Num = "V";
            }
            else if (letra == 49)
            {
                Num = "W";
            }
            else if (letra == 50)
            {
                Num = "X";
            }
            else if (letra == 51)
            {
                Num = "Y";
            }
            else if (letra == 52)
            {
                Num = "Z";
            }

            return Num;

        }

        public string ConvierteNumAStr(int letra)
        {
            string Num = "0";


            if (letra == 0)
            {
                Num = "0";
            }
            else if (letra == 1)
            {
                Num = "1";
            }
            else if (letra == 2)
            {
                Num = "2";
            }
            else if (letra == 3)
            {
                Num = "3";
            }
            else if (letra == 4)
            {
                Num = "4";
            }
            else if (letra == 5)
            {
                Num = "5";
            }
            else if (letra == 6)
            {
                Num = "6";
            }
            else if (letra == 7)
            {
                Num = "7";
            }
            else if (letra == 8)
            {
                Num = "8";
            }
            else if (letra == 9)
            {
                Num = "9";
            }
            return Num;
        }


    }
}


