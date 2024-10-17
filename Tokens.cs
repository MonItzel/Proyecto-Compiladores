using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Tokens
    {
        //Variables
        public int id;
        public static int iEstadoID = 0;
        //Listas
        public List<Produce_a> producciones = new List<Produce_a>();
        public List<TransConEstado> transiciones = new List<TransConEstado>();
        //funciones sets y gets
        public List<Produce_a> GetSetProducciones
        {
            get { return producciones; }
            set { producciones = value; }
        }
        public List<TransConEstado> GetSetTransiciones
        {
            get { return transiciones; }
            set { transiciones = value; }
        }
        //Constructores
        public Tokens()
        {
            id = iEstadoID;
            iEstadoID++;
        }
        public Tokens(List<Produce_a> list)
        {
            this.producciones = list;
        }

    }
}
