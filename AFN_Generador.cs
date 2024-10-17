using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    internal class AFN_Generador
    {

        private Dictionary<string, int> tipos = new Dictionary<string, int>();
        private int count; //Contador para colocar el número a cada nodo
        private List<List<Arista>> lista_adyacencia;
        public AFN_Generador()
        {
            lista_adyacencia = new List<List<Arista>>();
            tipos = new Dictionary<string, int>();
            tipos.Add("inicio", 1);
            tipos.Add("medio", 2);
            tipos.Add("aceptacion", 3);
            count = 0;
        }

        /*
         * Método que genera el autómata
         * @Param: Recibe la expresión posfija que se obtuvo en el
         *         método, la recorre para verificar que tipo de es
         *         y agregarla a la pila
         * @Return: Regresa el autómata
         */
        public AFN Evalua_Posfija(List<char> posfija)
        {
            AFN a, b;

            Stack<AFN> pila = new Stack<AFN>();

            for (int i = 0; i < posfija.Count; i++)
            {
                if (posfija[i] == '?')
                {
                    a = pila.Pop();
                    Cero_o_una_instancia(a);
                    pila.Push(a);
                }
                else if (posfija[i] == '+')
                {
                    a = pila.Pop();
                    Cerradura_Positiva(a);
                    pila.Push(a);
                }
                else if (posfija[i] == '*')
                {
                    a = pila.Pop();
                    CerraduraKleene(a);
                    pila.Push(a);
                }
                else if (posfija[i] == '&')
                {
                    a = pila.Pop();
                    b = pila.Pop();
                    Concatena(b, a);
                    pila.Push(b);

                }
                else if (posfija[i] == '|')
                {
                    a = pila.Pop();
                    b = pila.Pop();
                    SeleccionAlternativas(b, a);
                    pila.Push(b);
                }
                else
                {
                    AFN ato = new AFN(posfija[i].ToString());

                    pila.Push(ato);
                }
            }

            return (pila.Pop());

        }


        /**
         * Cuando el autómata ya se ha creado en su totalidad, 
         * el método se encarga de enumerar cada uno de los nodos;
         */
        public void Enumera_Automata(Nodo n)
        {
            n.Name = count;
            for (int i = 0; i < n.Relaciones.Count; i++)
            {
                n.Relaciones[i].Origen = n;
            }
            // Console.WriteLine("Nodo " + n.Name.ToString() + " visitado.");
            count++;
            n.Visited = true;
            if (n.Relaciones.Count == 0)
                return;
            for (int i = 0; i < n.Relaciones.Count; i++)
                if (!n.Relaciones[i].Destino.Visited)
                    Enumera_Automata(n.Relaciones[i].Destino);
        }

        /*
         * Imprime autómata
         */
        public void ImprimeAutomata(Nodo a)
        {
            a.Imprime_Relaciones();
            a.Visited = true;
            if (a.Relaciones.Count == 0)
                return;
            for (int i = 0; i < a.Relaciones.Count; i++)
                if (!a.Relaciones[i].Destino.Visited)
                    ImprimeAutomata(a.Relaciones[i].Destino);


        }



        /*
        * Método que genera la matriz de adyacencia de un AFD
        */
        public void Genera_Lista(Nodo n)
        {
            // Console.WriteLine("Nombre del nodo = " + n.Name);
            List<Arista> fila = new List<Arista>();
            n.Visited = true;
            if (n.Relaciones.Count == 0)
            {

                lista_adyacencia.Add(fila);
                return;
            }

            for (int j = 0; j < n.Relaciones.Count; j++)
            {

                fila.Add(n.Relaciones[j]);

            }
            lista_adyacencia.Add(fila);
            for (int i = 0; i < n.Relaciones.Count; i++)
            {

                if ((!n.Relaciones[i].Destino.Visited))
                    Genera_Lista(n.Relaciones[i].Destino);
            }


        }




        /*En la siguiente sección se implementan los metodos necesaros para formar el algoritmo de 
         Thompson
         */

        /*
         * Realiza la acción de concatenación de 2 AFN
         * @Param: Recibe el otro autómata con el que se va a concatenar.
         */
        public void Concatena(AFN a, AFN b)
        {
            a.Aceptacion.Copia_Nodo(b.Inicio);
            a.Aceptacion.Tipo = tipos["medio"];
            a.Aceptacion = b.Aceptacion;
            a.Aceptacion.Tipo = tipos["aceptacion"];
        }

     /*
     * Realiza la selección de alternativas con otro autómata
     * @Param: Recibe el autómata con el cual se realizará la 
     *         selección de alternativas.
     */
        public void SeleccionAlternativas(AFN a, AFN b)
        {

            Nodo auxInicio = new Nodo(tipos["inicio"]);
            Nodo auxAcep = new Nodo(tipos["aceptacion"]);
            Arista a1 = new Arista(auxInicio, a.Inicio, "ε");
            Arista a2 = new Arista(auxInicio, b.Inicio, "ε");
            auxInicio.Relaciones.Add(a1);
            auxInicio.Relaciones.Add(a2);
            a1.Destino.Tipo = tipos["medio"];
            a2.Destino.Tipo = tipos["medio"];
            a.Inicio = auxInicio;
            Arista a3 = new Arista(a.Aceptacion, auxAcep, "ε");
            Arista a4 = new Arista(b.Aceptacion, auxAcep, "ε");
            a.Aceptacion.Relaciones.Add(a3);
            b.Aceptacion.Relaciones.Add(a4);
            a.Aceptacion.Tipo = tipos["medio"];
            b.Aceptacion.Tipo = tipos["medio"];
            a.Aceptacion = auxAcep;

        }


        /*
         * Método que realiza cerradura de Kleene
         * @Param: Recibe el autómata
         */ 
      public void CerraduraKleene(AFN a)
      {

          Nodo auxInicio = new Nodo(tipos["inicio"]);
          Nodo auxAcep = new Nodo(tipos["aceptacion"]);
          Arista a1 = new Arista(auxInicio, a.Inicio, "ε");
          Arista a2 = new Arista(a.Aceptacion, auxAcep, "ε");
          auxInicio.Relaciones.Add(a1);
          a.Aceptacion.Relaciones.Add(a2);
          Arista a3 = new Arista(auxInicio, auxAcep, "ε");
          auxInicio.Relaciones.Add(a3);
          Arista a4 = new Arista(a.Aceptacion, a.Inicio, "ε");
          a.Aceptacion.Relaciones.Add(a4);
          a.Aceptacion.Tipo = tipos["medio"];
          a.Inicio.Tipo = tipos["medio"];
          a.Aceptacion = auxAcep;
          a.Inicio = auxInicio;

      }

     /*
      * Método que realiza la Cerradura Positiva con otro autómata
      * @Param: Recibe el autómata.
      */
       public void Cerradura_Positiva(AFN a)
       {
           Nodo auxInicio = new Nodo(tipos["inicio"]);
           Nodo auxAcep = new Nodo(tipos["aceptacion"]);
           Arista a1 = new Arista(auxInicio, a.Inicio, "ε");
           Arista a2 = new Arista(a.Aceptacion, auxAcep, "ε");
           auxInicio.Relaciones.Add(a1);
           a.Aceptacion.Relaciones.Add(a2);
           Arista a3 = new Arista(a.Aceptacion, a.Inicio, "ε");
           a.Aceptacion.Relaciones.Add(a3);
           a.Aceptacion.Tipo = tipos["medio"];
           a.Inicio.Tipo = tipos["medio"];
           a.Aceptacion = auxAcep;
           a.Inicio = auxInicio;
       }

       /*
       * Cero o una instancia
       */
        public void Cero_o_una_instancia(AFN a)
        {
            Nodo auxInicio = new Nodo(tipos["inicio"]);
            Nodo auxAcep = new Nodo(tipos["aceptacion"]);
            Arista a1 = new Arista(auxInicio, a.Inicio, "ε");
            Arista a2 = new Arista(a.Aceptacion, auxAcep, "ε");
            auxInicio.Relaciones.Add(a1);
            a.Aceptacion.Relaciones.Add(a2);
            Arista a3 = new Arista(auxInicio, auxAcep, "ε");
            auxInicio.Relaciones.Add(a3);
            a.Aceptacion.Tipo = tipos["medio"];
            a.Inicio.Tipo = tipos["medio"];
            a.Aceptacion = auxAcep;
            a.Inicio = auxInicio;
        }


        public int Count
        {
            get { return count; }
            set { count = value; }
        }


        public List<List<Arista>> Lista_Adyacencia
        {
            get { return lista_adyacencia; }
            set { lista_adyacencia = value; }
        }


    }
}

