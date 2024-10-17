using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Proyecto_Compiladores
{
    public partial class Form1 : Form
    {
        public List<string> token;
        Posfija postF = new Posfija();
        public string stringposfija;
        public AFN afn;
        public AFD afd;
        public SQL g;
        public List<string> TokensLexema;
        public List<string> TokensText;

        private List<char> posfija;
        private Expresion_Regular exp;
        private List<List<Arista>> lista_adyacencia;
        // private AFN afn;
        List<char> alfabeto;
        List<List<string>> relacionesAFD = new List<List<string>>();

        public string s1;
        public string a1 = "";
        public string linea;
        public string NOMToken;

        List<List<string>> AFDenOrden = new List<List<string>>();
        List<string> auxiliar = new List<string>();
        int NumEmcaGlob = 0, repetidos = 0;
        List<int> EdosAcepta = new List<int>();

        public int OpcionMathER = 0;
        string posfij;
        char[] ArrayER = new char[500];
        Stack<string> Pila;
        public string ExpresionRegular;
        string[] ArrayInfija;
        //ER
        public int capacidadArrayInfija = 0; //no perder la ultima posicion del arreglo ArrayInfija
        char[] alpha = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        string[] ArrayPosfija = new string[500];
        //Gramatic Lista = new Gramatic();

        List<List<string>> resultado = new List<List<string>>();
        List<TreeNode> arbolaux = new List<TreeNode>();



        ///////////////// LR(0) /////////////////

        List<List<string>> GramSQL = new List<List<string>>();
        List<string> FilasGT = new List<string>();
        List<string> PalRes = new List<string>();
        List<string> SimbEsp = new List<string>();
        List<string> Terminales = new List<string>();
        List<string> NoTerminales = new List<string>();
        List<List<string>> Siguientes = new List<List<string>>();
        List<string> SigAux = new List<string>();
        List<string> FilasJ = new List<string>();
        List<List<List<string>>> In = new List<List<List<string>>>();
        List<string> auxConecte = new List<string>();
        List<List<string>> Conecte = new List<List<string>>();
        List<string> auxTAS = new List<string>();
        List<List<string>> TAS = new List<List<string>>();


        //Sexta entrega

        public Form1()
        {
            //Pila = new Stack<string>();
            InitializeComponent();
            ExpReg.Enabled = false;
            textBox2.Enabled = false;
            button1.Enabled = false;
            posfija = new List<char>();
            // afn = new AFN();
            exp = new Expresion_Regular();
            this.WindowState = FormWindowState.Maximized;
        }

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
                                if (pila.Count == 0 || pila.Peek() == '(')
                                //|| operadores[infija[cont]] > operadores[pila.Peek()])
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

        /**
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





        /** -------------- Método 1 -------------**/
        /*Escribir método para crear una secuencia de caracteres
        * tanto números, letras en mayúscula y minúscula*
        * Por ejemplo, la función recibe el seguiente parametro
        * "[A-D]" y la funcion regresaría un string con la siguiente cadena
        * "(A|B|C|D)"
        */

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            if (textBox1.Text != "")
            {
                textBox2.Text = "";//El texBox no se llene de información 
                ExpresionRegular = textBox1.Text; //Expresion regular junta
                ArrayER = ExpresionRegular.ToCharArray();//Expresion regular separada
                Pila = new Stack<string>(500);
                ArrayInfija = new string[500];
                SepararCadena();
                switch (OpcionMathER)
                {
                    case 0: //No es ni matematica ni regular
                        MessageBox.Show("Escoja si es Expresión matematica o si es Expresión Regular");
                        break;
                    case 1://Expresión matematica
                        InfijaAPosfija();
                        textBox2.Enabled = true;
                        textBox2.Text = posfija;
                        button1.Enabled = false;
                        break;
                    case 2://Expresión regular
                        InfijaAPosfijaER();
                        textBox2.Text = posfija;
                        break;
                }
            }
            else
            {
                MessageBox.Show("Escriba la expresión");
            }*/

            //ExpReg.Text = " "; //limpia cuadro de texto del resultado
            textBox2.Text = " "; //limpia cuadro de texto del resultado

            //lista donde se recibe el resultado
            List<char> CadenaRes = new List<char>();
            List<char> CadenaRes2 = new List<char>();
            List<char> postfija = new List<char>();
            Expresion_Regular ExReg = new Expresion_Regular();
            //manda llamar a los métodos
            CadenaRes = ExReg.Recibo(ExpReg.Text);
            CadenaRes2 = ExReg.Concatena(CadenaRes);
            postfija = posfija = ExReg.Postfija(CadenaRes2);

            exp = ExReg;
            Debug.WriteLine("Recibo la cadena" + exp);
            //imprime el resultado
            for (int i = 0; i < postfija.Count; i++)
            {
                textBox2.Text = textBox2.Text + postfija[i];
                Debug.WriteLine("posfija " + posfija[i]);
            }


        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {


        }
        //Inicio expresion matematica
        public int PrioridadOperaciones(string opcion)//Paso uno Definir las prioridades
        {
            int prioridad = 0;
            switch (opcion)
            {
                case "x": prioridad = 1; break;
                case "/": prioridad = 1; break;
                case "+": prioridad = 2; break;
                case "-": prioridad = 2; break;
            }
            Debug.WriteLine("La prioridad de " + opcion + " es " + prioridad);
            return prioridad;
        }

        public void InfijaAPosfija()
        {
            int contArray = 0, Tope = 0;
            while (ArrayER != null && contArray != ArrayER.Length) 
            {
                switch (ArrayER[contArray].ToString())
                {
                    case "(": 
                        Pila.Push("(");
                        Tope++;
                        break;
                    case ")": 
                        Tope = ExtraerPila(Tope);
                        break;
                    //"Operado" -> Desplegar en posfija
                    case "1":
                        posfij += ArrayER[contArray];
                        break;
                    case "2":
                        posfij += ArrayER[contArray];
                        break;
                    case "3":
                        posfij += ArrayER[contArray];
                        break;
                    case "4":
                        posfij += ArrayER[contArray];
                        break;
                    case "5":
                        posfij += ArrayER[contArray];
                        break;
                    case "6":
                        posfij += ArrayER[contArray];
                        break;
                    case "7":
                        posfij += ArrayER[contArray];
                        break;
                    case "8":
                        posfij += ArrayER[contArray];
                        break;
                    case "9":
                        posfij += ArrayER[contArray];
                        break;
                    //Operador
                    case "+":
                        Tope = Operador(ArrayER[contArray].ToString(), Tope);
                        break;
                    case "-":
                        Tope = Operador(ArrayER[contArray].ToString(), Tope);
                        break;
                    case "x":
                        Tope = Operador(ArrayER[contArray].ToString(), Tope);
                        break;
                    case "/":
                        Tope = Operador(ArrayER[contArray].ToString(), Tope);
                        break;
                }
                contArray++;//Apunta al siguiente caracter de la expresion infija
            }
            if (Tope != 0)
            {
                for (int i = Tope; i > 0; i--)
                {
                    posfij += Pila.Pop();
                }
            }
        }

        public int Operador(string caracter, int Tope)
        {

            bool band = true;
            while (band)
            {//pila vacia o el tope de la pila es un "Parentesis Izquierdo" o el operador tiene mayor prioridad que el tope de la pila
                Debug.WriteLine(caracter);
                if (Tope == 0)//la pila esta vacia
                {
                    Pila.Push(caracter);
                    Tope++;//Aumenta el tope de la pila
                    band = false;
                }
                else
                {
                    if (Pila.Peek() == "(" || PrioridadOperaciones(Pila.Peek()) > PrioridadOperaciones(caracter))
                    {

                        Pila.Push(caracter);
                        Tope++;
                        band = false;
                    }

                    else
                    {//Extraer el tope de la pila y desplegar en posfija
                        Debug.WriteLine("Pila.Peek() =" + Pila.Peek());
                        posfij += Pila.Pop();
                    }
                }

            }
            return Tope;
        }
        public int ExtraerPila(int Tope)
        {
            int contaux = 0;//contar las veces que se quita de la pila
            for (int i = Tope; i > 0; i--)
            {
                if (Pila.Peek() != "(")
                {
                    Debug.WriteLine("Se insertara " + Pila.Peek());
                    posfij += Pila.Pop();

                    Debug.WriteLine("La posfija va : " + posfija);
                    contaux++;
                }
                else
                {

                    Debug.WriteLine("Se ELIMINA " + Pila.Peek());
                    Pila.Pop();
                    contaux++;
                    return Tope - contaux;
                }
            }
            return Tope - contaux;
        }
        public void SepararCadena()
        {
            for (int i = 0; i < ExpresionRegular.Length; i++)
            {
                textBox2.Text += ArrayER[i] + "-";
            }
        }


        //Fin expresion matematica

        //Inicio de la expresion regular


        public void InfijaAPosfijaER()
        {
            ConvierteER();
            ExplicitaPosInfija();
        }
        public int PrioridadOperacionesER(string opcion)//Paso uno Definir las prioridades
        {
            int prioridad = 0;
            switch (opcion)
            {
                case "|": prioridad = 3; break;
                case "&": prioridad = 2; break;
                case "*": prioridad = 1; break;
                case "+": prioridad = 1; break;
                case "?": prioridad = 1; break;
            }
            Debug.WriteLine("La prioridad de " + opcion + " es " + prioridad);
            return prioridad;
        }
        public void ExplicitaPosInfija()
        {
            textBox2.Text = "";//El texBox no se llene de información 
            Console.WriteLine(posfija);
            ArrayER = posfij.ToCharArray();
            posfij = "";
            int contArray = 0, Tope = 0;
            while (contArray != ArrayER.Length) 
            {
                imprime();
                Debug.WriteLine("La posfja va: " + posfija);
                Debug.WriteLine("ArrayER[contArray] = " + ArrayER[contArray].ToString());
                switch (ArrayER[contArray].ToString())
                {
                    case "(": 
                        Pila.Push("(");
                        Tope++;//Capacidad de la pila
                        break;
                    case ")"://"Parentesis derecho" -> Extraer de la pila y desplegar en posfija hasta encontrar "Parentesis izquierdo" (no desplegarlo)
                        Tope = ExtraerPilaER(Tope);
                        break;
                    //Operador
                    case "+":
                        Tope = OperadorER(ArrayER[contArray].ToString(), Tope);
                        break;
                    case "*":
                        Tope = OperadorER(ArrayER[contArray].ToString(), Tope);
                        break;
                    case "?":
                        Tope = OperadorER(ArrayER[contArray].ToString(), Tope);
                        break;
                    case "&":
                        Tope = OperadorER(ArrayER[contArray].ToString(), Tope);
                        break;
                    case "|":
                        Tope = OperadorER(ArrayER[contArray].ToString(), Tope);
                        break;

                    //"Operado" -> Desplegar en posfija
                    default:
                        posfij += ArrayER[contArray].ToString();
                        break;
                }
                contArray++;//Apunta al siguiente caracter de la expresion infija
            }
            textBox2.Enabled = true;
            if (Tope != 0)
            {
                for (int i = Tope; i > 0; i--)
                {
                    posfij += Pila.Pop();
                }
            }

        }

        public int OperadorER(string caracter, int Tope)
        {

            bool band = true;
            while (band)
            {//pila vacia o el tope de la pila es un "Parentesis Izquierdo" o el operador tiene mayor prioridad que el tope de la pila
                Debug.WriteLine(caracter);
                if (Tope == 0)//la pila esta vacia
                {
                    Pila.Push(caracter);
                    Tope++;//Aumenta el tope de la pila
                    band = false;
                }
                else
                {
                    if (Pila.Peek() == "(" || PrioridadOperacionesER(Pila.Peek()) > PrioridadOperacionesER(caracter))
                    {
                        Debug.WriteLine("Pila.Peek() =" + Pila.Peek() + "Caracter " + caracter);
                        Pila.Push(caracter);
                        Tope++;
                        band = false;
                    }
                    else
                    {//Extraer el tope de la pila y desplegar en posfija
                        Debug.WriteLine("Pila.Peek() =" + Pila.Peek());
                        posfij += Pila.Pop();
                        Tope--;

                    }
                }

            }
            return Tope;
        }
        public int ExtraerPilaER(int Tope)
        {
            int contaux = 0;//contar las veces que se quita de la pila
            for (int i = Tope; i > 0; i--)
            {
                if (Pila.Peek() != "(")
                {
                    Debug.WriteLine("Se insertara " + Pila.Peek());
                    posfij += Pila.Pop();

                    Debug.WriteLine("La posfija va : " + posfija);
                    contaux++;
                }
                else
                {

                    Debug.WriteLine("Se ELIMINA " + Pila.Peek());
                    Pila.Pop();
                    contaux++;
                    return Tope - contaux;
                }
            }
            return Tope - contaux;
        }


        public void ConvierteER()
        {
            int cont = 0;//para saber cuantos caracteres (letras o numeros) estan unidos
            for (int i = 0; i < ArrayER.Length; i++)
            {
                switch (ArrayER[i].ToString())
                {
                    case "(":
                        imprime();
                        cont = 0;//se iguala a cero por que ya no hay letras o numeros juntos
                        ArrayInfija[capacidadArrayInfija] = "(";
                        capacidadArrayInfija++;
                        i = Ampersam(i + 1, 1) - 1;
                        break;
                    case ")":
                        cont = 0;
                        if (i == ArrayER.Length)
                        {
                            ArrayInfija[capacidadArrayInfija] = ")";
                            capacidadArrayInfija++;
                        }
                        else
                        {
                            if (ArrayER[i + 1].ToString() == "(" || ArrayER[i + 1].ToString() == "[")
                            {
                                ArrayInfija[capacidadArrayInfija] = ")";
                                capacidadArrayInfija++;
                                ArrayInfija[capacidadArrayInfija] = "&";
                                capacidadArrayInfija++;
                            }
                            else
                            {
                                ArrayInfija[capacidadArrayInfija] = ")";
                                capacidadArrayInfija++;
                            }
                        }
                        break;
                    case "[":
                        cont = 0;//se iguala a cero por que ya no hay letras o numeros juntos
                        ArrayInfija[capacidadArrayInfija] = "(";
                        capacidadArrayInfija++;
                        i = Ampersam(i + 1, 2) - 1;
                        break;
                    case "]":
                        cont = 0;
                        if (i == ArrayER.Length - 1)
                        {
                            ArrayInfija[capacidadArrayInfija] = ")";
                            capacidadArrayInfija++;
                        }
                        else
                        {
                            if (ArrayER[i + 1].ToString() == "(" || ArrayER[i + 1].ToString() == "[")
                            {
                                ArrayInfija[capacidadArrayInfija] = ")";
                                capacidadArrayInfija++;
                                ArrayInfija[capacidadArrayInfija] = "&";
                                capacidadArrayInfija++;
                            }
                            else
                            {
                                ArrayInfija[capacidadArrayInfija] = ")";
                                capacidadArrayInfija++;
                            }
                        }
                        break;
                    case "-":
                        cont = 0;
                        int NL = Numero_O_Letra(i - 1);
                        if (NL == -1)//letra
                        {
                            InsertaLegtra(i - 1, i + 1);
                            i++;
                        }
                        else//numero
                        {
                            int aux = Int32.Parse(ArrayER[i + 1].ToString());
                            Inserta(NL, aux, i);
                            i++;
                        }
                        break;
                    case "|":
                        ArrayInfija[capacidadArrayInfija] = "|";
                        capacidadArrayInfija++;
                        cont = 0;
                        break;
                    case "*":
                        if (ArrayER.Length == i)
                        {
                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                            capacidadArrayInfija++;
                        }
                        else
                        {
                            if (i + 1 < ArrayER.Length)
                            {
                                if (ArrayER[i + 1].ToString() == ")" || ArrayER[i + 1].ToString() == "]" || ArrayER[i + 1].ToString() == "|" || ArrayER[i + 1].ToString() == "*" || ArrayER[i + 1].ToString() == "+" || ArrayER[i + 1].ToString() == "?")
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                }
                                else
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                    ArrayInfija[capacidadArrayInfija] = "&";
                                    capacidadArrayInfija++;
                                }
                            }
                            else
                            {
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++;
                            }
                        }
                        break;
                    case "?":
                        if (ArrayER.Length == i)
                        {
                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                            capacidadArrayInfija++;
                        }
                        else
                        {
                            if (i + 1 < ArrayER.Length)
                            {
                                if (ArrayER[i + 1].ToString() == ")" || ArrayER[i + 1].ToString() == "]" || ArrayER[i + 1].ToString() == "|" || ArrayER[i + 1].ToString() == "*" || ArrayER[i + 1].ToString() == "+" || ArrayER[i + 1].ToString() == "?")
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                }
                                else
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                    ArrayInfija[capacidadArrayInfija] = "&";
                                    capacidadArrayInfija++;
                                }
                            }
                            else
                            {
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++;
                            }
                        }
                        cont = 0;
                        break;
                    case "+":
                        if (ArrayER.Length == i)
                        {
                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                            capacidadArrayInfija++;
                        }
                        else
                        {
                            if (i + 1 < ArrayER.Length)
                            {
                                if (ArrayER[i + 1].ToString() == ")" || ArrayER[i + 1].ToString() == "]" || ArrayER[i + 1].ToString() == "|" || ArrayER[i + 1].ToString() == "*" || ArrayER[i + 1].ToString() == "+" || ArrayER[i + 1].ToString() == "?")
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                }
                                else
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                    ArrayInfija[capacidadArrayInfija] = "&";
                                    capacidadArrayInfija++;
                                }
                            }
                            else
                            {
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++;
                            }
                        }
                        cont = 0;
                        break;
                    default:
                        if (ArrayER.Length > i + 1)//evitar que busque fuera del tamaño del arreglo
                        {
                            if (cont == 0)
                            {
                                if (i == 0)
                                {
                                    if (ArrayER[i + 1].ToString() == "(" || ArrayER[i + 1].ToString() == "[")
                                    {

                                        ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                        capacidadArrayInfija++; //aumenta por el dato que entra
                                        ArrayInfija[capacidadArrayInfija] = "&";
                                        capacidadArrayInfija++; //aumenta por el or |
                                        cont++;
                                    }
                                    else
                                    {
                                        if (ArrayER[i + 1].ToString() == ")" || ArrayER[i + 1].ToString() == "]" || ArrayER[i + 1].ToString() == "+" || ArrayER[i + 1].ToString() == "*" || ArrayER[i + 1].ToString() == "?" || ArrayER[i + 1].ToString() == "|")
                                        {
                                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                            capacidadArrayInfija++;
                                        }
                                        else
                                        {
                                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                            capacidadArrayInfija++;
                                            ArrayInfija[capacidadArrayInfija] = "&";
                                            capacidadArrayInfija++;
                                            cont++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (ArrayER[i + 1].ToString() == "(" || ArrayER[i + 1].ToString() == "[")
                                    {

                                        ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                        capacidadArrayInfija++; //aumenta por el dato que entra
                                        ArrayInfija[capacidadArrayInfija] = "&";
                                        capacidadArrayInfija++; //aumenta por el or |
                                        cont++;
                                    }
                                    else
                                    {
                                        if (ArrayER[i - 1].ToString() == "(" || ArrayER[i - 1].ToString() == "[")
                                        {
                                            ArrayInfija[capacidadArrayInfija] = "&";
                                            capacidadArrayInfija++;
                                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                            capacidadArrayInfija++;
                                            cont++;
                                        }
                                        else
                                        {
                                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                            capacidadArrayInfija++;
                                            cont++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (ArrayER[i + 1].ToString() == "(" || ArrayER[i + 1].ToString() == "[")
                                {

                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++; //aumenta por el dato que entra
                                    ArrayInfija[capacidadArrayInfija] = "&";
                                    capacidadArrayInfija++; //aumenta por el or |
                                    cont++;
                                }
                                else
                                {
                                    if (ArrayER[i + 1].ToString() == ")" || ArrayER[i + 1].ToString() == "]" || ArrayER[i + 1].ToString() == "+" || ArrayER[i + 1].ToString() == "*" || ArrayER[i + 1].ToString() == "?" || ArrayER[i + 1].ToString() == "|")
                                    {
                                        ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                        capacidadArrayInfija++;
                                    }
                                    else
                                    {
                                        ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                        capacidadArrayInfija++;
                                        ArrayInfija[capacidadArrayInfija] = "&";
                                        capacidadArrayInfija++;
                                        cont++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ArrayER[i - 1].ToString() == "]" || ArrayER[i - 1].ToString() == ")") {
                                ArrayInfija[capacidadArrayInfija] = "&";
                                capacidadArrayInfija++;
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++;

                            } else
                            {
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++;
                                cont++;
                            }
                        }
                        break;
                }
            }
            ImprimeCadena();
        }
        public int Ampersam(int posision, int tipo)
        {
            int cont = 0;
            for (int i = posision; i < ArrayER.Length; i++)
                switch (ArrayER[i].ToString())
                {
                    case "(":
                        imprime();
                        cont = 0;
                        ArrayInfija[capacidadArrayInfija] = "(";
                        capacidadArrayInfija++;
                        i = Ampersam(i + 1, 1) - 1;
                        break;
                    case ")":
                        return i;
                    case "[":
                        ArrayInfija[capacidadArrayInfija] = "(";
                        capacidadArrayInfija++;
                        i = Ampersam(i + 1, 2) - 1;
                        break;
                    case "]":
                        return i;
                    case "-":
                        cont = 0;
                        int NL = Numero_O_Letra(i - 1);
                        if (NL == -1)//letra
                        {
                            InsertaLegtra(i - 1, i + 1);
                            i++;
                        }
                        else//numero
                        {
                            int aux = Int32.Parse(ArrayER[i + 1].ToString());
                            Inserta(NL, aux, i);
                            i++;
                        }
                        break;
                    case "|":
                        ArrayInfija[capacidadArrayInfija] = "|";
                        capacidadArrayInfija++;
                        cont = 0;
                        break;
                    case "*":
                        if (ArrayER.Length == i)
                        {
                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                            capacidadArrayInfija++;
                        }
                        else
                        {
                            if (i + 1 < ArrayER.Length)
                            {
                                if (ArrayER[i + 1].ToString() == ")" || ArrayER[i + 1].ToString() == "]" || ArrayER[i + 1].ToString() == "|" || ArrayER[i + 1].ToString() == "*" || ArrayER[i + 1].ToString() == "+" || ArrayER[i + 1].ToString() == "?")
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                }
                                else
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                    ArrayInfija[capacidadArrayInfija] = "&";
                                    capacidadArrayInfija++;
                                }
                            }
                            else
                            {
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++;
                            }
                        }
                        break;
                    case "?":
                        if (ArrayER.Length == i)
                        {
                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                            capacidadArrayInfija++;
                        }
                        else
                        {
                            if (i + 1 < ArrayER.Length)
                            {
                                if (ArrayER[i + 1].ToString() == ")" || ArrayER[i + 1].ToString() == "]" || ArrayER[i + 1].ToString() == "|" || ArrayER[i + 1].ToString() == "*" || ArrayER[i + 1].ToString() == "+" || ArrayER[i + 1].ToString() == "?")
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                }
                                else
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                    ArrayInfija[capacidadArrayInfija] = "&";
                                    capacidadArrayInfija++;
                                }
                            }
                            else
                            {
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++;
                            }
                        }
                        cont = 0;
                        break;
                    case "+":
                        if (ArrayER.Length == i)
                        {
                            ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                            capacidadArrayInfija++;
                        }
                        else
                        {
                            if (i + 1 < ArrayER.Length)
                            {
                                if (ArrayER[i + 1].ToString() == ")" || ArrayER[i + 1].ToString() == "]" || ArrayER[i + 1].ToString() == "|" || ArrayER[i + 1].ToString() == "*" || ArrayER[i + 1].ToString() == "+" || ArrayER[i + 1].ToString() == "?")
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                }
                                else
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                    ArrayInfija[capacidadArrayInfija] = "&";
                                    capacidadArrayInfija++;
                                }
                            }
                            else
                            {
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++;
                            }
                        }
                        cont = 0;
                        break;
                        cont = 0;
                        break;
                    default:
                        if (tipo == 1)
                        {
                            if (cont == 0)
                            {
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++;
                                cont++;
                            }
                            else
                            {
                                ArrayInfija[capacidadArrayInfija] = "&";
                                capacidadArrayInfija++; //aumenta por el or |
                                ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                capacidadArrayInfija++; //aumenta por el dato que entra
                            }
                        }
                        else
                        {
                            if (tipo == 2)
                            {
                                if (cont == 0)
                                {
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++;
                                    cont++;
                                }
                                else
                                {
                                    ArrayInfija[capacidadArrayInfija] = "|";
                                    capacidadArrayInfija++; //aumenta por el or |
                                    ArrayInfija[capacidadArrayInfija] = ArrayER[i].ToString();
                                    capacidadArrayInfija++; //aumenta por el dato que entra
                                }
                            }
                        }
                        break;
                }
            return posision;
        }
        public void OR(int posision)
        {

        }
        public void ImprimeCadena()
        {
            posfij = "";
            for (int i = 0; i < ArrayInfija.Length; i++)
            {
                posfij += ArrayInfija[i];
            }
            Debug.WriteLine(posfija);
        }
        public int Numero_O_Letra(int indicador)// 1 = numero 0 = letra
        {
            bool band = true;
            string aux3 = ArrayER[indicador].ToString();
            band = int.TryParse(aux3, out _);//Para saber si es un numero o letra
            if (band)
            {
                for (int i = 0; i < 10; i++)
                {
                    int aux = Int32.Parse(ArrayER[indicador].ToString());//conbvertir cadena a entero
                    Debug.WriteLine("ArrayER  " + ArrayER[indicador] + "parce" + aux);
                    if (aux == i)
                    {
                        Debug.WriteLine("i = " + i);
                        return i;
                    }
                }

            }
            else
            {
                for (int j = 0; j < 26; j++)
                {
                    if (ArrayER[indicador].ToString() == alpha[j].ToString()) ;
                    {
                        Debug.WriteLine(ArrayER[indicador] + "   " + alpha[j]);
                        return -1;
                    }
                }
            }
            return 0;
        }

        public void Inserta(int inicio, int final, int posisión)
        {
            for (int i = inicio + 1; i < final + 1; i++)
            {
                ArrayInfija[capacidadArrayInfija] = "|";
                capacidadArrayInfija++; //aumenta por el or |
                ArrayInfija[capacidadArrayInfija++] = i.ToString();
                capacidadArrayInfija++; //aumenta por el or el calor que entra

            }
        }
        public void InsertaLegtra(int inicio, int final)
        {
            bool band = false;
            int i = 0;
            while (i < 26)
            {
                Debug.WriteLine(ArrayER[inicio]);
                if (ArrayER[inicio] == alpha[i])
                    band = true;
                while (band)
                {
                    i++;
                    Debug.WriteLine(ArrayER[final]);
                    imprime();
                    ArrayInfija[capacidadArrayInfija] = "|";
                    capacidadArrayInfija++; //aumenta por el or |
                    ArrayInfija[capacidadArrayInfija] = alpha[i].ToString();
                    capacidadArrayInfija++; //aumenta por el or el calor que entra
                    if (ArrayER[final] == alpha[i])
                        band = false;
                }
                i++;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            OpcionMathER = 2;
            ExpReg.Text = "";//limpiar
            textBox2.Text = "";//limpiar
            ExpReg.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
            label5.Text = "Expresión Regular";
            textBox3.Clear();
            textBox4.Clear();
            tablaAFN.Rows.Clear();
            tablaAFN.Columns.Clear();
            tablaAFD.Rows.Clear();
            tablaAFD.Columns.Clear();

        }

        private void button2_Click(object sender, EventArgs e)
        {

            OpcionMathER = 1;
            ExpReg.Enabled = true;
            ExpReg.Text = "";
            label5.Text = "Expresión Matematica";

        }

        /*
         * Método para realizar el AFN
        */
        private void construirAFN_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox1.Text = "";

            tablaAFN.Rows.Clear();
            tablaAFN.Columns.Clear();

            DataGridViewTextBoxColumn columnaInicial = new DataGridViewTextBoxColumn();
            columnaInicial.HeaderText = "Estados";
            tablaAFN.Columns.Add(columnaInicial);

            int CuentaEstEp = 0;
            int CuentaCol = 0;

            List<char> encabezados = new List<char>();
            List<char> repeticion = new List<char>();
            AFN_Generador generator = new AFN_Generador();
            afn = generator.Evalua_Posfija(posfija);
            generator.Enumera_Automata(afn.Inicio);
            afn.Desmarca_Visitado(afn.Inicio);
            generator.Genera_Lista(afn.Inicio);
            afn.Desmarca_Visitado(afn.Inicio);
            lista_adyacencia = generator.Lista_Adyacencia;
            for (int i = 0; i < posfija.Count; i++)
            {
                if (posfija[i] != '*' && posfija[i] != '?' && posfija[i] != '+' && posfija[i] != '&' && posfija[i] != '|')
                {
                    CuentaCol++;
                    encabezados.Add(posfija[i]);
                }
            }
            CuentaCol++;
            encabezados.Add('ε');

            for (int i = 0; i < encabezados.Count; i++)
            {
                for (int j = 0; j < encabezados.Count; j++)
                {
                    if (encabezados[i] == encabezados[j] && i != j && encabezados[i] != '*' && encabezados[i] != '?' && encabezados[i] != '+' && encabezados[i] != '&' && encabezados[i] != '|')
                    {
                        encabezados.RemoveAt(i);
                        CuentaCol--;
                    }
                }
            }


            //agrega las filas a la tabla
            for (int i = 0; i < lista_adyacencia.Count; i++)
            {
                tablaAFN.Rows.Add();
                tablaAFN.Rows[i].Cells[0].Value = i;
            }

            //agrega las columnas a la tabla
            for (int i = 0; i < CuentaCol; i++)
            {
                DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
                columna.HeaderText = encabezados[i].ToString();
                tablaAFN.Columns.Add(columna);
            }

            //recorrido para llenar la tabla
            for (int i = 0; i < lista_adyacencia.Count; i++)
            {
                if (lista_adyacencia[i].Count != 0)
                {
                    for (int j = 0; j < lista_adyacencia[i].Count; j++)
                    {
                        for (int k = 0; k < CuentaCol; k++)
                        {
                            if (j == 0)
                                tablaAFN.Rows[i].Cells[k + 1].Value = '{';
                            else
                            {
                                tablaAFN.Rows[i].Cells[k + 1].Value = tablaAFN.Rows[i].Cells[k + 1].Value.ToString() + ',';
                            }
                            if (encabezados[k].ToString() == lista_adyacencia[i][j].Name)
                            {
                                tablaAFN.Rows[i].Cells[k + 1].Value = tablaAFN.Rows[i].Cells[k + 1].Value.ToString() + lista_adyacencia[i][j].Destino.Name.ToString();
                                if (encabezados[k].ToString() == "ε")
                                    CuentaEstEp++;
                            }
                            else
                            {
                                tablaAFN.Rows[i].Cells[k + 1].Value = 'Ø';
                            }
                            if (j == lista_adyacencia[i].Count - 1 && tablaAFN.Rows[i].Cells[k + 1].Value.ToString() != "Ø")
                                tablaAFN.Rows[i].Cells[k + 1].Value = tablaAFN.Rows[i].Cells[k + 1].Value.ToString() + '}';
                        }
                    }
                }
                else
                {
                    for (int l = 0; l < CuentaCol; l++)
                    {
                        tablaAFN.Rows[i].Cells[l + 1].Value = 'Ø';
                    }
                }
            }
            alfabeto = encabezados;
            textBox3.Text = textBox3.Text + lista_adyacencia.Count.ToString();
            textBox1.Text = textBox1.Text + CuentaEstEp.ToString();
        }

        private void construirAFD_Click(object sender, EventArgs e)
        {
            AFD(0, relacionesAFD);
        }

        public void AFD(int num, List<List<string>> relacionesAFD)
        {
            List<char> encabezados = new List<char>();

            int Nnodos = 0;
            List<Nodo> NodosRes = new List<Nodo>();

            if (num == 0)
            {
                textBox4.Text = "";

                tablaAFD.Rows.Clear();
                tablaAFD.Columns.Clear();


                DataGridViewTextBoxColumn columnaInicialAFD = new DataGridViewTextBoxColumn();
                columnaInicialAFD.HeaderText = "Estados";
                tablaAFD.Columns.Add(columnaInicialAFD);

            }

            AFD a = new AFD(lista_adyacencia, alfabeto);
            a.Construccion_de_Subconjuntos(afn, ref Nnodos, ref NodosRes, ref EdosAcepta);
            //a.Imprime_relaciones();



            //guarda los valores para el encabezado
            for (int i = 0; i < posfija.Count; i++)
            {
                if (posfija[i] != '*' && posfija[i] != '?' && posfija[i] != '+' && posfija[i] != '&' && posfija[i] != '|')
                {
                    encabezados.Add(posfija[i]);
                }
            }
            repetidos = 0;
            //elimina valores repetidos en el encabezado
            for (int i = 0; i < encabezados.Count; i++)
            {
                for (int j = 0; j < encabezados.Count; j++)
                {
                    if (encabezados[i] == encabezados[j] && i != j && encabezados[i] != '*' && encabezados[i] != '?' && encabezados[i] != '+' && encabezados[i] != '&' && encabezados[i] != '|')
                    {
                        encabezados.RemoveAt(i);
                        repetidos++;
                    }
                }
            }

            if (num == 0)
            {
                //agrega las filas a la tabla
                for (int i = 0; i < NodosRes.Count; i++)
                {
                    tablaAFD.Rows.Add();
                    tablaAFD.Rows[i].Cells[0].Value = ConvierteNumero(i);
                }

                //agrega las columnas a la tabla
                for (int i = 0; i < encabezados.Count; i++)
                {
                    DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
                    columna.HeaderText = encabezados[i].ToString();
                    tablaAFD.Columns.Add(columna);
                }
            }

            relacionesAFD.Clear();
            auxiliar.Clear();

            //inicializa la lista donde se guardan las relaciones            
            for (int i = 0; i < NodosRes.Count; i++)
            {
                auxiliar = new List<string>();
                for (int j = 0; j < NodosRes.Count; j++)
                {
                    auxiliar.Add("-1");
                }
                relacionesAFD.Add(auxiliar);
            }

            //manda llenar la lista con las relaciones
            for (int i = 0; i < NodosRes.Count; i++)
            {
                NodosRes[i].Llena_Relaciones(ref relacionesAFD, i);
            }

            NumEmcaGlob = encabezados.Count;

            if (num == 0)
            {
                //comienza a rellenar la tabla
                for (int i = 0; i < NodosRes.Count; i++)
                {
                    for (int j = 0; j < NodosRes.Count; j++)
                    {
                        for (int k = 0; k < encabezados.Count; k++)
                        {
                            if (relacionesAFD[i][j] == encabezados[k].ToString())
                            {
                                tablaAFD.Rows[i].Cells[k + 1].Value = ConvierteNumero(j);
                            }
                        }
                        for (int k = 0; k < encabezados.Count; k++)
                        {
                            if (tablaAFD.Rows[i].Cells[k + 1].Value == null)
                            {
                                tablaAFD.Rows[i].Cells[k + 1].Value = 'Ø';
                            }
                        }
                    }
                }
                textBox4.Text = textBox4.Text + Nnodos.ToString();
            }
        }

        public void imprime()
        {
            for (int i = 0; i < 15; i++)
            {
                Debug.WriteLine(ArrayInfija[i]);
            }
        }

        private void btnValidar_Click(object sender, EventArgs e)
        {
            //Recibe la cadena 
            char[] cadena = txtCadena.Text.ToCharArray();
            int resLex = 0, Estado = 0;
            Lexema lexema = new Lexema();

            AFD(1, relacionesAFD);
            //                            char[] cadena, List<List<string>> relacionesAFD, ref int busca, int pos, int numenca, int repetidos
            resLex = lexema.VerificaLexema(cadena, relacionesAFD, ref Estado, 0, NumEmcaGlob, repetidos);

            lbResLexema.ForeColor = System.Drawing.Color.Red;
            if (resLex == 1)
            {
                for (int k = 0; k < EdosAcepta.Count; k++)
                {
                    if (Estado == EdosAcepta[k])
                    {
                        lbResLexema.ForeColor = System.Drawing.Color.Green;
                        lbResLexema.Text = "Sí pertenece al lenguaje de la ER";
                        break;
                    }
                    else
                        if (k == EdosAcepta.Count - 1)
                        lbResLexema.Text = "No pertenece al lenguaje de la ER";
                }
            }
            else
            {
                lbResLexema.Text = "No pertenece al lenguaje de la ER";
            }
        }

        private void btnClasTokens_Click(object sender, EventArgs e)
        {
            int banderaerror = 0;
            Clas_Tokens(ref banderaerror);
            //generaTablaTokens(tokens, nombreLexema);
        }

        private bool DFS(Estadoaux actual, string lexema, int indx)
        {
            bool test = true;
            if (lexema.Length > indx)
            {
                if (actual.dtransiciones.Count > 0)
                {
                    foreach (var t in actual.dtransiciones)
                    {
                        if (lexema[indx] == t.Simbolo)
                        {
                            indx++;
                            test = DFS(t.next, lexema, indx);
                            break;
                        }
                        else
                        {
                            test = false;
                        }
                    }
                }
                else
                {
                    test = false;
                }
            }
            else
            {
                if (actual.tipo != 2)
                {
                    test = false;
                }
                else
                    test = true;
            }


            return test;
        }


        private void Token(object sender, EventArgs e)
        {
            if (checkDataInputed())
            {
                List<string> reservedWords = createReservedWords();
                List<string> specialSymbols = createSpecialSymbols();

                string letterPostFija = postF.Convierte_Posfija(txtLetter.Text);
                string digitPostfija = postF.Convierte_Posfija(txtDigit.Text);

                AFN letterAFN = new AFN(letterPostFija);
                AFN digitAFN = new AFN(digitPostfija);

                letterAFN.generaAFN();
                digitAFN.generaAFN();

                AFD letterAFD = new AFD(letterAFN);
                AFD digitAFD = new AFD(digitAFN);

                letterAFD.generaAFD();
                digitAFD.generaAFD();

                List<string> tokens = entradaLenguaje.Text.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> nombreLexema = new List<string>();

                foreach (string token in tokens)
                {
                    if (reservedWords.Contains(token))
                        nombreLexema.Add(token);
                    else if (specialSymbols.Contains(token))
                        nombreLexema.Add(token);
                    else if (DFS(letterAFD.destados.First(), token, 0))
                        nombreLexema.Add("letter");
                    else if (DFS(digitAFD.destados.First(), token, 0))
                        nombreLexema.Add("digit");
                    else
                        nombreLexema.Add("Error léxico");
                }

                generaTablaTokens(tokens, nombreLexema);
                TokensLexema = nombreLexema;
                TokensText = tokens;
            }
        }

        private void generaTablaTokens(List<string> tokens, List<string> nombreLexema)
        {
            tokensTabla.Rows.Clear();
            for (int i = 0; i < tokens.Count; i++)
            {
                tokensTabla.Rows.Add(tokens[i], nombreLexema[i]);
                if (nombreLexema[i] == "Error léxico")
                {
                    tokensTabla.Rows[i].Cells[0].Style.ForeColor = Color.Red;
                    tokensTabla.Rows[i].Cells[1].Style.ForeColor = Color.Red;
                }
            }
        }

        private bool checkDataInputed()
        {
            bool flag = false;

            if (txtLetter.Text == "" || txtDigit.Text == "" || entradaLenguaje.Text == "")
                flag = false;
            else
                flag = true;

            return flag;
        }

        private List<string> createReservedWords()
        {
            List<string> reservedWords = new List<string>();

            string[] collection = new string[] { "select", "from", "where", "insert", "into", "values", "update", "set", "delete" };
            reservedWords.AddRange(collection);

            return reservedWords;
        }

        private List<string> createSpecialSymbols()
        {
            List<string> specialSymbols = new List<string>();

            string[] collection = new string[] { "*", "=", ",", "<=", "(", ")", ">=", "<", ">" };
            specialSymbols.AddRange(collection);

            return specialSymbols;
        }






















        private void GraficaLista(List<List<string>> res)
        {   /*
            res_Tokens.Rows.Clear();
            res_Tokens.Columns.Clear();

            DataGridViewTextBoxColumn columnaNomTok = new DataGridViewTextBoxColumn();
            columnaNomTok.HeaderText = "Nombre";
            res_Tokens.Columns.Add(columnaNomTok);

            DataGridViewTextBoxColumn columnaLexTok = new DataGridViewTextBoxColumn();
            columnaLexTok.HeaderText = "Lexema";
            res_Tokens.Columns.Add(columnaLexTok);

            for (int i = 0; i < res.Count; i++)
            {
                res_Tokens.Rows.Add();
                for (int j = 0; j < res[i].Count; j++)
                {
                    res_Tokens.Rows[i].Cells[j].Value = res[i][j].ToString();
                }
            }*/

        }

        public void Clas_Tokens(ref int be)
        {   /*
            string id = txtLetter.Text;
            string num = txtDigit.Text;
            List<char> lId;
            List<char> lNum;
            //Creamos las expresiones postfijas de ambas expresiones regulares (identificador, numero)
            Expresion_Regular erId = new Expresion_Regular();
            Expresion_Regular erNum = new Expresion_Regular();
            lId = erId.Recibo(id);
            lNum = erNum.Recibo(num);
            List<char> cId, cNum;
            cId = erId.Concatena(lId);
            cNum = erNum.Concatena(lNum);
            List<char> posfijaId = erId.Postfija(cId);
            List<char> posfijaNumero = erNum.Postfija(cNum);

            //Se construyen los respectivos AFN
            AFN_Generador generator = new AFN_Generador();
            AFN afnId = new AFN();
            afnId = generator.Evalua_Posfija(posfijaId);
            generator.Enumera_Automata(afnId.Inicio);
            afnId.Desmarca_Visitado(afnId.Inicio);
            generator.Genera_Lista(afnId.Inicio);
            afnId.Desmarca_Visitado(afnId.Inicio);
            List<List<Arista>> ad = generator.Lista_Adyacencia;



            AFN_Generador generatorNum = new AFN_Generador();
            AFN afnNum = new AFN();
            afnNum = generatorNum.Evalua_Posfija(posfijaNumero);
            generatorNum.Enumera_Automata(afnNum.Inicio);
            afnNum.Desmarca_Visitado(afnNum.Inicio);
            generatorNum.Genera_Lista(afnNum.Inicio);
            afnNum.Desmarca_Visitado(afnNum.Inicio);


            //Generamos los respectivos AFD
            AFD afdId = new AFD(generator.Lista_Adyacencia, getAlfabeto(posfijaId));
            AFD afdNum = new AFD(generatorNum.Lista_Adyacencia, getAlfabeto(posfijaNumero));

            int n = 0;
            List<Nodo> relacionesId = new List<Nodo>();
            List<Nodo> relacionesNum = new List<Nodo>();

            List<Nodo> nod = new List<Nodo>();
            List<int> list = new List<int>();


            afdId.Construccion_de_Subconjuntos(afnId, ref n, ref nod, ref list);
            relacionesId = afdId.Nodos;
            // afdId.Imprime_relaciones();



            int n_num = 0;
            List<Nodo> nodNum = new List<Nodo>();
            List<int> edo_aceptacion = new List<int>();
            afdNum.Construccion_de_Subconjuntos(afnNum, ref n_num, ref nodNum, ref edo_aceptacion);
            relacionesNum = afdNum.Nodos;
            // afdNum.Imprime_relaciones();


            //Se divide palabra por palabra para la posterior clasificaión
            List<List<char>> nombres;
            Clasifica_Tokens t = new Clasifica_Tokens(afdId, afdNum);
            nombres = t.Divide_Cadena(entrada_lenguaje.Text);
            //Resultado de la clasificación de tokens
            resultado = t.Clasificar_Tokens(nombres, afdId, afdNum, list, edo_aceptacion);
            //ImprimeLista(resultado);
            GraficaLista(resultado);



            //identificamos errores léxicos
            List<int> num_lineas = t.Num_linea;
            /*consola.Text = "";
            for (int i = 0; i < resultado.Count; i++)
            {
                if (resultado[i][0] == "Error Léxico")
                {
                    consola.Text += "Linea " + num_lineas[i] + ": "
                                    + resultado[i][1] + " " + "no se reconoce." + "\r\n";
                    be = 1;
                }
            }*/
        }

        private List<char> getAlfabeto(List<char> posfija)
        {
            List<char> encabezados = new List<char>();
            //guarda los valores para el alfabeto
            for (int i = 0; i < posfija.Count; i++)
            {
                if (posfija[i] != '*' && posfija[i] != '?' && posfija[i] != '+' && posfija[i] != '&' && posfija[i] != '|')
                {
                    encabezados.Add(posfija[i]);
                }
            }
            int repetidos = 0;
            //elimina valores repetidos en el encabezado
            for (int i = 0; i < encabezados.Count; i++)
            {
                for (int j = 0; j < encabezados.Count; j++)
                {
                    if (encabezados[i] == encabezados[j] && i != j && encabezados[i] != '*' && encabezados[i] != '?' && encabezados[i] != '+' && encabezados[i] != '&' && encabezados[i] != '|')
                    {
                        encabezados.RemoveAt(i);
                        repetidos++;
                    }
                }
            }
            encabezados.Add('ε');
            return encabezados;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        /*
        private void button4_Click(object sender, EventArgs e)
        {
          

            TablaLR0.DataSource = null;
            TablaLR0.Rows.Clear();
            //List<String> transiciones = Lista.Estados2.SimbolosGramaticales;
            transiciones.ForEach(t => TablaLR0.Columns.Add(t, t));
           
            foreach (Estados estados in Lista.Estados2.Estados)
            {
                textBox5.Text += "l-" + estados.estadoID + " =\r\n" + "{\r\n" +  estados.getEstado() + "\r\n" + "}\r\n";
                List<String> elementos = new List<String> { "l-" + estados.estadoID.ToString() };
                foreach (String s in transiciones)
                {
                    LRT? transicion = estados.getTransicion(s);
                    if (transicion == null)
                        elementos.Add("");
                    else
                        elementos.Add(transicion.Id.ToString());
                }
                TablaLR0.Rows.Add(elementos.ToArray());
            }
            if (GramSQL.Count == 0)
            {
                LLenaSQL();
                LLenaListas();
                CreaGramNueva();
                LR0();
               //MuestraConectesSQL();
                creaTAS();
                RecorridoTAS();
                MuestraTAS();
            }

        }

        */
        public void LLenaSQL()
        {

            FilasGT = new List<string>();
            FilasGT.Add("A");
            FilasGT.Add("B");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("B");
            FilasGT.Add("CB");
            FilasGT.Add(" ");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("C");
            FilasGT.Add("D");
            FilasGT.Add("N");
            FilasGT.Add("Q");
            FilasGT.Add("T");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("D");
            FilasGT.Add("aEbHJ");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("E");
            FilasGT.Add("*");
            FilasGT.Add("F");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("F");
            FilasGT.Add("IG");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("G");
            FilasGT.Add(",IG");
            FilasGT.Add(" ");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("H");
            FilasGT.Add("U");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("I");
            FilasGT.Add("U");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("J");
            FilasGT.Add("cK");
            FilasGT.Add(" ");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("K");
            FilasGT.Add("ILM");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("L");
            FilasGT.Add("=");
            FilasGT.Add("<>");
            FilasGT.Add("<");
            FilasGT.Add(">");
            FilasGT.Add("<=");
            FilasGT.Add(">=");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("M");
            FilasGT.Add("V");
            FilasGT.Add("U");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("N");
            FilasGT.Add("deH(F)f(O)");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("O");
            FilasGT.Add("MP");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("P");
            FilasGT.Add(",MP");
            FilasGT.Add(" ");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("Q");
            FilasGT.Add("gHhRcK");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("R");
            FilasGT.Add("I=MS");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("S");
            FilasGT.Add(",I=MS");
            FilasGT.Add(" ");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("T");
            FilasGT.Add("ibHcK");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("U");
            FilasGT.Add("j");
            GramSQL.Add(FilasGT);

            FilasGT = new List<string>();
            FilasGT.Add("V");
            FilasGT.Add("k");
            GramSQL.Add(FilasGT);


        }

        public void LLenaListas()
        {
            PalRes = new List<string>();
            PalRes.Add("select");
            PalRes.Add("from");
            PalRes.Add("where");
            PalRes.Add("insert");
            PalRes.Add("into");
            PalRes.Add("values");
            PalRes.Add("update");
            PalRes.Add("set");
            PalRes.Add("delete");
            PalRes.Add("letter");
            PalRes.Add("digit");



            SimbEsp = new List<string>();
            SimbEsp.Add("*");
            SimbEsp.Add(",");
            SimbEsp.Add("=");
            SimbEsp.Add("<>");
            SimbEsp.Add("<");
            SimbEsp.Add(">");
            SimbEsp.Add("<=");
            SimbEsp.Add(">=");
            SimbEsp.Add("(");
            SimbEsp.Add(")");


            //Lista de no terminales
            NoTerminales = new List<string>();
            NoTerminales.Add("A'"); //programa
            NoTerminales.Add("A"); //programa
            NoTerminales.Add("B"); //statement-list
            NoTerminales.Add("C"); //statement
            NoTerminales.Add("D"); //select-statement
            NoTerminales.Add("E"); //select-list
            NoTerminales.Add("F"); //column-list
            NoTerminales.Add("G"); //column-tail
            NoTerminales.Add("H"); //table-name
            NoTerminales.Add("I"); //column-name
            NoTerminales.Add("J"); //where-clause
            NoTerminales.Add("K"); //condition
            NoTerminales.Add("L"); //operator
            NoTerminales.Add("M"); //value
            NoTerminales.Add("N"); //insert-statement
            NoTerminales.Add("O"); //value-list
            NoTerminales.Add("P"); //value-tail
            NoTerminales.Add("Q"); //update-statement
            NoTerminales.Add("R"); //update-list 
            NoTerminales.Add("S"); //update-tail 
            NoTerminales.Add("T"); //delete-statement
            NoTerminales.Add("U"); //identifier
            NoTerminales.Add("V"); //digit

            //Terminales
            Terminales = new List<string>();
            Terminales.Add("a"); //select
            Terminales.Add("b"); //from
            Terminales.Add("c"); //where
            Terminales.Add("d"); //insert 
            Terminales.Add("e"); //into
            Terminales.Add("f"); //values
            Terminales.Add("j"); //update
            Terminales.Add("h"); //set
            Terminales.Add("i"); //delete
            Terminales.Add("j"); //letter
            Terminales.Add("k"); //digit

            Terminales.Add("*");
            Terminales.Add(",");
            Terminales.Add("=");
            Terminales.Add("l"); //l
            Terminales.Add("<");
            Terminales.Add(">");
            Terminales.Add("m"); //m
            Terminales.Add("n"); //n
            Terminales.Add("(");
            Terminales.Add(")");


            //Siguientes
            SigAux = new List<string>();
            SigAux.Add("$");//program'
            Siguientes.Add(SigAux);

            SigAux = new List<string>();
            SigAux.Add("$");//program
            Siguientes.Add(SigAux);

            SigAux = new List<string>();
            SigAux.Add("$");//statement-list
            Siguientes.Add(SigAux);

            SigAux = new List<string>();
            SigAux.Add("$");//statement
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            Siguientes.Add(SigAux);

            SigAux = new List<string>();
            SigAux.Add("$");//select-statement
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            Siguientes.Add(SigAux);

            SigAux = new List<string>(); //select-list
            SigAux.Add("b"); //from
            Siguientes.Add(SigAux);

            SigAux = new List<string>(); //column-list 
            SigAux.Add("b");//from
            SigAux.Add(")");
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//column-tail
            SigAux.Add("b");//from
            SigAux.Add(")");
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//table-name
            SigAux.Add("c");//where
            SigAux.Add("$");
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            SigAux.Add("(");//update
            SigAux.Add("h");//delete
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//column-name
            SigAux.Add(",");//column-name
            SigAux.Add("b");
            SigAux.Add("=");
            SigAux.Add("l"); //<>
            SigAux.Add("<");
            SigAux.Add(">");
            SigAux.Add("m"); //<=
            SigAux.Add("n"); //>=
            SigAux.Add(")");
            Siguientes.Add(SigAux);

            SigAux = new List<string>(); //where-clause
            SigAux.Add("$");
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//condition
            SigAux.Add("$");
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//dele
            Siguientes.Add(SigAux); ;

            SigAux = new List<string>();//operator
            SigAux.Add("k"); //digit
            SigAux.Add("j"); //letter
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//value
            SigAux.Add("$");
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            SigAux.Add(",");
            SigAux.Add(")");
            SigAux.Add("c");//where
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//insert-statement
            SigAux.Add("$");
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            Siguientes.Add(SigAux);

            SigAux = new List<string>(); //value-list
            SigAux.Add(")");
            Siguientes.Add(SigAux);

            SigAux = new List<string>(); //value-tail
            SigAux.Add(")");
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//update-statement
            SigAux.Add("$");
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//update-list
            SigAux.Add("c");//where
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//update-tail
            SigAux.Add("c");//where
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//delete-statement
            SigAux.Add("$");
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//identifier
            SigAux.Add("c");
            SigAux.Add("$");
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            SigAux.Add(",");
            SigAux.Add("b");//from
            SigAux.Add("(");
            SigAux.Add("h");//set
            SigAux.Add("=");
            SigAux.Add("l"); //l <>
            SigAux.Add("<");
            SigAux.Add(">");
            SigAux.Add("m"); //m <=
            SigAux.Add("n"); //n >=
            SigAux.Add(")");
            Siguientes.Add(SigAux);

            SigAux = new List<string>();//number
            SigAux.Add("$");
            SigAux.Add("a");//select
            SigAux.Add("d");//insert
            SigAux.Add("g");//update
            SigAux.Add("i");//delete
            SigAux.Add(",");
            SigAux.Add(")");
            SigAux.Add("c");//where
            Siguientes.Add(SigAux);


        }


        public void creaTAS()
        {
            auxTAS = new List<string>();
            auxTAS.Add("-1");
            for (int i = 0; i < Terminales.Count; i++)
            {
                auxTAS.Add(Terminales[i]);
            }
            auxTAS.Add("$");
            for (int i = 0; i < NoTerminales.Count; i++)
            {
                auxTAS.Add(NoTerminales[i]);
            }
            TAS.Add(auxTAS);
            for (int i = 0; i < In.Count; i++)
            {
                auxTAS = new List<string>();
                auxTAS.Add(i.ToString());
                for (int j = 0; j < Terminales.Count + NoTerminales.Count + 1; j++)
                {
                    auxTAS.Add(" ");
                }
                TAS.Add(auxTAS);
            }
        }

        public void MuestraConectesSQL()
        {
            TablaLR0.Rows.Clear();
            TablaLR0.Columns.Clear();

            DataGridViewTextBoxColumn columnaInicial = new DataGridViewTextBoxColumn();
            columnaInicial.HeaderText = "Estados";
            TablaLR0.Columns.Add(columnaInicial);

            //agrega las filas a la tabla
            for (int i = 0; i < In.Count; i++)
            {
                TablaLR0.Rows.Add();
                TablaLR0.Rows[i].Cells[0].Value = "I" + i;
            }

            //agrega las columnas a la tabla
            for (int i = 0; i < Terminales.Count; i++)
            {
                DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
                columna.HeaderText = CambiaSQL(Terminales[i].ToString());
                TablaLR0.Columns.Add(columna);
            }
            for (int i = 0; i < NoTerminales.Count; i++)
            {
                DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
                columna.HeaderText = CambiaSQL(NoTerminales[i].ToString());
                TablaLR0.Columns.Add(columna);
            }

            for (int i = 0; i < Conecte.Count; i++)
            {
                for (int j = 0; j < Terminales.Count; j++)
                {
                    if (Conecte[i][1] == Terminales[j])
                    {
                        TablaLR0.Rows[Int32.Parse(Conecte[i][0])].Cells[j + 1].Value = "I" + Int32.Parse(Conecte[i][2]).ToString();
                    }
                }
                for (int j = 0; j < NoTerminales.Count; j++)
                {
                    if (Conecte[i][1] == NoTerminales[j])
                    {
                        TablaLR0.Rows[Int32.Parse(Conecte[i][0])].Cells[j + Terminales.Count + 1].Value = "I" + Int32.Parse(Conecte[i][2]).ToString();
                    }
                }
            }

        }

        public void MuestraTAS()
        {
            TablaAS.Rows.Clear();
            TablaAS.Columns.Clear();

            for (int i = 0; i < TAS[0].Count; i++)
            //for (int i = 0; i < 70; i++)
            {
                DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
                if (i == 0)
                    columna.HeaderText = "Estados";
                else
                    //columna.HeaderText = CambiaSQL(TAS[0][i]);
                    columna.HeaderText = CambiaSQL(TAS[0][i]);
                TablaAS.Columns.Add(columna);
            }

            //agrega las filas a la tabla
            //for (int i = 0; i < In.Count; i++)
            for (int i = 0; i < 71; i++)
            {
                TablaAS.Rows.Add();
                TablaAS.Rows[i].Cells[0].Value = i;
            }

            for (int i = 1; i < TAS.Count; i++)
            {
                for (int j = 1; j < TAS[i].Count; j++)
                {
                    TablaAS.Rows[i - 1].Cells[j].Value = TAS[i][j];
                }
            }


        }

        //Crea la gramatica aumentada
        public void CreaGramNueva()
        {
            FilasGT = new List<string>();
            FilasGT.Add(GramSQL[0][0] + "'");
            FilasGT.Add(GramSQL[0][0]);
            GramSQL.Insert(0, FilasGT);
        }

        public void LR0()
        {
            //se le agrega el . a la producción
            string auxGT = "." + GramSQL[0][1];

            /*INICIA GUARDANDO LA PRIMERA PRODUCCION EN J*/
            FilasJ = new List<string>();
            FilasJ.Add(GramSQL[0][0]);
            FilasJ.Add(auxGT);  //producción con punto
            List<List<string>> Jn = new List<List<string>>();
            Jn.Add(FilasJ);
            Console.WriteLine(FilasJ);
            for (int i = 0; i < Jn.Count; i++)
            {
                for (int j = 1; j < Jn[i].Count; j++)
                {
                    Cerradura(Jn, Jn[i][0], Jn[i][j]);
                }
            }
            In.Add(Jn);

            for (int o = 0; o < In.Count; o++)
            {
                for (int i = 0; i < Terminales.Count; i++)
                {
                    ir_a(In[o], Terminales[i], o);
                }
                for (int i = 0; i < NoTerminales.Count; i++)
                {
                    ir_a(In[o], NoTerminales[i], o);
                }
            }

            //I.Text = I.Text + "contenido de In " + Environment.NewLine;
            for (int i = 0; i < In.Count; i++)
            {
                returnIrA.Text = returnIrA.Text + "I" + i + "  " + Environment.NewLine;
                for (int j = 0; j < In[i].Count; j++)
                {
                    for (int k = 0; k < In[i][j].Count; k++)
                    {
                        char[] aux = In[i][j][k].ToCharArray();
                        for (int k2 = 0; k2 < aux.Length; k2++)
                            returnIrA.Text = returnIrA.Text + CambiaSQL(aux[k2].ToString()) + " ";
                        if (k == 0)
                            returnIrA.Text = returnIrA.Text + "-> ";
                    }
                    returnIrA.Text = returnIrA.Text + Environment.NewLine;
                }
                returnIrA.Text = returnIrA.Text + Environment.NewLine;
            }

        }

        public string CambiaSQL(string letra)
        {
            string cambio = letra;

            if (letra == "A")
            {
                cambio = "program";
            }
            else if (letra == "B")
            {
                cambio = "statement-list";
            }
            else if (letra == "C")
            {
                cambio = "statement";
            }
            else if (letra == "D")
            {
                cambio = "select-statement";
            }
            else if (letra == "E")
            {
                cambio = "select-list";
            }
            else if (letra == "F")
            {
                cambio = "column-list";
            }
            else if (letra == "G")
            {
                cambio = "column-tail";
            }
            else if (letra == "H")
            {
                cambio = "table-name";
            }
            else if (letra == "I")
            {
                cambio = "column-name";
            }
            else if (letra == "J")
            {
                cambio = "where-clause";
            }
            else if (letra == "K")
            {
                cambio = "condition";
            }
            else if (letra == "L")
            {
                cambio = "operator";
            }
            else if (letra == "M")
            {
                cambio = "value";
            }
            else if (letra == "N")
            {
                cambio = "insert-statement";
            }
            else if (letra == "O")
            {
                cambio = "value-list";
            }
            else if (letra == "P")
            {
                cambio = "value-tail";
            }
            else if (letra == "Q")
            {
                cambio = "update-statement";
            }
            else if (letra == "R")
            {
                cambio = "update-list";
            }
            else if (letra == "S")
            {
                cambio = "update-tail";
            }
            else if (letra == "T")
            {
                cambio = "delete-statement";
            }
            else if (letra == "U")
            {
                cambio = "identifier";
            }
            else if (letra == "V")
            {
                cambio = "number";
            }
            else if (letra == "a")
            {
                cambio = "select";
            }
            else if (letra == "b")
            {
                cambio = "from";
            }
            else if (letra == "c")
            {
                cambio = "where";
            }
            else if (letra == "d")
            {
                cambio = "insert";
            }
            else if (letra == "e")
            {
                cambio = "into";
            }
            else if (letra == "f")
            {
                cambio = "values";
            }
            else if (letra == "g")
            {
                cambio = "update";
            }
            else if (letra == "h")
            {
                cambio = "set";
            }
            else if (letra == "i")
            {
                cambio = "delete";
            }
            else if (letra == "j")
            {
                cambio = "letter";
            }
            else if (letra == "k")
            {
                cambio = "digit";
            }
            else if (letra == "*")
            {
                cambio = "*";
            }
            else if (letra == ",")
            {
                cambio = ",";
            }
            else if (letra == "=")
            {
                cambio = "=";
            }
            else if (letra == "l")
            {
                cambio = "<>";
            }
            else if (letra == "<")
            {
                cambio = "<";
            }
            else if (letra == ">")
            {
                cambio = ">";
            }
            else if (letra == "m")
            {
                cambio = "<=";
            }
            else if (letra == "n")
            {
                cambio = ">=";
            }
            else if (letra == "(")
            {
                cambio = "(";
            }
            else if (letra == ")")
            {
                cambio = ")";
            }


            return cambio;
        }

        //nombre de la produccion  //símbolo con punto = producto
        public void Cerradura(List<List<string>> Jn, string nombre, string producto)
        {

            /*CONVIERTE EN CHAR EL STRING DEL PRODUCTO PARA PODER RECORRERLO*/
            char[] aux = producto.ToCharArray();
            char sig = ' ';
            List<int> numNT = new List<int>();
            Console.WriteLine(aux);

            /*RECORRE EL CHAR HASTA ENCONTRAR UN PUNTO, AL ENCONTRARLO GUARDA EL SIGUIENTE CARACTER*/
            for (int i = 0; i < aux.Length; i++)
            {
                if (aux[i] == '.' && i + 1 < aux.Length)
                {
                    sig = aux[i + 1];
                    i = aux.Length;
                }
            }

            /*REVISA QUE LO QUE SE ENCONTRO DESPUES DEL PUNTO SEA UN NO TERMINAL*/
            int banderaNoTer = 0;
            for (int i = 0; i < NoTerminales.Count; i++)
            {
                if (sig.ToString() == NoTerminales[i])
                {
                    banderaNoTer = 1;
                }
            }

            if (banderaNoTer == 1)
            {
                /*GUARDA LAS POSICIONES QUE ENCONTRO EN LA GRAMATICA DEL NO TERMINAL GUARDADO ANTES*/
                numNT = new List<int>();
                for (int j = 0; j < GramSQL.Count; j++)
                {
                    if (GramSQL[j][0] == sig.ToString())
                    {
                        numNT.Add(j);
                    }
                }

                /*AGREGA EL PUNTO AL INICIO DE TODAS LAS PRODUCCIONES DEL NO TERMINAL Y LAS GUARDA EN J*/
                int k = 0;
                for (int l = 0; l < numNT.Count; l++)
                {
                    for (int m = 1; m < GramSQL[numNT[k]].Count; m++)
                    {
                        string auxInsPunt = "." + GramSQL[numNT[k]][m];
                        FilasJ = new List<string>();
                        FilasJ.Add(GramSQL[numNT[k]][0]);
                        FilasJ.Add(auxInsPunt);
                        /*FUNCION PARA SABER SI LA PRODUCCION YA SE ENCUENTRA EN J, 1 SI YA ESTABA, 0 SI NO Y LO AGREGA*/
                        int Bandera = RevisaJ(Jn, GramSQL[numNT[k]][0], auxInsPunt);
                        if (Bandera == 0)
                            Jn.Add(FilasJ);
                    }
                    k++;
                }

            }
        }

        public int RevisaJ(List<List<string>> Jn, string Nombre, string Producto)
        {
            int Bandera = 0;
            for (int i = 0; i < Jn.Count; i++)
            {
                for (int j = 0; j < Jn[i].Count; j++)
                {
                    if (Jn[i][0] == Nombre && Jn[i][j] == Producto)
                    {
                        Bandera = 1;
                    }
                }
            }
            return Bandera;
        }

        public void ir_a(List<List<string>> Ina, string x, int o)
        {
            List<List<string>> Jn = new List<List<string>>();
            int entro = 0;
            for (int k = 0; k < Ina.Count; k++)
            {
                for (int l = 1; l < Ina[k].Count; l++)
                {
                    char[] aux = Ina[k][l].ToCharArray();
                    /*RECORRE EL PUNTO UNA POSICION A LA DERECHA*/
                    for (int m = 0; m < aux.Length; m++)
                    {
                        if (aux[m] == '.')
                        {
                            if (m + 1 < aux.Length)
                            {
                                if (aux[m + 1].ToString() == x)
                                {
                                    char recorrepunto;
                                    recorrepunto = aux[m + 1];
                                    aux[m + 1] = aux[m];
                                    aux[m] = recorrepunto;

                                    string straux = "";
                                    for (int i = 0; i < aux.Length; i++)
                                    {
                                        straux = straux + aux[i].ToString();
                                    }

                                    FilasJ = new List<string>();
                                    FilasJ.Add(Ina[k][0]);
                                    FilasJ.Add(straux);
                                    Jn.Add(FilasJ);

                                    for (int i = 0; i < Jn.Count; i++)
                                    {
                                        for (int j = 1; j < Jn[i].Count; j++)
                                        {
                                            Cerradura(Jn, Jn[i][0], Jn[i][j]);
                                        }
                                    }
                                    entro = 1;
                                }
                            }
                        }
                    }
                }
            }

            int Bandera; /*BANDERA QUE REVISA SI EL J CREADO YA EXISTE, 0 NO EXISTE Y SE AGREGA, 1 YA EXISTE Y NO SE AGREGA*/
            if (entro == 1)
            {
                Bandera = RevisaI(Jn, x, o);
                if (Bandera == 0)
                {
                    In.Add(Jn);
                    auxConecte = new List<string>();
                    auxConecte.Add(o.ToString());
                    auxConecte.Add(x);
                    int confin = In.Count - 1;
                    auxConecte.Add(confin.ToString());
                    Conecte.Add(auxConecte);
                }
            }
        }

        public int RevisaI(List<List<string>> Jn, string x, int o)
        {
            int BanderaI = 0;
            int BanderaT = 0;
            int total = 0;

            for (int i = 0; i < In.Count; i++)
            {
                if (Jn.Count == In[i].Count)
                {
                    for (int j = 0; j < In[i].Count; j++)
                    {
                        for (int k = 0; k < In[i][j].Count; k++)
                        {
                            if (In[i][j][k] == Jn[j][k])
                            {
                                BanderaI++;
                            }
                            total++;
                        }
                    }
                    if (BanderaI == total)
                    {
                        BanderaT = 1;
                        auxConecte = new List<string>();
                        auxConecte.Add(o.ToString());
                        auxConecte.Add(x);
                        int confin = i;
                        auxConecte.Add(confin.ToString());
                        Conecte.Add(auxConecte);
                    }
                    else
                    {
                        BanderaI = 0;
                        total = 0;
                    }
                }
            }
            return BanderaT;
        }

        public void RecorridoTAS()
        {

            for (int i = 0; i < In.Count; i++)
            {
                for (int j = 0; j < In[i].Count; j++)
                {
                    for (int k = 1; k < In[i][j].Count; k++)
                    {
                        char[] aux = In[i][j][k].ToCharArray();
                        for (int k2 = 0; k2 < aux.Length; k2++)
                        {
                            if (aux[k2] == '.' && k2 + 1 < aux.Length)
                            {
                                for (int k3 = 0; k3 < Terminales.Count; k3++)
                                {
                                    if (aux[k2 + 1].ToString() == Terminales[k3])
                                    {
                                        //entra en el inciso a, desplazar
                                        Accion(i.ToString(), aux[k2 + 1].ToString());
                                    }
                                }
                            }
                            char[] GramAu = In[i][j][0].ToCharArray();
                            if (aux[k2] == '.' && k2 + 1 >= aux.Length && GramAu[GramAu.Length - 1].ToString() != "'")
                            {
                                //entra en el inciso b, reduccion
                                string SinPunto = In[i][j][k].Replace(".", string.Empty);
                                incisoB(In[i][j][0], SinPunto, i);
                            }
                            if (aux[k2] == '.' && k2 + 1 >= aux.Length && GramAu[GramAu.Length - 1].ToString() == "'")
                            {
                                //entra en el inciso c, aceptacion
                                for (int k4 = 1; k4 < TAS[0].Count; k4++)
                                {
                                    if (TAS[0][k4] == "$")
                                    {
                                        TAS[i + 1][k4] = "ac";
                                    }
                                }
                            }

                        }
                    }
                }
            }

            for (int i = 0; i < Conecte.Count; i++)
            {
                for (int j = 0; j < NoTerminales.Count; j++)
                {
                    if (Conecte[i][1] == NoTerminales[j])
                    {
                        TAS[Int32.Parse(Conecte[i][0]) + 1][Terminales.Count + 1 + j + 1] = Conecte[i][2];
                    }
                }
            }

        }

        public void Accion(string origen, string term)
        {
            string destino = " ";
            for (int i = 0; i < Conecte.Count; i++)
            {
                if (Conecte[i][0] == origen && Conecte[i][1] == term)
                {
                    destino = Conecte[i][2];
                }
            }

            for (int k4 = 1; k4 < TAS[0].Count; k4++)
            {
                if (TAS[0][k4] == term)
                {
                    TAS[Int32.Parse(origen) + 1][k4] = "d" + destino;
                }
            }
        }

        public void incisoB(string A, string produce, int edo)
        {
            int NumDeProd = 0, vueltas = -1; ;
            for (int i = 0; i < GramSQL.Count; i++)
            {
                for (int j = 1; j < GramSQL[i].Count; j++)
                {
                    vueltas++;
                    if (GramSQL[i][0] == A && GramSQL[i][j] == produce)
                    {
                        NumDeProd = vueltas;
                    }
                }
            }

            for (int i = 0; i < Siguientes.Count; i++)
            {
                if (Siguientes[i][0] == A)
                {
                    for (int j = 1; j < Siguientes[i].Count; j++)
                    {
                        for (int k = 1; k < TAS[0].Count; k++)
                        {
                            if (TAS[0][k] == Siguientes[i][j])
                            {
                                TAS[edo + 1][k] = "r" + NumDeProd.ToString();
                            }
                        }
                    }
                }

            }
        }

        private void btnAnalisisLyS_Click(object sender, EventArgs e)
        {
            // Form2 form2 = new Form2();

            List<string> tokens = TokensLexema;
            List<Tree> treeTokens = new List<Tree>();
            //form2.TreeV.Nodes.Clear();
            treeView1.Nodes.Clear();
            txtErrores.Text = "";
            txtresFinal.Text = "";
            if (g == null)
            {
                btnLR0_Click(null, null);
            }
            foreach (string token in tokens)
            {
                treeTokens.Add(new Tree(token));
            }
            tokens.Add("$");
            int aindx = 0;
            int treeindx = -1;
            int s;
            int t;
            string a = tokens[aindx];
            int acell = g.T.IndexOf(a) + 1; //indice de la celda a escribir
            Stack<int> Pila = new Stack<int>();
            Pila.Push(0);
            string aux;
            Tree newFather, casoEpsilon;
            while (true)
            {
                s = Pila.Peek();
                if (a == "$")
                    acell = 22;
                else
                    acell = g.T.IndexOf(a) + 1;
                if (TablaAccion[acell, s].Value != null)
                {
                    aux = TablaAccion[acell, s].Value.ToString();
                    if (aux.StartsWith("d"))
                    {
                        t = int.Parse(aux.Substring(1));
                        Pila.Push(t);
                        aindx++;
                        a = tokens[aindx];
                        treeindx++;
                    }
                    else
                    {
                        if (aux.StartsWith("r"))
                        {
                            t = int.Parse(aux.Substring(1)) - 1;
                            Produce_a p = g.producciones[t];
                            foreach (var token in p.ListTokens)
                            {
                                Pila.Pop();
                            }
                            t = Pila.Peek();
                            Conjuntos left = g.conjuntos.FirstOrDefault(PrimSig => PrimSig.NoTerminal == p.izq);
                            s = g.conjuntos.IndexOf(left);
                            Pila.Push(int.Parse(TablaIrA[s, t].Value.ToString()));
                            //p.left se agrega al arbol? enviar de salida A -> B que basicamente es p
                            if (p.ListTokens.Count > 0)
                            {
                                int k = 0;
                                foreach (var token in p.ListTokens)
                                {
                                    if (k == 0)
                                    {
                                        newFather = new Tree(p.izq);
                                        newFather.hijos.Add(treeTokens[treeindx]);
                                        treeTokens[treeindx] = newFather;
                                    }
                                    else
                                    {
                                        treeTokens[treeindx].hijos.Add(treeTokens[treeindx - 1]);
                                        treeTokens.RemoveAt(treeindx - 1);
                                        treeindx--;
                                    }
                                    k++;
                                }
                            }
                            else
                            {
                                //Caso epsilon
                                treeindx++;
                                casoEpsilon = new Tree("");
                                treeTokens.Insert(treeindx, casoEpsilon);
                                newFather = new Tree(p.izq);
                                newFather.hijos.Add(treeTokens[treeindx]);
                                treeTokens[treeindx] = newFather;
                            }
                        }
                        else
                        {
                            if (TablaAccion[acell, s].Value.ToString().Equals("ac"))
                            {
                                txtErrores.ForeColor = System.Drawing.Color.Green;
                                txtresFinal.ForeColor = System.Drawing.Color.Green;
                                txtErrores.Text = "Correcto";
                                txtresFinal.Text = "Correcto";
                                break;
                            }
                            else
                            {
                                string errtext = "";
                                int i = 0;
                                List<string> text = entradaLenguaje.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                int line = 0;
                                foreach (var token in TokensLexema)
                                {
                                    if (token == "Error léxico")
                                    {
                                        foreach (var te in text)
                                        {
                                            if (te.Contains(TokensText[i]))
                                            { line = text.IndexOf(te) + 1; break; }
                                        }

                                        txtErrores.ForeColor = System.Drawing.Color.Red;
                                        txtresFinal.ForeColor = System.Drawing.Color.Red;
                                        errtext += "Línea " + line.ToString() + ". " + TokensText[i] + " no se reconoce.\r\n ";

                                        Console.WriteLine(errtext);
                                    }
                                    i++;
                                }

                                txtErrores.Text = errtext;
                                txtresFinal.Text = "Se encontró uno o mas errores en el programa.";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    string errores = "";
                    int i = 0;
                    List<string> text = entradaLenguaje.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<string> linetxt = new List<string>();
                    int line = 1;
                    foreach (var te in text)
                    {
                        linetxt = te.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var l in linetxt)
                        {
                            if (a.Equals(l))
                            {
                                if (i == aindx)
                                    errores += "Error en la linea " + line.ToString() + " con el token : \"" + TokensText[i] + "\" , el token no cumple con el análisis sintáctico.\n";
                            }
                            i++;
                        }
                        line++;
                    }
                    errores += "Error, el token no cumple con el análisis sintáctico.";
                    txtErrores.Text = errores;
                    break;
                }
            }
            //Imprimir TreeView
            muestraArbolSintactico(null, treeTokens.First());
            //form2.Show();
        }






        public void muestraArbolSintactico(TreeNode print, Tree actual)
        {
            string simbolo = actual.simbolo;
            if (simbolo == "")
                simbolo = "ε";
            if (print == null)
            {
                print = new TreeNode(simbolo);
                actual.hijos.Reverse();
                foreach (Tree son in actual.hijos)
                {
                    muestraArbolSintactico(print, son);
                }
                treeView1.Nodes.Add(print);
                treeView1.ExpandAll();
            }
            else
            {
                TreeNode nodoActual = new TreeNode(simbolo);
                actual.hijos.Reverse();
                foreach (Tree son in actual.hijos)
                {
                    muestraArbolSintactico(nodoActual, son);
                }
                print.Nodes.Add(nodoActual);
            }
        }

        private void btnLR0_Click(object sender, EventArgs e)
        {
            if (GramSQL.Count == 0)
            {
                LLenaSQL();
                LLenaListas();
                CreaGramNueva();
                LR0();
                MuestraConectesSQL();
                creaTAS();
                RecorridoTAS();
                MuestraTAS();
            }
            List<string> reservedWords = createReservedWords();
            List<string> specialSymbols = createSpecialSymbols();

            string letterPostFija = postF.Convierte_Posfija("[a-z]+");
            string digitPostfija = postF.Convierte_Posfija("[0-9]+");

            AFN letterAFN = new AFN(letterPostFija);
            AFN digitAFN = new AFN(digitPostfija);

            letterAFN.generaAFN();
            digitAFN.generaAFN();

            AFD letterAFD = new AFD(letterAFN);
            AFD digitAFD = new AFD(digitAFN);

            letterAFD.generaAFD();
            digitAFD.generaAFD();

            //Aun no se usan letter y digit de igual manera. (Rev 6)
            g = new SQL(letterAFD, digitAFD);

            creaAutomataLR();
            tablaAnalisisSintacticoLR();
        }


        public void tablaAnalisisSintacticoLR()
        {
            int i = 0;
            int dIndx = 0;
            string writer = "";
            Conjuntos prim;
            Produce_a aAlfa;
            int aAlfaID;
            foreach (var I in g.automataLR)
            {
                TablaAccion.Rows.Add(new DataGridViewRow());
                TablaIrA.Rows.Add(new DataGridViewRow());
                TablaAccion[0, i].Value = i;
                TablaIrA[0, i].Value = i;
                //Paso 2.
                //inciso a si [A->alfa .a beta] esta en I e ir_A = Ij, Accion[i, a] = dj
                //inciso b si [A -> alfa.] esta en I, Accion[i, a] a "reducir A -> alfa" para toda a en SIGUIENTE(A)
                foreach (var prod in I.producciones)
                {
                    if (prod.Indx < prod.ListTokens.Count)
                    {
                        //inciso a
                        foreach (var t in I.transiciones)
                        {
                            if (t.Simbolo.Equals(prod.getDotB()))
                            {
                                if (g.checkTerminal(t.Simbolo))
                                {
                                    dIndx = g.T.IndexOf(t.Simbolo) + 1; //indice de la celda a escribir.
                                    writer = "d" + t.idEdoDes.ToString(); //Basicamente, ir_A() = Ij
                                    TablaAccion[dIndx, i].Value = writer;
                                }
                            }
                        }
                    }
                    else
                    {

                        if (prod.Indx == prod.ListTokens.Count && prod.izq != "program'")
                        {
                            //Inciso b
                            prim = g.conjuntos.FirstOrDefault(PrimSig => PrimSig.NoTerminal == prod.izq);
                            aAlfa = encuenrtraProduccion(prod);
                            aAlfaID = encuenrtraProduccionId(prod) + 1;
                            foreach (var s in prim.Siguiente)
                            {
                                if (s != "$")
                                {
                                    dIndx = g.T.IndexOf(s) + 1; //indice de la celda a escribir.
                                    writer = "r" + aAlfaID.ToString();
                                    TablaAccion[dIndx, i].Value = writer;
                                }
                                else
                                {
                                    writer = "r" + aAlfaID.ToString();
                                    TablaAccion[22, i].Value = writer;
                                }
                            }
                        }
                        else
                        {
                            //inciso c Si [S' -> S.] esta en Ii, entonces establecer Accion[i, $] a "aceptar"
                            if (prod.Indx == prod.ListTokens.Count && prod.izq.Equals("program'"))
                            {
                                TablaAccion[22, i].Value = "ac";
                            }
                        }
                    }
                }
                //Paso 3
                foreach (var t in I.transiciones)
                {
                    if (!g.checkTerminal(t.Simbolo))
                    {
                        dIndx = g.X.IndexOf(t.Simbolo) + 1;
                        TablaIrA[dIndx, i].Value = t.idEdoDes;
                    }
                }
                i++;
            }
        }

        public int encuenrtraProduccionId(Produce_a Ip)
        {
            int i = -1;
            foreach (Produce_a pe in g.producciones)
            {
                i++;
                if (pe.izq.Equals(Ip.izq) && pe.der.Equals(Ip.der))
                {
                    return i;
                }
            }

            return i;
        }
        public Produce_a encuenrtraProduccion(Produce_a Ip)
        {
            Produce_a p = null;

            foreach (Produce_a pe in g.producciones)
            {
                if (pe.izq.Equals(Ip.izq) && pe.der.Equals(Ip.der))
                {
                    return pe;
                }
            }
              
            return p;
        }

        public List<Produce_a> ir_A(string X, Tokens I)
        {
            Produce_a res = null;
            String checkX = "";
            List<Produce_a> conjunto = new List<Produce_a>();
            foreach (var p in I.producciones)
            {
                checkX = p.getDotB();
                if (X.Equals(checkX))
                {
                    res = getProductionData(p);
                    res.Indx++;
                    conjunto.Add(res);
                }
            }
            conjunto = CerraduraLR(conjunto);


            return conjunto;
        }

        public List<Produce_a> CerraduraLR(Tokens I)
        {
            List<Produce_a> J = new List<Produce_a>();
            J.AddRange(I.producciones);
            string bProd = "";

            for (int i = 0; i < J.Count; i++)
            {
                var A = J[i];
                //Obtenemos B de A -> a.Bb
                bProd = A.getDotB();
                if (bProd != "")
                {
                    foreach (var B in g.producciones)
                    {
                        if (bProd == B.izq && !J.Contains(B))
                        {
                            J.Add(B);
                        }
                    }
                }
            }


            return J;
        }

        public List<Produce_a> CerraduraLR(List<Produce_a> I)
        {
            List<Produce_a> J = new List<Produce_a>();
            J.AddRange(I);
            string bProd = "";

            for (int i = 0; i < J.Count; i++)
            {
                var A = J[i];
                //Obtenemos B de A -> a.Bb
                bProd = A.getDotB();
                if (bProd != "")
                {
                    foreach (var B in g.producciones)
                    {
                        if (bProd == B.izq && !JcontieneB(B, J))
                        {
                            J.Add(B);
                        }
                    }
                }
            }

            return J;
        }


        public Produce_a getProductionData(Produce_a p)
        {
            Produce_a nuevo = new Produce_a(p.izq, p.der);
            nuevo.Indx = p.Indx;
            return nuevo;
        }


        public void creaAutomataLR()
        {
            //Primera Cerradura con program' -> .program
            Produce_a programaPrime = new Produce_a("program'", "program");
            Tokens estadoZero = new Tokens();
            estadoZero.producciones.Add(programaPrime);
            List<Produce_a> I0 = CerraduraLR(estadoZero);
            estadoZero.producciones = I0;
            printEstadoZero(estadoZero);
            g.automataLR.Add(estadoZero);
            List<Produce_a> irAux = new List<Produce_a>();
            Tokens print;
            Tokens newI;
            int l;
            for (int i = 0; i < g.automataLR.Count; i++)
            {
                var I = g.automataLR[i];
                if (i == 70)
                    l = 0;
                foreach (var sg in g.X)
                {
                    if (sg == ",")
                        l = 1;
                    irAux = ir_A(sg, I);
                    if (irAux.Count > 0)
                    {
                        if (!checkIEstadoExists(irAux))
                        {
                            newI = new Tokens();
                            newI.producciones = irAux;
                            g.automataLR.Add(newI);
                            printNewEstado(newI, I, sg);
                            I.transiciones.Add(new TransConEstado(sg, newI));
                        }
                        else
                        {
                            print = getIEstado(irAux);
                            if (print != null)
                            {
                                printOldEstado(irAux, I, sg, print);
                                I.transiciones.Add(new TransConEstado(sg, print));
                            }
                        }
                    }
                }
            }
        }


        public Tokens? getIEstado(List<Produce_a> p)
        {
            foreach (var I in g.automataLR)
            {
                if (compruebaIEstado(I.producciones, p))
                {
                    return I;
                }
            }

            return null;
        }


        public bool checkIEstadoExists(List<Produce_a> p)
        {
            bool exists = false;

            foreach (var I in g.automataLR)
            {
                if (compruebaIEstado(I.producciones, p))
                { exists = true; break; }
            }

            return exists;
        }

        public bool compruebaIEstado(List<Produce_a> I, List<Produce_a> p)
        {
            bool found = false;
            if (I.Count == p.Count)
            {
                foreach (var pp in p)
                {
                    if (JcontieneB(pp, I))
                    {
                        found = true;
                    }
                    else
                    {
                        found = false;
                        break;
                    }
                }
            }

            return found;
        }

        public bool JcontieneB(Produce_a B, List<Produce_a> J)
        {
            bool found = false;

            foreach (var A in J)
            {
                if (B.izq.Equals(A.izq) && B.der.Equals(A.der) && B.Indx == A.Indx)
                {
                    return true;
                }
            }

            return found;
        }

        public void printOldEstado(List<Produce_a> added, Tokens origin, string token, Tokens existing)
        {
            string ir_A = "Ir_a(" + origin.id.ToString() + ", " + token + ")";
            string elemento2 = "{ " + added.First().izq + " -> ";
            for (int i = 0; i < added.First().ListTokens.Count; i++)
            {
                if (added.First().Indx == i)
                {
                    elemento2 += ".";
                }
                elemento2 += " " + added.First().ListTokens[i];
                if (!added.First().ListTokens.Last().Equals(added.First().ListTokens[i]))
                {
                    ;
                }
            }
            if (added.First().Indx >= added.First().ListTokens.Count)
                elemento2 += ".";
            elemento2 += "}";
            LRGrid.Rows.Add(ir_A, elemento2, existing.id.ToString(), "");
        }

        public void printNewEstado(Tokens added, Tokens origin, string token)
        {
            string ir_A = "Ir_a(" + origin.id.ToString() + ", " + token + ")";
            string elemento2 = "{ " + added.producciones.First().izq + " -> ";
            for (int i = 0; i < added.producciones.First().ListTokens.Count; i++)
            {
                if (added.producciones.First().Indx == i)
                {
                    elemento2 += ".";
                }
                elemento2 += added.producciones.First().ListTokens[i];
                if (!added.producciones.First().ListTokens.Last().Equals(added.producciones.First().ListTokens[i]))
                    elemento2 += " ";
            }
            if (added.producciones.First().Indx == added.producciones.First().ListTokens.Count)
                elemento2 += ".";
            elemento2 += "}";
            string producciones = "{";
            foreach (var p in added.producciones)
            {
                producciones += p.izq + " -> ";
                if (p.ListTokens.Count > 0)
                {
                    for (int i = 0; i < p.ListTokens.Count; i++)
                    {
                        if (p.Indx == i)
                        {
                            producciones += ".";
                        }
                        producciones += p.ListTokens[i];
                        if (!p.ListTokens.Last().Equals(p.ListTokens[i]))
                            producciones += " ";
                    }
                    if (p.ListTokens.Count >= p.Indx)
                        producciones += ".";
                    if (!added.producciones.Last().Equals(p))
                        producciones += "; ";
                }
            }
            producciones += "}";
            LRGrid.Rows.Add(ir_A, elemento2, added.id.ToString(), producciones);
        }

        public void printEstadoZero(Tokens Zero)
        {
            string elemento2 = "{ " + Zero.producciones[0].izq + " -> " + "." + Zero.producciones[0].Derecha + "}";
            string producciones = "{";
            foreach (var p in Zero.producciones)
            {
                producciones += p.izq + " -> ";
                if (p.ListTokens.Count > 0)
                {
                    for (int i = 0; i < p.ListTokens.Count; i++)
                    {
                        if (p.Indx == i)
                        {
                            producciones += ".";
                        }
                        producciones += p.ListTokens[i];
                        if (!p.ListTokens.Last().Equals(p.ListTokens[i]))
                            producciones += " ";
                    }
                    if (p.ListTokens.Count >= p.Indx)
                        producciones += ".";
                    if (!Zero.producciones.Last().Equals(p))
                        producciones += "; ";
                }
            }
            producciones += "}";
            LRGrid.Rows.Add("", elemento2, Zero.id.ToString(), producciones);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }


        /** -------------- Función -------------**/
        /*Muestra el treeview
         * apunta al tope de la pila
         *  La función regresa un CORRECTO si encuentra un AC en la tabla Análisis sintactico
         *  La función un que encontró un ereror
        */
        public void muestra_Arbol()
        {
      
            int line = 0;
            string l;
            string d;
            //1. Obtener w y la cadena para hacer el arbol 
            List<string> w = new List<string>();
            List<string> waux = new List<string>();
            //Declarar una lista de arbol sintactico
            List<TreeNode> arbol = new List<TreeNode>();
            
            w.Add("$");
            //2. Crear pila
            Stack pila = new Stack();
            pila.Push(0);
            int s, t;
            
            int i = 0;
            int i2 = -1;
            string a = w.ElementAt(i);
            int numr;
            string aux;
            while (true)
            {
                s = (int)pila.Peek();

                string acc= "ac";
                    if (acc != "" && !acc.Equals("error"))
                {
                    if (acc.Contains("D"))
                    {
                        Match m = Regex.Match(acc, "(\\d+)");

                        if (m.Success)
                        {
                            numr = Int32.Parse(m.Value);
                            pila.Push(numr);
                            i++;
                            i2++;
                            a = w.ElementAt(i);

                        }
                    }
                    else
                    {
                        if (acc.Contains("R"))
                        {
                            Match m = Regex.Match(acc, "(\\d+)");

                            if (m.Success)
                            {
                                numr = Int32.Parse(m.Value);

                                int sacar = Int32.Parse(m.Value);
                                for (int j = 0; j < sacar; j++)
                                {
                                    pila.Pop();
                                }
                                t = (int)pila.Peek();
                                
                                
                                pila.Push(numr);


                                // parte del arbol
                                i2 = i2 - sacar + 1;
                                arbolaux = arbol.GetRange(i2, sacar);
                                arbol.RemoveRange(i2, sacar);
                                arbol.Insert(i2, new TreeNode(a1, arbolaux.ToArray()));
                            }
                        }
                        else
                        {
                            if (acc.Equals("AC"))
                            {
                                treeView1.Nodes.Clear();
                                treeView1.BeginUpdate();
                                treeView1.Nodes.AddRange(arbol.ToArray());
                                treeView1.EndUpdate();
                                treeView1.ExpandAll();
                                break;
                            }
                        }

                    }



                }
                else
                {
                    txtErrores.ForeColor = System.Drawing.Color.Red;
                    txtresFinal.ForeColor = System.Drawing.Color.Red;
                    txtErrores.Text += "Línea " + linea + ". " + NOMToken + " no se reconoce.\r\n ";
                    txtresFinal.Text = "Se encontró uno o mas errores en el programa.";
                    break;
                }
            }

        }


        public string ConvierteNumero(int letra)
        {
            string Num = "A";

            if (letra == 0)
            {
                Num = "A";
            }
            else if (letra == 1)
            {
                Num = "B";
            }
            else if (letra == 2)
            {
                Num = "C";
            }
            else if (letra == 3)
            {
                Num = "D";
            }
            else if (letra == 4)
            {
                Num = "E";
            }
            else if (letra == 5)
            {
                Num = "F";
            }
            else if (letra == 6)
            {
                Num = "G";
            }
            else if (letra == 7)
            {
                Num = "H";
            }
            else if (letra == 8)
            {
                Num = "I";
            }
            else if (letra == 9)
            {
                Num = "J";
            }
            else if (letra == 10)
            {
                Num = "K";
            }
            else if (letra == 11)
            {
                Num = "L";
            }
            else if (letra == 12)
            {
                Num = "M";
            }
            else if (letra == 13)
            {
                Num = "N";
            }
            else if (letra == 14)
            {
                Num = "O";
            }
            else if (letra == 15)
            {
                Num = "P";
            }
            else if (letra == 16)
            {
                Num = "Q";
            }
            else if (letra == 17)
            {
                Num = "R";
            }
            else if (letra == 18)
            {
                Num = "S";
            }
            else if (letra == 19)
            {
                Num = "T";
            }
            else if (letra == 20)
            {
                Num = "U";
            }
            else if (letra == 21)
            {
                Num = "V";
            }
            else if (letra == 22)
            {
                Num = "W";
            }
            else if (letra == 23)
            {
                Num = "X";
            }
            else if (letra == 24)
            {
                Num = "Y";
            }
            else if (letra == 25)
            {
                Num = "Z";
            }
            else if (letra == 26)
            {
                Num = "AA";
            }
            else if (letra == 27)
            {
                Num = "AB";
            }
            else if (letra == 28)
            {
                Num = "AC";
            }
            else if (letra == 29)
            {
                Num = "AD";
            }
            else if (letra == 30)
            {
                Num = "AE";
            }
            else if (letra == 31)
            {
                Num = "AF";
            }
            else if (letra == 32)
            {
                Num = "AG";
            }
            else if (letra == 33)
            {
                Num = "AH";
            }
            else if (letra == 34)
            {
                Num = "AI";
            }
            else if (letra == 39)
            {
                Num = "AJ";
            }
            else if (letra == 40)
            {
                Num = "AK";
            }
            else if (letra == 41)
            {
                Num = "AL";
            }
            else if (letra == 42)
            {
                Num = "AM";
            }
            else if (letra == 43)
            {
                Num = "AN";
            }
            else if (letra == 44)
            {
                Num = "AO";
            }
            else if (letra == 45)
            {
                Num = "AP";
            }
            else if (letra == 46)
            {
                Num = "AQ";
            }
            else if (letra == 47)
            {
                Num = "AR";
            }
            else if (letra == 48)
            {
                Num = "AS";
            }
            else if (letra == 49)
            {
                Num = "AT";
            }
            else if (letra == 50)
            {
                Num = "AU";
            }
            else if (letra == 51)
            {
                Num = "AV";
            }
            else if (letra == 52)
            {
                Num = "AW";
            }
            else if (letra == 53)
            {
                Num = "AX";
            }
            else if (letra == 54)
            {
                Num = "AY";
            }
            else if (letra == 55)
            {
                Num = "AZ";
            }

            return Num;

        }





    }
    
    }


