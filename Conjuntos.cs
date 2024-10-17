using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Conjuntos
    {
        //Variables
        public string terminal = "";
        public List<string> Prim = new List<string>();
        public List<string> Sig = new List<string>();
        public string NoTerminal = "";

        //Sets y gets
        public string NOTerminal
        {
            get { return NoTerminal; }
            set { NoTerminal = value; }
        }
        public List<string> Primero
        {
            get { return Prim; }
            set { Prim = value; }
        }
        public List<string> Siguiente
        {
            get { return Sig; }
            set { Sig = value; }
        }

        //Constructores
        public Conjuntos() { }
        public Conjuntos(string Terminal)
        {
           this.terminal = Terminal;
        }
        public Conjuntos(List<string> primeros,string noTerminal, List<string> siguientes)
        {
            Sig = siguientes;
            NoTerminal = noTerminal;
            Prim = primeros;
        }
    }
}
