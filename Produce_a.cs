using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Produce_a
    {
        //variables
        public string izq;
        public string izqAux;
        public string der;
        public string derAux;
        public int Indx = 0;
        public List<string> ListTokens = new List<string>();
        public List<string> ListAux = new List<string>();

        //Funciones
        public void InicializaProduce_a()
        {
            izq = "";
            izqAux = "";
            der = "";
            derAux = "";
            Indx = 0;
            ListTokens = new List<string>();
            ListAux = new List<string>();
        }
        public void clearAux()
        {
            izqAux = "";
            derAux = "";
            ListAux = new List<string>();
        }

        //Gets y sets
        public string Izquierda
        {
            get { return izq; }
            set { izq = value; }
        }
        public string Derecha
        {
            get { return der; }
            set { der = value; }
        }
        public List<string> List
        {
            get { return ListTokens; }
            set { ListTokens = value; }
        }
        public string getDotB()
        {
            if (Indx < ListTokens.Count)
            {
                return ListTokens[Indx];
            }

            return "";
        }
        //constructores
        public Produce_a() { }
        public Produce_a(string izquierda, string derecha)
        {
            izqAux = izq;
            derAux = der;
            izq = izquierda;
            der = derecha;
            ListTokens = der.Split(new char[] { ' ', '\t', '\n' }, 
                StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        public Produce_a(bool Elimina, string izquierda, string derecha)
        {
            if (Elimina)
            {
                InicializaProduce_a();
                izq = izquierda;
                der = derecha;
                ListTokens = der.Split(new char[] { ' ', '\t', '\n' },
                    StringSplitOptions.TrimEntries).ToList();
            }
            else
            {
                clearAux();
                izqAux = izquierda;
                derAux = derecha;
                ListTokens = der.Split(new char[] { ' ', '\t', '\n' },
                    StringSplitOptions.None).ToList();
            }
        }
    }
}
