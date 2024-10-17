using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Transicion
    {
        //variables
        private bool visited;
        private string PenultEstado;
        //Funciones declaraadas
        public int numEstado { get; set; }
        public int numTransicion { get; set; }
        public char Simbolo { get; set; }
        public int idEdoDes { get; set; }

        public Estado next { get; set; }
        //penultimo estado
        public string PenUltEstado
        {
            get { return PenultEstado; }
            set { PenultEstado = value; }
        }

        public bool Visited
        {
            get { return visited; }
            set { visited = value; }
        }
        //Constructores
        public Transicion() { }
        public Transicion(char S, Estado Des)
        {
            numEstado++;
            Simbolo = S;
            idEdoDes = Des.id;
            next = Des;
            numTransicion++;
        }
    }
}
