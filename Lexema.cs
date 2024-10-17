using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    internal class Lexema
    {
        /*
       *Algoritmo que convierte de la notación infija a postfija
       *@Param: El agoritmo recibe la cadena de tipo char que contiene el lexema a validar,
       *        pos contiene la posición del lexema, y la variable busca la posición para buscar 
       *        en la lista de relaciones AFD.
       *     
       *@Return: regresa 1 si encontró la cadena, 0 en caso contrario.    
       */
        public int VerificaLexema(char[] cadena, List<List<string>> relacionesAFD, ref int busca, int pos, int cantalf, int repetidos)
        {
            Debug.WriteLine("num alfabeto  " + cantalf);
            Debug.WriteLine("repetidos " + repetidos);
            int retu = 0;
            if (pos < cadena.Length)
            {
                for (int i = 0; i < cantalf + repetidos + 1; i++)
                {
                    Debug.WriteLine("busca " + cadena[pos] + " en " + relacionesAFD[busca][i]);
                    if (cadena[pos].ToString() == relacionesAFD[busca][i])
                    {
                        if (pos < cadena.Length)
                        {
                            busca = i;
                            retu = VerificaLexema(cadena, relacionesAFD, ref busca, pos + 1, cantalf, repetidos);
                            break;
                        }

                    }
                }
            }
            else
            {
                retu = 1;
            }
            return retu;
        }
        public int VerificaTokens(char[] cadena, List<List<string>> relacionesAFD, ref int busca, int pos, int numenca)
        {
            int retu = 0;
            if (pos < cadena.Length)
            {

                for (int i = 0; i < numenca; i++)
                {
                    if (cadena[pos].ToString() == relacionesAFD[busca][i])
                    {
                        if (pos < cadena.Length)
                        {
                            busca = i;
                            retu = VerificaTokens(cadena, relacionesAFD, ref busca, pos + 1, numenca);
                            break;
                        }

                    }
                }
            }
            else
            {
                retu = 1;
            }
            return retu;
        }
    }

}


