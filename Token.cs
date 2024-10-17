using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class Token
    {
        //Variables
        public string lexema;
        public string nombre;

        //funciones sets y gets
        public string Lexema
        {
            get { return lexema; }
            set { lexema = value; }
        }
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        //Constructores
        public Token() { }
        public Token(string lexema, string nombre)
        {
            this.lexema = lexema;
            this.nombre = nombre;
        }
    }
}
