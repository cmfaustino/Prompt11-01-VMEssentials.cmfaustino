using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sessao2
{   
    class ObjectoNaLista : IEqualityComparer<Object>
    {
        public bool Equals(object x, object y)
        {
            if (x.GetType() != y.GetType()) return false;
            if (x.GetType() == typeof(DirectoryInfo))
                return ((DirectoryInfo)x).FullName == ((DirectoryInfo)y).FullName;
            return x.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            if (obj.GetType() == typeof(DirectoryInfo))
                return ((DirectoryInfo)obj).FullName.Length;
            return obj.GetHashCode();
        }
    }

    class Program
    {
        static int contador = 0;
        static Dictionary<Object, int> lista = new Dictionary<Object, int>(new ObjectoNaLista());

        static void CriarGravarFicheiro(string nomedeficheiro, string texto)
        {
            StreamWriter swficheiro = new StreamWriter(@"c:\z_PROMPT\sessao2_resultado\" + nomedeficheiro);
            swficheiro.WriteLine(texto);
            swficheiro.Close();

            Console.WriteLine(nomedeficheiro + "\n\n" + texto + "\n\n");
        }

        static void CriarGravarFicheiroHTMLdeObjecto(string nomedeficheiro, string titulo, string texto)
        {
            string sheader = "<html>\n\n<head><title>";
            string smiddle1 = "</title></head>\n\n<body>\n\n<h1>";
            string smiddle2 = "</h1>\n\n";
            string sfooter = "</body>\n\n</html>";
            string srestexto = sheader + titulo + smiddle1 + titulo + smiddle2 + texto + sfooter;
            CriarGravarFicheiro(nomedeficheiro, srestexto);
        }

        static void ProcessarObjectoParaFicheirosHTML(Object objecto)
        {
            if (objecto == null) return;

            Type tipo = objecto.GetType();
            StringBuilder nomedeficheiro = new StringBuilder();
            StringBuilder titulo = new StringBuilder();
            StringBuilder texto = new StringBuilder();

            int v;

            if (lista.TryGetValue(objecto, out v))
            {
                return;
            }

            contador += 1;
            lista.Add(objecto, contador);

            nomedeficheiro.Append("ficheiro_" + contador.ToString() + ".htm");

            titulo.Append("Ficheiro " + contador.ToString());

            texto.Append(tipo.FullName + "<br />");
            texto.Append(objecto.ToString() + "<br />");
            texto.Append("<table>\n<tr><th>Propriedade</th><th>Tipo</th><th>Valor</th></tr>");  
            foreach (var propriedade in tipo.GetProperties())
            {
                texto.Append("<tr><td>" + propriedade.Name + "</td>");
                var propvalor = propriedade.GetValue(objecto, null);
                if (propvalor == null)
                {
                    texto.Append("<td>" + propriedade.PropertyType + "</td>");
                    texto.Append("<td>" + "&nbsp;" + "</td>");
                }
                else
                {
                    texto.Append("<td>" + propvalor.GetType().Name + "</td>");
                    if (propvalor.GetType().IsPrimitive || (propvalor.GetType() == typeof(String)) || (propvalor.GetType().GetProperties().Length == 0))
                    {
                        texto.Append("<td>" + propvalor.ToString() + "</td>");
                    }
                    else
                    {
                        ProcessarObjectoParaFicheirosHTML(propvalor);
                        texto.Append("<td><b>" + propvalor.ToString() + "</b></td>");
                    }
                }
                texto.Append("</tr>\n");
            }
            texto.Append("</table>");
            CriarGravarFicheiroHTMLdeObjecto(nomedeficheiro.ToString(), titulo.ToString(), texto.ToString());
        }

        static void Main(string[] args)
        {
            ProcessarObjectoParaFicheirosHTML(new DirectoryInfo(@"c:\program files"));
            
            Console.ReadLine();
        }
    }
}
