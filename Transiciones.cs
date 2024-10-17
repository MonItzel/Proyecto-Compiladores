using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Transiciones
    {
        //variables
        private bool visited;
        private string ultEstado;
        
        //Funciones declaradas
        public int numTransicion { get; set; }
        public char Simbolo { get; set; }
        public int numEstado { get; set; }
        public char nameEdoDes { get; set; }

        public Estadoaux next { get; set; }
        //ultimo estado
        public string UltEstado 
        {
            get { return ultEstado; }
            set { ultEstado = value; }
        }
       
        public bool Visited
        {
            get { return visited; }
            set { visited = value; }
        }
        //constructores
        public Transiciones() { }
        public Transiciones(char S, Estadoaux Des)
        {
            numEstado++;
            numTransicion++;
            Simbolo = S;
            nameEdoDes = Des.name;
            next = Des;
        }

    }
}
