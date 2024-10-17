using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Nodo
    {
        private int name; //nombre del nodo

        /*Determina el tipo del nodo
         *  1 - Estado inicial
         *  2 - Intermedio
         *  3 - Estado de aceptación
         */
        private int tipo; 
        
        //Variable que lista las relaciones
        private List<Arista> relaciones;
        private bool visited;

        public Nodo(int tipo)
        {
            relaciones = new List<Arista>();
            this.tipo = tipo;
            visited = false;

        }

        public void Imprime_Relaciones()
        {
            Console.Write(this.Name + "- ");
            for (int i = 0; i < this.Relaciones.Count; i++)
            {
                if (this.Relaciones.Count > 0)
                    Console.Write(this.Relaciones[i].Destino.Name + "--" + this.relaciones[i].Name + "-- ");
                else
                    Console.Write(this.Relaciones[i].Destino.Name);
            }
            Console.WriteLine();
        }

        public void Llena_Relaciones(ref List<List<string>> relacionesAFD, int cont)
        {
            for (int i = 0; i < this.Relaciones.Count; i++)
            {
                if (this.Relaciones.Count > 0)
                {
                    //Console.WriteLine(this.Relaciones.Count);
                    relacionesAFD[cont][this.Relaciones[i].Destino.name] = this.relaciones[i].Name;
                    //Console.WriteLine("cuarde " + this.relaciones[i].Name + " en [" + cont + "] [" + this.Relaciones[i].Destino.name + "]");
                }
            }
        }

        public void Copia_Nodo(Nodo n)
        {
            this.tipo = Tipo;
            this.relaciones = n.relaciones;
            this.Visited = n.Visited;

        }

        public int Name
        {
            get { return name; }
            set { this.name = value; }
        }


        public int Tipo
        {
            get { return tipo; }
            set { this.tipo = value; }
        }

        public List<Arista> Relaciones
        {
            get { return relaciones; }
            set { relaciones = value; }

        }

        public bool Visited
        {
            get { return visited; }
            set { visited = value; }
        }


    }
}
