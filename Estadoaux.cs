using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Estadoaux
    {
        //public static char namestatic = 'A';
        public char name;
        public List<Estado> estados;
        public List<Transiciones> dtransiciones = new List<Transiciones>();
        public int tipo { get; set; } // 0 - inicial | 1 - neutro | 2 - aceptacion

        public Estadoaux(char name)
        {
            this.name = name;
        }

        public void generaTransicion(char s, Estadoaux next)
        {
            Transiciones newT = new Transiciones(s, next);
            dtransiciones.Add(newT);
        }

    }
}
