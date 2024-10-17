using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Estado
    {
        public int id { get; set; }
        public int tipo { get; set; } // 0 - inicial | 1 - neutro | 2 - aceptacion
        public int numtransiciones;
        public int index { get; set; }

        public List<Transicion> transiciones;

        public Estado(int id, int tipo)
        {
            this.id = id;
            this.tipo = tipo;
            transiciones = new List<Transicion>();
            numtransiciones = 0;
        }

        public void changeTipoNeutro()
        {
            tipo = 1;
        }

        public void changeTipoFinal()
        {
            tipo = 2;
        }

        public void changeTipoInicial()
        {
            tipo = 0;
        }

        public void generaTransicion(char s, Estado next)
        {
            Transicion newT = new Transicion(s, next);
            transiciones.Add(newT);
            numtransiciones++;
        }

    }
}
