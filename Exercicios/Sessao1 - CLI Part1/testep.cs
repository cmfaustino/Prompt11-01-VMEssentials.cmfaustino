using System;

namespace Sessao1
{
    class Program
    {
        static void Main(string[] args)
        {
          Ponto p1 = new Ponto(2,3);
		  Ponto p2 = new Ponto(4,5);
		  int d1 = p1.Distance(p2);
		  int d2 = p2.Distance(p1);
		  string s1 = "O ponto 1 é " + p1.ToString() + ";";
		  string s2 = "O ponto 2 é " + p2.ToString() + ";";
		  string sres = "A distância resultante entre os 2 pontos acima é " + d1 +
			" que é igual a " + d2 + ".";
			Console.WriteLine(s1);
			Console.WriteLine(s2);
			Console.WriteLine(sres);
        }
    }
}
