using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    internal class Clasifica_Tokens
    {
        public int contCaracter =0;
        public string aux ="";
        private AFD identificador;
        private AFD numero;
        private List<string> palabras_reservadas;
        private List<string> simbolos_especiales;
        private List<List<char>> palabras;
        private List<int> num_linea;
        List<List<string>> relacionesAFDLexemaId = new List<List<string>>();
        List<List<string>> relacionesAFDLexemaNum = new List<List<string>>();


        public Clasifica_Tokens(AFD identificador, AFD numero)
        {
            this.identificador = identificador;
            this.numero = numero;
            num_linea = new List<int>();
            palabras = new List<List<char>>();
            palabras_reservadas = new List<string>();
            palabras_reservadas.Add("select");
            palabras_reservadas.Add("from");
            palabras_reservadas.Add("where");
            palabras_reservadas.Add("insert");
            palabras_reservadas.Add("into");
            palabras_reservadas.Add("values");
            palabras_reservadas.Add("update");
            palabras_reservadas.Add("set");
            palabras_reservadas.Add("delete");
            palabras_reservadas.Add("if");
            palabras_reservadas.Add("then");
            palabras_reservadas.Add("else");
            palabras_reservadas.Add("end");
            palabras_reservadas.Add("repeat");
            palabras_reservadas.Add("until");
            ;

            simbolos_especiales = new List<string>();
            simbolos_especiales.Add("+");
            simbolos_especiales.Add("-");
            simbolos_especiales.Add("*");
            simbolos_especiales.Add("/");
            simbolos_especiales.Add("=");
            simbolos_especiales.Add("<");
            simbolos_especiales.Add(">");
            simbolos_especiales.Add("<=");
            simbolos_especiales.Add(">=");
            simbolos_especiales.Add("(");
            simbolos_especiales.Add(")");
            simbolos_especiales.Add(";");
            simbolos_especiales.Add(":=");
            simbolos_especiales.Add(",");
        }



        /*
         * Método que se encarga de dividir palabra por palabra la entrada de texto en 
         * gramática TINY
         * @Param: Recibe una string con todo el programa ingresado en gramática TINY
         * @Return: Regresa una lista de listas de char, la cual contiene cada una de las
         *          palabras, símbolos especiales y operadores del programa         */
        public List<List<char>> Divide_Cadena(string cadena)
        {
            Console.WriteLine(cadena);
            cadena += '\n';
            char[] caracteres = cadena.ToCharArray();

            int linea = 1;
            int lim_izq = 0, lim_der = 0;
            List<char> caracteresID = new List<char>();
            List<char> sEspeciales = new List<char>();
            for (int i = 0; i < caracteres.Length; i++)
            {
                if (caracteres[i] == '\t')
                    continue;
                if (caracteres[i] == '\r')
                    continue;
                if (caracteres[i] == '\n')
                {
                    palabras.Add(caracteresID);
                    lim_izq = lim_der;
                    lim_der = palabras.Count;
                    Console.Write("Salto de linea ----- ");
                    if (caracteresID.Count > 0)
                        Console.WriteLine("Palabras " + palabras.Count + " " + caracteresID[caracteresID.Count - 1]);
                    for (int j = lim_izq; j < lim_der; j++)
                    {
                        num_linea.Add(linea);
                    }
                    linea++;


                    caracteresID = new List<char>();
                    continue;
                }
                if (caracteres[i] == ' ')
                {
                    if (caracteres[i + 1] == ' ')
                        continue;
                    if (EsCaracterEspecial(caracteres[i + 1]) || caracteres[i + 1] == ':')
                    {
                        if (caracteresID.Count > 0)
                        {
                            palabras.Add(caracteresID);
                            caracteresID = new List<char>();
                            continue;
                        }

                    }
                    palabras.Add(caracteresID);
                    caracteresID = new List<char>();
                    continue;
                }
                //error Aqui 
                if ((EsCaracterEspecial(cadena[i]) || cadena[i] == ':') && caracteresID.Count > 0)
                {
                    if (caracteres.Length - 1 > i && caracteres[i + 1] == '=')
                    {
                        palabras.Add(caracteresID);
                        caracteresID = new List<char>();
                        sEspeciales = new List<char>();
                        sEspeciales.Add(cadena[i]);
                        sEspeciales.Add(cadena[i + 1]);
                        i++;
                        palabras.Add(sEspeciales);
                        continue;
                    }

                    palabras.Add(caracteresID);
                    caracteresID = new List<char>();
                    sEspeciales = new List<char>();
                    sEspeciales.Add(cadena[i]);
                    palabras.Add(sEspeciales);
                    continue;
                }

                if (!EsCaracterEspecial(cadena[i]) && cadena[i] != ':')
                {
                    contCaracter = 0;
                    caracteresID.Add(cadena[i]);
                    if (i == caracteres.Length - 1)
                    {
                        palabras.Add(caracteresID);
                        caracteresID = new List<char>();
                    }
                    continue;


                }
                if ((cadena[i] == ':' && caracteres.Length - 1 > i && caracteres[i + 1] == '='))
                {

                    if (caracteresID.Count == 0)
                    {
                        sEspeciales = new List<char>();
                        sEspeciales.Add(cadena[i]);
                        sEspeciales.Add(cadena[i + 1]);
                        palabras.Add(sEspeciales);
                        i++;
                        continue;
                    }
                    else
                    {
                        palabras.Add(caracteresID);
                        caracteresID = new List<char>();
                        sEspeciales.Add(cadena[i]);
                        sEspeciales.Add(cadena[i + 1]);
                        palabras.Add(sEspeciales);
                        i++;
                        continue;
                    }


                }
                //Corrección de error con esta función
                if (EsCaracterEspecial(cadena[i]))
                {
                    sEspeciales = new List<char>();
                    sEspeciales.Add(cadena[i]);
                    if (EsCaracterEspecial(cadena[i+1]))
                    {
                        i++;
                        sEspeciales.Add(cadena[i]);
                    }  
                    palabras.Add(sEspeciales);
                    continue;
                }
                //fin de la corrección

            }

            List<List<char>> nuevo = Elimina_Vacios(palabras);
            imprimeNuevo(nuevo);
            return nuevo;
        }


        private void imprimeNuevo(List<List<char>> list)
        {
            for (int i = 0; i < list.Count; i++)
            {

                for (int j = 0; j < list[i].Count; j++)
                {
                    Console.Write(list[i][j]);
                }
                Console.WriteLine();
            }

            for (int i = 0; i < num_linea.Count; i++)
                Console.WriteLine(num_linea[i] + " ");
        }


        /*
         * Método que se encarga de la clasificación de cada una de las palabras 
         * @Param: ---Recibe una lista de listas con el desglose palabra por palabra
         *         ---AFD de identificadores
         *         ---AFD de números
         *         ---Estados de aceptación del AFD de identificadores
         *         ---Estados de aceptación del AFD de números
         * @Return: Regresa una lista de listas con las respectivas clasificaciones
         *          Por ejemplo:
         *          read --- read
         *          identificador --- x
         *          Error Léxico --- write1
         */

        public List<List<string>> Clasificar_Tokens(List<List<char>> tokens, AFD afdId, AFD afdNum, List<int> ea_id, List<int> ea_num)
        {
            string s;
            List<List<string>> evaluacion = new List<List<string>>();
            for (int i = 0; i < tokens.Count; i++)
            {
                s = "";
                for (int j = 0; j < tokens[i].Count; j++)
                {
                    s += tokens[i][j].ToString();
                }
                Evalua(s, evaluacion, afdId, afdNum, ea_id, ea_num);
            }
            return evaluacion;
        }

        /*
         * Funcion que determina la categoria del string enviado*
         * @Param :string a evaluar
         *          lista de listas donde se regresara la palabra con su identificación
         *          AFD identificadores
         *          AFD números
         *          Estados de aceptación del AFD identificadores
         *          Estados de aceptación del AFD números
         */

        private void Evalua(string s, List<List<string>> evaluacion, AFD afdId, AFD afdNum, List<int> ea_id, List<int> ea_num)
        {
            //Es un símbolo especiasl
            if (EsCaracterEspecialS(s))
            {
                List<string> l = new List<string>();
                l.Add(s);
                l.Add(s);
                evaluacion.Add(l);
                return;
            }
            //Es una palabra reservada
            if (EsPalabraReservada(s))
            {
                List<string> l = new List<string>();
                l.Add(s);
                l.Add(s);
                evaluacion.Add(l);
                return;
            }
            //Verifica en el AFD de identificadores
            int NumEncaId = 0;
            afdId.CreaTabla(ref relacionesAFDLexemaId, ref NumEncaId);
            int resLexId = 0, EstadoId = 0, ResFinId = 0;
            Lexema lexemaId = new Lexema();
            resLexId = lexemaId.VerificaTokens(s.ToCharArray(), relacionesAFDLexemaId, ref EstadoId, 0, NumEncaId);

            if (resLexId == 1)
            {
                ResFinId = RespuestaFinal(ea_id, EstadoId);

            }
            if (ResFinId == 1)
            {
                List<string> l = new List<string>();
                l.Add("letter");
                l.Add(s);
                evaluacion.Add(l);
                return;

            }

            //Verifica en el AFD de números
            int NumEncaNum = 0;
            afdNum.CreaTabla(ref relacionesAFDLexemaNum, ref NumEncaNum);
            int resLexNum = 0, EstadoNum = 0, ResFinNum = 0;
            Lexema lexemaNum = new Lexema();
            resLexNum = lexemaNum.VerificaTokens(s.ToCharArray(), relacionesAFDLexemaNum, ref EstadoNum, 0, NumEncaNum);
            if (resLexNum == 1)
            {
                ResFinNum = RespuestaFinal(ea_num, EstadoNum);
                //Si ResFin es igual a 1 si es aceptada, si no no 
            }
            if (ResFinNum == 1)
            {
                List<string> l = new List<string>();
                l.Add("digit");
                l.Add(s);
                evaluacion.Add(l);
                return;

            }
            else
            {
                //No fue encontrado en ninguno de los anteriores por lo que es un error léxico
                List<string> l = new List<string>();
                l.Add("Error Léxico");
                l.Add(s);
                evaluacion.Add(l);
                return;
            }

        }

        private List<List<char>> Elimina_Vacios(List<List<char>> list)
        {
            Console.WriteLine("numero de palabras " + list.Count);

            Console.WriteLine("Numero de linea de cada palabra " + num_linea.Count);
            List<List<char>> ret = new List<List<char>>();
            List<int> auxNum_linea = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Count > 0)
                {
                    ret.Add(list[i]);
                    auxNum_linea.Add(num_linea[i]);
                }

            }

            num_linea = auxNum_linea;
            return ret;

        }

        private bool EsCaracterEspecial(char c)
        {
            bool band = false;
            for (int i = 0; i < simbolos_especiales.Count && band == false; i++)
            {
                if (simbolos_especiales[i] == c.ToString())
                {
                    //contCaracter++;
                    return band = true;
                }
            }
            return band;
        }

        private bool EsPalabraReservada(string s)
        {
            bool band = false;
            int i = 0;
            while (band == false && i < palabras_reservadas.Count())
            {
                if (s == palabras_reservadas[i])
                    band = true;
                i++;

            }
            return band;
        }

        private bool EsCaracterEspecialS(string s)
        {
            bool band = false;
            int i = 0;
            while (band == false && i < simbolos_especiales.Count())
            {
                if (s == simbolos_especiales[i])
                    band = true;
                i++;

            }
            return band;
        }


        private int RespuestaFinal(List<int> list, int Estado)
        {
            int RF = 0;
            for (int k = 0; k < list.Count; k++)
            {
                if (Estado == list[k])
                {
                    RF = 1;
                    break;
                }
                else
                    if (k == list.Count - 1)
                    RF = 0;
            }
            return RF;
        }

        public List<int> Num_linea
        {
            get { return num_linea; }
        }
    }
}

