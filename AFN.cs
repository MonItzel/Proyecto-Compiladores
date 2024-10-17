using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class AFN
    {
        private Nodo inicio;
        private Nodo aceptacion;

        public List<Estado> estados;
        public string operadores = "*+?&|";
        private string opUnarios = "*+?";
        private string opBinarios = "&|";
        public string alfabeto;
        public string posfija;
        public int numEstados = 0;
        public TokensAux afnres;

        /**
        *Método auxiliar que funciona para colocar a todos los nodos visitados
        *como no visitados
        */
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

        /*
       * Método que genera el autómata
       * @Param: Recibe la expresión posfija que se obtuvo en el
       *         método, la recorre para verificar que tipo de es
       *         y agregarla a la pila
       * @Return: Regresa el autómata
       */
        public AFN(string posfija)
        {
            estados = new List<Estado>();
            generaAlfabeto(posfija);
            inicio = new Nodo(1);
            aceptacion = new Nodo(3);
            Arista ar = new Arista(inicio, aceptacion, posfija);
            inicio.Relaciones.Add(ar);
        }

        
        #region BootUp
        public Estado? getEstado(int index)
        {
            foreach (Estado e in estados)
            {
                if (e.id == index)
                    return e;
            }

            return null;
        }

        public void generaAlfabeto(string posfija)
        {
            string res = "";
            posfija = posfija.Trim('\0');
            foreach (char c in posfija)
            {
                if (!operadores.Contains(c) && !res.Contains(c))
                {
                    res += c;
                }
            }
            res += "ε";
            this.posfija = posfija;
            alfabeto = res;
        }
        #endregion

        public TokensAux generaAFN()
        {
            //Pila de operandos.
            Stack<TokensAux> operandos = new Stack<TokensAux>();
            TokensAux opAux, op1, op2;

            //Por cada caracter en la posfija, se evalua.
            foreach (char c in posfija)
            {
                opAux = new TokensAux();
                if (alfabeto.Contains(c))
                {
                    operandos.Push(generaBase(c));
                }
                else
                {
                    if (opUnarios.Contains(c))
                    {
                        switch (c)
                        {
                            case '*':
                                opAux = operandos.Pop();
                                operandos.Push(Kleene(opAux));
                                break;
                            case '+':
                                opAux = operandos.Pop();
                                operandos.Push(Positiva(opAux));
                                break;
                            case '?':
                                opAux = operandos.Pop();
                                operandos.Push(ZeroUno(opAux));
                                break;
                        }
                    }
                    else if (opBinarios.Contains(c))
                    {
                        switch (c)
                        {
                            case '&':
                                op2 = operandos.Pop();
                                op1 = operandos.Pop();
                                opAux = Concatenacion(op1, op2);
                                operandos.Push(opAux);
                                break;
                            case '|':
                                op2 = operandos.Pop();
                                op1 = operandos.Pop();
                                opAux = Alternativas(op1, op2);
                                operandos.Push(opAux);
                                break;
                        }
                    }
                }
            }

            afnres = operandos.Pop();
            return afnres;
        }


        #region generarMapaAFN

        // 0 - inicial | 1 - neutro | 2 - aceptacion
        public Estado generaEstado(int tipo)
        {
            Estado nuevoEstado = new Estado(numEstados, tipo);
            numEstados++;
            return nuevoEstado;
        }

        // -> o - char c (simbolo) -> of
        public TokensAux generaBase(char c)
        {
            TokensAux nuevo = new TokensAux();

            Estado inicial = generaEstado(0);
            Estado final = generaEstado(2);

            inicial.generaTransicion(c, final);

            nuevo.estados.Add(inicial);
            nuevo.estados.Add(final);

            return nuevo;
        }

        #region opUnarios
        //Cerradura de Kleene (*) ε 
        public TokensAux Kleene(TokensAux op)
        {
            TokensAux nuevo = new TokensAux();

            //1. agregar el loop de la cerradura de kleene
            op.estados.Last().changeTipoNeutro();
            op.estados.First().changeTipoNeutro();
            op.estados.Last().generaTransicion('ε', op.estados.First());

            //2. agregar el nuevo estado inicial
            Estado inicial = generaEstado(0);
            inicial.generaTransicion('ε', op.estados.First());
            nuevo.estados.Add(inicial);
            nuevo.estados.AddRange(op.estados);

            //3. agregar el nuevo estado final
            Estado final = generaEstado(2);
            nuevo.estados.Last().generaTransicion('ε', final);
            nuevo.estados.Add(final);

            //4. agregar el skip de la cerradura de kleene
            nuevo.estados.First().generaTransicion('ε', nuevo.estados.Last());

            return nuevo;
        }

        //Cerradura Positiva (+)
        public TokensAux Positiva(TokensAux op)
        {
            TokensAux nuevo = new TokensAux();

            //1. agregar el loop de la cerradura positiva
            op.estados.Last().changeTipoNeutro();
            op.estados.First().changeTipoNeutro();
            op.estados.Last().generaTransicion('ε', op.estados.First());

            //2. agregar el nuevo estado inicial
            Estado inicial = generaEstado(0);
            inicial.generaTransicion('ε', op.estados.First());
            nuevo.estados.Add(inicial);
            nuevo.estados.AddRange(op.estados);

            //3. agregar el nuevo estado final
            Estado final = generaEstado(2);
            nuevo.estados.Last().generaTransicion('ε', final);
            nuevo.estados.Add(final);

            return nuevo;
        }

        //Cero o Uno
        public TokensAux ZeroUno(TokensAux op)
        {
            TokensAux nuevo = new TokensAux();

            //1. agregar el nuevo estado inicial
            op.estados.Last().changeTipoNeutro();
            op.estados.First().changeTipoNeutro();
            Estado inicial = generaEstado(0);
            inicial.generaTransicion('ε', op.estados.First());
            nuevo.estados.Add(inicial);
            nuevo.estados.AddRange(op.estados);

            //2. agregar el nuevo estado final
            Estado final = generaEstado(2);
            nuevo.estados.Last().generaTransicion('ε', final);
            nuevo.estados.Add(final);

            //3. agregar el skip de Zero
            nuevo.estados.First().generaTransicion('ε', nuevo.estados.Last());

            return nuevo;
        }

        #endregion

        #region opBinarios

        // Operador Binario & Concatenacion
        public TokensAux Concatenacion(TokensAux op1, TokensAux op2)
        {
            TokensAux nuevo = new TokensAux();

            //1. neutralizar op1 last
            op1.estados.Last().changeTipoNeutro();

            //2. obtener transiciones de op2 first en op1 last
            op1.estados.Last().transiciones.AddRange(op2.estados.First().transiciones);

            //3. remover op2 first
            op2.estados.RemoveAt(0);

            //4. agregar a nuevo operando
            nuevo.estados.AddRange(op1.estados);
            nuevo.estados.AddRange(op2.estados);

            return nuevo;
        }

        // Operador Binario | Seleccion de Alternativas
        public TokensAux Alternativas(TokensAux op1, TokensAux op2)
        {
            TokensAux nuevo = new TokensAux();

            //1. neutralizar op1 y op2
            op1.estados.Last().changeTipoNeutro();
            op1.estados.First().changeTipoNeutro();
            op2.estados.Last().changeTipoNeutro();
            op2.estados.First().changeTipoNeutro();

            //2. agregar nuevo estado inicial
            Estado inicial = generaEstado(0);
            inicial.generaTransicion('ε', op1.estados.First());
            inicial.generaTransicion('ε', op2.estados.First());

            //3. agregar nuevo estado final
            Estado final = generaEstado(2);
            op1.estados.Last().generaTransicion('ε', final);
            op2.estados.Last().generaTransicion('ε', final);

            //4. crear la nueva lista
            nuevo.estados.Add(inicial);
            nuevo.estados.AddRange(op1.estados);
            nuevo.estados.AddRange(op2.estados);
            nuevo.estados.Add(final);

            return nuevo;
        }

        #endregion



        #endregion

        
        public AFN()
        {
            inicio = null;
            aceptacion = null;
        }

        /*
        * Método que genera el autómata
        * @Param: Recibe la expresión posfija que se obtuvo en el
        *         método, la recorre para verificar que tipo de es
        *         y agregarla a la pila
        * @Return: Regresa el autómata
        */
        /*
        public AFN(string arista_name)
        {
            inicio = new Nodo(1);
            aceptacion = new Nodo(3);
            Arista ar = new Arista(inicio, aceptacion, arista_name);
            inicio.Relaciones.Add(ar);
        }*/

       

        /*
         * Genera la cerradura de épsilon*
         * @Param: Recibe el nodo al cual se le aplicará.
         * 
         */
        public void Cerradura_de_Epsilon(Nodo n, List<Nodo> list)
        {
            //Console.Write(n.Name + "-- >");
            list.Add(n);
            if (n.Tipo == 3)
                return;
            for (int i = 0; i < n.Relaciones.Count; i++)
            {
                //Console.WriteLine(n.Relaciones[i].Name.ToString() + n.Relaciones[i].Destino.Visited.ToString());
                if (n.Relaciones[i].Name == "ε")
                    Cerradura_de_Epsilon(n.Relaciones[i].Destino, list);
            }
        }



        public Nodo Inicio
        {
            get { return inicio; }
            set { inicio = value; }
        }

        public Nodo Aceptacion
        {
            get { return aceptacion; }
            set { aceptacion = value; }
        }



    }
}

