using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Proyecto_Compiladores
{
    public class Tree
    {
        /*Determina el tipo del nodo
         *  1 - Estado inicial
         *  2 - Intermedio
         *  3 - Estado de aceptación
         */
        private int tipo;

        //Variable que lista las relaciones
        private bool Evisited;
        public string arbol;
        public string simbolo;
        public List<Tree> hijos = new List<Tree>();
        public Tree padre;

        //funciones
        public string node { get; set; }
        public int idEdoDes { get; set; }

        //Constructores
        public Tree() { }

        public Tree(string s)
        {
            Evisited = true;
            idEdoDes = 0;
            simbolo = s;
            node = s;
        }
    }
}
