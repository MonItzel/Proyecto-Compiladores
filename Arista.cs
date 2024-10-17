using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Arista
    {
        private Nodo origen;

        private Nodo destino;

        private string name;
        public Arista(Nodo origen, Nodo destino, string name)
        {
            this.origen = origen;
            this.destino = destino;
            this.name = name;
        }


        public Nodo Origen
        {
            get { return origen; }
            set { origen = value; }
        }


        public Nodo Destino
        {
            get { return destino; }
            set { destino = value; }
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
