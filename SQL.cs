using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Compiladores
{
    public class SQL
    {
        //Diccionarios
        public Dictionary<String, String> Gramatica;
        public Dictionary<String, String> Siguientes;
        //Listas
        public List<String> terminal;
        public List<String> noterminal;
        //public List<Produce_a> produceA = new List<Produce_a>();
        public List<Produce_a> producciones = new List<Produce_a>();
        public List<string> palabrasRerservadas = new List<string>();
        public List<string> caracteres = new List<string>();
        public List<string> X = new List<string>();
        public List<string> T = new List<string>();
        public List<Conjuntos> conjuntos = new List<Conjuntos>();
        public List<Tokens> automataLR = new List<Tokens>();
        //variables
        public String Produccion;
        public AFD letter;
        public AFD digit;
        

        //Get y sets
        public Dictionary<String, String> GramDiccionario
        {
            get { return Gramatica; }
            set { Gramatica = value; }
        }
        public Dictionary<String, String> SigDiccionario
        {
            get { return Siguientes; }
            set { Siguientes = value; }
        }
        public List<Produce_a> Produce_A
        {
            get { return producciones; }
            set { producciones = value; }
        }
        public List<string> PalabrasReservadas
        {
            get { return palabrasRerservadas; }
            set { palabrasRerservadas = value; }
        }
        public List<String> NoTerminal
        {
            get { return noterminal; }
            set { noterminal = value; }
        }
        public List<String> Terminal
        {
            get { return terminal; }
            set { terminal = value; }
        }
        public List<string> CaracteresESPECIALES
        {
            get { return caracteres; }
            set { caracteres = value; }
        }
        //Funciones
        public void inicializaProducciones()
        {
            Gramatica = new Dictionary<String, String>
            {
                {"program", "statement-list"},
                {"statement-list", "statement statement-list|ε"},
                {"statement", "select-statement|insert-statement|update-statement|delete-statement"},
                {"select-statement" , "select select-list from table-name where-clause"},
                {"select-list", "*|column-list"},
                {"column-list", "column-name column-tail"},
                {"column-tail", ", column-name column-tail|ε"},
                {"table-name", "identifier"},
                {"column-name", "identifier"},
                {"where-clause", "where condition|ε"},
                {"condition", "column-name operator value"},
                {"operator", "=|<>|<|>|<=|>="},
                {"value", "number|identifier"},
                {"insert-statement", "insert into table-name ( column-list ) values ( value-list )"},
                {"value-list", "value value-tail"},
                {"value-tail", ", value value-tail|ε"},
                {"update-statement", "update table-name set update-list where condition"},
                {"update-list", "column-name = value update-tail"},
                {"update-tail", ", column-name = value update-tail|ε"},
                {"delete-statement", "delete from table-name where condition"},
                {"identifier", "letter"},
                {"number", "digit"},
            };
        
            
            producciones.Clear();
            producciones.Add(new Produce_a("program", "statement-list"));
            producciones.Add(new Produce_a("statement-list", "statement statement-list"));
            producciones.Add(new Produce_a("statement-list", ""));
            producciones.Add(new Produce_a("statement", "select-statement"));
            producciones.Add(new Produce_a("statement", "insert-statement"));
            producciones.Add(new Produce_a("statement", "update-statement"));
            producciones.Add(new Produce_a("statement", "delete-statement"));
            producciones.Add(new Produce_a("select-statement", "select select-list from table-name where-clause"));
            producciones.Add(new Produce_a("select-list", "*"));
            producciones.Add(new Produce_a("select-list", "column-list"));
            producciones.Add(new Produce_a("column-list", "column-name column-tail"));
            producciones.Add(new Produce_a("column-tail", ", column-name column-tail"));
            producciones.Add(new Produce_a("column-tail", ""));
            producciones.Add(new Produce_a("table-name", "identifier"));
            producciones.Add(new Produce_a("column-name", "identifier"));
            producciones.Add(new Produce_a("where-clause", "where condition"));
            producciones.Add(new Produce_a("where-clause", ""));
            producciones.Add(new Produce_a("condition", "column-name operator value"));
            producciones.Add(new Produce_a("operator", "="));
            producciones.Add(new Produce_a("operator", "<>"));
            producciones.Add(new Produce_a("operator", "<"));
            producciones.Add(new Produce_a("operator", ">"));
            producciones.Add(new Produce_a("operator", "<="));
            producciones.Add(new Produce_a("operator", ">="));
            producciones.Add(new Produce_a("value", "number"));
            producciones.Add(new Produce_a("value", "identifier"));
            producciones.Add(new Produce_a("insert-statement", "insert into table-name ( column-list ) values ( value-list )"));
            producciones.Add(new Produce_a("value-list", "value value-tail"));
            producciones.Add(new Produce_a("value-tail", ", value value-tail"));
            producciones.Add(new Produce_a("value-tail", ""));
            producciones.Add(new Produce_a("update-statement", "update table-name set update-list where condition"));
            producciones.Add(new Produce_a("update-list", "column-name = value update-tail"));
            producciones.Add(new Produce_a("update-tail", ", column-name = value update-tail"));
            producciones.Add(new Produce_a("update-tail", ""));
            producciones.Add(new Produce_a("delete-statement", "delete from table-name where condition"));
            producciones.Add(new Produce_a("identifier", "letter"));
            producciones.Add(new Produce_a("number", "digit"));
            
            foreach (var p in producciones)
            {
                if (!X.Contains(p.izq)) { X.Add(p.izq); }
            }
        }

        public void inicializaPrimSig()
        {
            conjuntos.Clear();
            List<string> primeros = new List<string>();
            List<string> segundos = new List<string>();
            terminal = new List<String> {
                "select",
                "from",
                "where",
                "insert",
                "into",
                "delete",
                "update",
                "set",
                "values",
                "letter",
                "digit",
                "(",
                ")",
                "=",
                "<>",
                "<",
                ">",
                "<=",
                ">=",
                "*",
                ","
            };
            primeros = new List<string>(new string[] { "", "select", "insert", "update", "delete" });
            segundos = new List<string>(new string[] { "$" });
            conjuntos.Add(new Conjuntos(primeros, "program'", segundos));
            conjuntos.Add(new Conjuntos(primeros, "program", segundos));
            conjuntos.Add(new Conjuntos(primeros, "statement-list", segundos));
            primeros = new List<string>(new string[] { "select", "insert", "update", "delete" });
            segundos = new List<string>(new string[] { "$", "select", "insert", "update", "delete" });
            conjuntos.Add(new Conjuntos(primeros, "statement", segundos));
            primeros = new List<string>(new string[] { "select" });
            conjuntos.Add(new Conjuntos(primeros, "select-statement", segundos));
            primeros = new List<string>(new string[] { "where", "" });
            Conjuntos whereclause = new Conjuntos(primeros, "where-clause", segundos);
            primeros = new List<string>(new string[] { "letter" });
            Conjuntos condition = new Conjuntos(primeros, "condition", segundos);
            primeros = new List<string>(new string[] { "insert" });
            Conjuntos insertstatement = new Conjuntos(primeros, "insert-statement", segundos);
            primeros = new List<string>(new string[] { "update" });
            Conjuntos updatestatement = new Conjuntos(primeros, "update-statement", segundos);
            primeros = new List<string>(new string[] { "delete" });
            Conjuntos deletestatement = new Conjuntos(primeros, "delete-statement", segundos);
            primeros = new List<string>(new string[] { "*", "letter" });
            segundos = new List<string>(new string[] { "from" });
            noterminal = new List<String> {
                "program",
                "statement-list",
                "statement",
                "select-statement",
                "insert-statement",
                "update-statement",
                "delete-statement",
                "select-list",
                "table-name",
                "where-clause",
                "column-list",
                "column-name",
                "column-tail",
                "identifier",
                "condition",
                "operator",
                "value",
                "value-list",
                "value-tail",
                "update-list",
                "update-tail",
                "number"
            };
            Produccion = ". program";
            conjuntos.Add(new Conjuntos(primeros, "select-list", segundos));
            primeros = new List<string>(new string[] { "letter" });
            segundos = new List<string>(new string[] { "from", ")" });
            conjuntos.Add(new Conjuntos(primeros, "column-list", segundos));
            primeros = new List<string>(new string[] { ",", "" });
            segundos = new List<string>(new string[] { "from", ")" });
            conjuntos.Add(new Conjuntos(primeros, "column-tail", segundos));
            primeros = new List<string>(new string[] { "letter" });
            segundos = new List<string>(new string[] { "where", "$", "select", "insert", "update", "delete", "(", "set" });
            conjuntos.Add(new Conjuntos(primeros, "table-name", segundos));
            primeros = new List<string>(new string[] { "letter" });
            segundos = new List<string>(new string[] { ",", "from", "=", "<>", "<", ">", "<=", ">=", ")" });
            conjuntos.Add(new Conjuntos(primeros, "column-name", segundos));
            conjuntos.Add(whereclause);
            conjuntos.Add(condition);
            primeros = new List<string>(new string[] { "=", "<>", "<", ">", "<=", ">=" });
            segundos = new List<string>(new string[] { "digit", "letter" });
            conjuntos.Add(new Conjuntos(primeros, "operator", segundos));
            primeros = new List<string>(new string[] { "digit", "letter" });
            segundos = new List<string>(new string[] { "$", "select", "insert", "update", "delete", ",", ")", "where" });
            conjuntos.Add(new Conjuntos(primeros, "value", segundos));
            conjuntos.Add(insertstatement);
            primeros = new List<string>(new string[] { "digit", "letter" });
            segundos = new List<string>(new string[] { ")" });
            conjuntos.Add(new Conjuntos(primeros, "value-list", segundos));
            primeros = new List<string>(new string[] { ",", "" });
            conjuntos.Add(new Conjuntos(primeros, "value-tail", segundos));
            conjuntos.Add(updatestatement);
            Gramatica = new Dictionary<String, String>
            {
                {"program", "statement-list"},
                {"statement-list", "statement statement-list|ε"},
                {"statement", "select-statement|insert-statement|update-statement|delete-statement"},
                {"select-statement" , "select select-list from table-name where-clause"},
                {"select-list", "*|column-list"},
                {"column-list", "column-name column-tail"},
                {"column-tail", ", column-name column-tail|ε"},
                {"table-name", "identifier"},
                {"column-name", "identifier"},
                {"where-clause", "where condition|ε"},
                {"condition", "column-name operator value"},
                {"operator", "=|<>|<|>|<=|>="},
                {"value", "number|identifier"},
                {"insert-statement", "insert into table-name ( column-list ) values ( value-list )"},
                {"value-list", "value value-tail"},
                {"value-tail", ", value value-tail|ε"},
                {"update-statement", "update table-name set update-list where condition"},
                {"update-list", "column-name = value update-tail"},
                {"update-tail", ", column-name = value update-tail|ε"},
                {"delete-statement", "delete from table-name where condition"},
                {"identifier", "letter"},
                {"number", "digit"},
            };
            primeros = new List<string>(new string[] { "letter" });
            segundos = new List<string>(new string[] { "where" });
            conjuntos.Add(new Conjuntos(primeros, "update-list", segundos));
            primeros = new List<string>(new string[] { ",", "" });
            segundos = new List<string>(new string[] { "where" });
            conjuntos.Add(new Conjuntos(primeros, "update-tail", segundos));
            conjuntos.Add(deletestatement);
            primeros = new List<string>(new string[] { "letter" });
            segundos = new List<string>(new string[] { "where", "$", "select", "insert", "update", "delete", ",", "from", "(", "set", "=", "<>", "<", ">", "<=", ">=", ")" });
            conjuntos.Add(new Conjuntos(primeros, "identifier", segundos));
            primeros = new List<string>(new string[] { "digit" });
            segundos = new List<string>(new string[] { "$", "select", "insert", "update", "delete", ",", ")", "where" });
            conjuntos.Add(new Conjuntos(primeros, "number", segundos));

        }
        public bool checkTerminal(string Token)
        {
            bool terminal = false;

            if (palabrasRerservadas.Contains(Token) || caracteres.Contains(Token) || Token.Equals("letter") || Token.Equals("digit") || Token.Equals("ε"))
            {
                terminal = true;
            }

            return terminal;
        }
        private List<string> createReservedWords()
        {
            List<string> reservedWords = new List<string>();

            string[] collection = new string[] { "select", "from", "where", "insert", "into", "values", "update", "set", "delete" };
            reservedWords.AddRange(collection);

            return reservedWords;
        }
        private List<string> createSpecialSymbols()
        {
            List<string> specialSymbols = new List<string>();

            string[] collection = new string[] { "*", ",", "=", "<>", "<", ">", "<=", ">=", "(", ")" };
            specialSymbols.AddRange(collection);

            return specialSymbols;
        }
        //Constructor
        public SQL() { }
        public SQL(AFD l, AFD d)
        {
            inicializaProducciones();
            inicializaPrimSig();
            palabrasRerservadas = createReservedWords();
            caracteres = createSpecialSymbols();
            X.AddRange(palabrasRerservadas);
            X.AddRange(caracteres);
            X.Add("letter");
            X.Add("digit");
            T.AddRange(palabrasRerservadas);
            T.AddRange(caracteres);
            T.Add("letter");
            T.Add("digit");
            letter = l;
            digit = d;
        }
    }
}
