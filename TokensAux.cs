using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Proyecto_Compiladores
{
    public class TokensAux
    {
        //Variables declaradas
        string simbolo = "";
        int transicionesConE;
        int transicionesT;
        public List<int> auxInt = new List<int>();
        public List<string> auxString = new List<string>();
        public List<Estado> estados = new List<Estado>();
        public List<Estado> auxiliar = new List<Estado>();
        //sets y gets
        public string Simbolos
        {
            get { return simbolo; }
            set { simbolo = value; }
        }
        public int TransicionesConEpsilon
        {
            get { return transicionesConE; }
            set { transicionesConE = value; }
        }
        //Funciones
        public int TransicionesEpsilon()
        {
            int ContTransiciones = 0;

            foreach (Estado estado in estados)
            {
                auxiliar.Add(estado);
                auxString.Add(simbolo);
                foreach (Transicion trancisionaux in estado.transiciones)
                {
                    transicionesConE++;
                    if (trancisionaux.Simbolo == 'ε')
                        ContTransiciones++;
                }
            }
            auxInt.Add(transicionesConE);
            return ContTransiciones;
        }
        public Estado? EstadoID(int idEstado)
        {
            foreach (var estadoAux in estados)
            {
                auxiliar.Add(estadoAux);
                auxString.Add(simbolo);
                if (estadoAux.id == idEstado)
                    return estadoAux;
            }
            return null;
        }
        //constructor
        public TokensAux() { }
        public TokensAux(string simbolo) 
        {
            if (simbolo == "ε")
                transicionesConE++;
            else
                transicionesT++;
            this.simbolo = simbolo;
        }
    }
}
