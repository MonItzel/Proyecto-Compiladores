using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Proyecto_Compiladores
{
    public class AFD
    {
        private Nodo inicio;
        private List<Nodo> aceptacion;
        private List<List<Nodo>> Destados;
        private List<List<Arista>> adyacencias;
        private List<List<Arista>> adyacenciasAFD;
        private List<char> alfabeto;
        private List<Nodo> nodos; //Lista donde se encuntran todos los nodos y sus relaciones
        public List<int> Acepta = new List<int>();
        private int count = 0;
        //Apta = new List<int>;
        //Función que realiza la conversión de un AFN a un AFD

        public AFN Afn;
        public TokensAux afn;
        public List<Estadoaux> destados;

        public AFD(AFN estados)
        {
            this.Afn = estados;
            this.afn = Afn.afnres;
            destados = new List<Estadoaux>();
        }

        
        public void generaAFD()
        {
            List<Estado> lista = new List<Estado>();
            Estado Zero = afn.estados.First();
            lista.Add(Zero);
            CerraduraEpsilon(Zero, lista);
            char name = 'A';
            Estadoaux destadoA = new Estadoaux(name);
            name++;
            destadoA.tipo = 0;
            destadoA.estados = lista;
            destados.Add(destadoA);
            Queue<Estadoaux> filaDestados = new Queue<Estadoaux>();
            filaDestados.Enqueue(destadoA);

            Estadoaux actual, nuevo;
            lista = new List<Estado>();
            while (filaDestados.Count > 0)
            {
                actual = filaDestados.Dequeue();
                foreach (char s in Afn.alfabeto)
                {
                    if (s != 'ε')
                    {
                        lista = mover(actual, s);
                        nuevo = checkDestadoExists(lista);
                        if (nuevo == null)
                        {
                            nuevo = new Estadoaux(name);
                            name++;
                            nuevo.estados = lista;
                            destados.Add(nuevo);
                            filaDestados.Enqueue(nuevo);
                            if (nuevo.estados.Contains(afn.estados.Last()))
                                nuevo.tipo = 2;
                            else
                                nuevo.tipo = 1;
                        }
                        if (lista.Count > 0)
                            actual.generaTransicion(s, nuevo);
                    }
                }
            }
        }

        public AFD(List<List<Arista>> adyacencias, List<char> alfabeto)
        {
            inicio = null;
            aceptacion = null;
            Destados = new List<List<Nodo>>();
            this.adyacencias = adyacencias;
            adyacenciasAFD = new List<List<Arista>>();
            this.alfabeto = alfabeto;
            this.nodos = new List<Nodo>();
        }

        public Estadoaux? checkDestadoExists(List<Estado> CeU)
        {
            bool band = true;
            Estadoaux found = null;

            foreach (Estadoaux de in destados)
            {
                foreach (Estado e in CeU)
                {
                    if (!de.estados.Contains(e) || de.estados.Count != CeU.Count)
                    {
                        band = false;
                        break;
                    }
                    else
                    {
                        band = true;
                    }
                }
                if (band)
                {
                    found = de;
                    break;
                }
            }

            return found;
        }

        public List<Estado> mover(Estadoaux origen, char simbolo)
        {
            List<Estado> U = new List<Estado>();
            List<Estado> CerraduraUnida = new List<Estado>();
            List<Estado> Aux = new List<Estado>();
            List<List<Estado>> Cerraduras = new List<List<Estado>>();

            //Se consigue U, que consta de los Estados que dirigen a una Transicion con simbolo diferente a epsilon.
            foreach (Estado e in origen.estados)
            {
                foreach (Transicion t in e.transiciones)
                {
                    if (t.Simbolo == simbolo)
                    {
                        U.Add(t.next);
                    }
                }
            }
            //Se hace cerradura de epsilon a cada elemento en U
            foreach (Estado e in U)
            {
                Aux.Add(e);
                Aux = CerraduraEpsilon(e, Aux);
                Cerraduras.Add(Aux);
                Aux = new List<Estado>();
            }
            //Union de cerraduras.
            foreach (var ce in Cerraduras)
            {
                //Si es el primer conjunto, se agrega sin evaluar repetidos.
                if (CerraduraUnida.Count == 0)
                    CerraduraUnida.AddRange(ce);
                else
                {
                    //Se comprueban que el estado no este ya en la union.
                    foreach (var t in ce)
                    {
                        if (!CerraduraUnida.Contains(t))
                            CerraduraUnida.Add(t);
                    }
                }
            }

            return CerraduraUnida;
        }

        public List<Estado> CerraduraEpsilon(Estado origen, List<Estado> lista)
        {
            foreach (Transicion t in origen.transiciones)
            {
                if (t.Simbolo == 'ε')
                {
                    if (!lista.Contains(t.next))
                    {
                        lista.Add(t.next);
                        lista = CerraduraEpsilon(t.next, lista);
                    }
                }
            }

            return lista;
        }


        
       

        //Función que va a recorrer la lista de nodos, para ir recorriendo y buscar las transiciones
        //epsilón
        public void Construccion_de_Subconjuntos(AFN a, ref int Nnodos, ref List<Nodo> NodoRes, ref List<int> EdosAcepta)
        {
            List<Nodo> lista = new List<Nodo>();
            a.Cerradura_de_Epsilon(a.Inicio, lista);
            Console.WriteLine();
            inicio = new Nodo(1);
            inicio.Name = count;
            count++;
            nodos.Add(inicio);
            Destados.Add(lista);
            for (int i = 0; i < Destados.Count; i++)
            {
                for (int j = 0; j < alfabeto.Count - 1; j++)
                {
                    Mover(Destados[i], alfabeto[j], a, i);
                }

            }
            MarcaEstadosAceptacion(a);
            Nnodos = nodos.Count;
            NodoRes = nodos;
            ColocaEA(ref EdosAcepta);
        }

        //Método 
        //Función que asigna la letra del alfabeto , al conjunto encontrado y verifica que no este repetido
        //para agregarlo a Destados
        private void Mover(List<Nodo> T, char c, AFN a, int index)
        {
            List<Nodo> list = new List<Nodo>(); /*Lista para guardar los nuevos Destados*/
            List<Nodo> aux = new List<Nodo>();
            for (int i = 0; i < T.Count; i++)
            {
                for (int j = 0; j < adyacencias[T[i].Name].Count; j++)
                {
                    if (adyacencias[T[i].Name][j].Name == c.ToString())
                    {
                        a.Cerradura_de_Epsilon(adyacencias[T[i].Name][j].Destino, aux);
                        list.AddRange(aux);
                    }
                }
            }

            if (list.Count == 0)//No hay relación entre los estados y el caracter del alfabeto
                return;

            aux = list.Distinct().ToList<Nodo>(); //Se quitan los elementos repetidos
            bool isEqual;
            bool band = false;
            int k = 0;
            while (!band && k < Destados.Count)
            {
                isEqual = Enumerable.SequenceEqual(Destados[k].OrderBy(e => e.Name), aux.OrderBy(e => e.Name));
                if (isEqual)
                    band = true;
                else
                    k++;
            }

            if (k == Destados.Count)
            {
                Destados.Add(aux); //Se agrega en Destados
                Nodo n = new Nodo(2);
                n.Name = count;
                count++;
                Arista ar = new Arista(nodos[index], n, c.ToString());
                nodos.Add(n);
                nodos[index].Relaciones.Add(ar);


            }
            else
            {
                Arista ar = new Arista(nodos[index], nodos[k], c.ToString());
                nodos[index].Relaciones.Add(ar);
            }

        }

        //Función que verifica si el estado en el que se encuentra ya ha sido agregado a Destados
        //en caso de que no lo agrega a la lista de Destados
        private void MarcaEstadosAceptacion(AFN a)
        {
            //List<int> Ata = new List<int>();
            for (int i = 0; i < Destados.Count; i++)
            {
                for (int j = 0; j < Destados[i].Count; j++)
                {
                    if (a.Aceptacion.Name == Destados[i][j].Name)
                    {
                        Destados[i][j].Tipo = 3;
                        Acepta.Add(i);
                    }
                }
            }
            for (int i = 0; i < Acepta.Count; i++)
            {
                nodos[Acepta[i]].Tipo = 3;
            }


        }


        //Función que genera la tabla de relaciones
        public void GeneraListaAdyacenciasAfd(Nodo n)
        {
            Console.WriteLine("Nombre del nodo = " + n.Name);
            Console.WriteLine("Cantidad de relaciones " + n.Relaciones.Count);
            List<Arista> fila = new List<Arista>();
            n.Visited = true;
            if (n.Relaciones.Count == 0)
            {

                adyacenciasAFD.Add(fila);
                return;
            }

            for (int j = 0; j < n.Relaciones.Count; j++)
            {

                fila.Add(n.Relaciones[j]);

            }
            adyacenciasAFD.Add(fila);
            for (int i = 0; i < n.Relaciones.Count; i++)
            {

                if ((!n.Relaciones[i].Destino.Visited))
                    GeneraListaAdyacenciasAfd(n.Relaciones[i].Destino);
            }


        }

        
        public void Desmarca_Visitado(Nodo n)
        {
            n.Visited = false;
            if (n.Relaciones.Count == 0)
            {
                return;
            }
            for (int i = 0; i < n.Relaciones.Count; i++)
                if (n.Relaciones[i].Destino.Visited)
                    Desmarca_Visitado(n.Relaciones[i].Destino);

        }

        //Función que muestra las relaciones encontradas 
        public void Imprime_relaciones()
        {
            for (int i = 0; i < nodos.Count; i++)
            {
                for (int j = 0; j < nodos[i].Relaciones.Count; j++)
                {
                    Console.Write(nodos[i].Name + " -- " + nodos[i].Relaciones[j].Name + " --- " +
                                    nodos[i].Relaciones[j].Destino.Name + " ");
                }
                Console.WriteLine();
            }
        }

        
        public void CreaTabla(ref List<List<string>> relacionesAFDLexema, ref int Num)
        {
            List<string> auxiliar = new List<string>();
            for (int i = 0; i < nodos.Count; i++)
            {
                auxiliar = new List<string>();
                for (int j = 0; j < nodos.Count; j++)
                {
                    auxiliar.Add("-1");
                }
                relacionesAFDLexema.Add(auxiliar);
            }
            LlenaTabla(ref relacionesAFDLexema);
            Num = nodos.Count;
        }

        public void LlenaTabla(ref List<List<string>> relacionesAFDLexema)
        {
            for (int i = 0; i < nodos.Count; i++)
            {
                for (int j = 0; j < nodos[i].Relaciones.Count; j++)
                {
                    relacionesAFDLexema[nodos[i].Name][nodos[i].Relaciones[j].Destino.Name] = nodos[i].Relaciones[j].Name;
                }

            }
        }

        private void imprimeLista(List<Nodo> l)
        {
            for (int i = 0; i < l.Count; i++)
            {
                Console.Write(l[i].Name + " ");
            }
            Console.WriteLine();
        }

        private void ImprimeNodos()
        {
            for (int i = 0; i < nodos.Count; i++)
            {
                nodos[i].Imprime_Relaciones();
            }
        }

        private void ColocaEA(ref List<int> EdosAcepta)
        {
            // Console.WriteLine("Estados de aceptacion");
            for (int i = 0; i < Acepta.Count; i++)
            {
                // Console.WriteLine(Acepta[i]);
                EdosAcepta.Add(Acepta[i]);
            }
        }

        public Nodo Inicio
        {
            get { return inicio; }

        }

        public List<Nodo> Nodos
        {
            get { return nodos; }
        }

    }

}

