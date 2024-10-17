using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class TransConEstado
    {
        //variables
        private bool visitedU;
        private string ultEstado;
        private bool visitedP;
        private string PenultEstado;
        //funciones declaradas
        public int numEstadoTotal { get; set; }
        public int numTransicionTotal { get; set; }
        public string Simbolo { get; set; }
        public int idEdoDes { get; set; }
        public Tokens next { get; set; }
        //ultimo estado
        public string PenUltEstado
        {
            get { return PenultEstado; }
            set { PenultEstado = value; }
        }
        public string UltEstado
        {
            get { return ultEstado; }
            set { ultEstado = value; }
        }

        public bool Visited
        {
            get { return visitedU; }
            set { visitedU = value; }
        }
        public bool VisitedP
        {
            get { return visitedP; }
            set { visitedP = value; }
        }
        //Constructores
        public TransConEstado() { }
        public TransConEstado(string S, Tokens Des)
        {
            numEstadoTotal++;
            numTransicionTotal++;
            Simbolo = S;
            idEdoDes = Des.id;
            next = Des;
        }
    }
}
