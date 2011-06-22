using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sessao5
{
    //nada
    public class InvalidRationalException : Exception //resolucao0: criada a classe
    {
    }

    public static class Utilitarios //resolucao2: criada a classe estatica
    {
        public static IEnumerable<T> RemoveDuplicated<T>(this IEnumerable<T> seq) //resolucao2: acrescentado o this no 1o argumento, e fazendo funcao
        {
            List<T> lista = new List<T>();
            //bool itens_iguais;
            foreach(var item in seq)
            {
                //itens_iguais = false;
                //foreach (var it2 in lista)
                //{
                //    if (item.Equals(it2))
                //        itens_iguais = true;
                //}
                //itens_iguais = lista.Contains(item);
                //if (!itens_iguais)
                if (!lista.Contains(item))
                {
                    lista.Add(item);
                    yield return item;
                }
            }
        }

        //parte I - pergunta 3
        public static IEnumerable<T> OrderBy<T, U>(this IEnumerable<T> seq, Func<T, U> criterium)
                    where U : IComparable<U>
        {
            var a = seq.ToArray();
            Array.Sort(a, (t1, t2) => criterium(t1).CompareTo(criterium(t2)));
            return a;
        }
    }

    //public struct RationalNumber
    public struct RationalNumber : IComparable<RationalNumber> , IEquatable<RationalNumber> //resolucao1: adicionadas as intefaces
    {
        private readonly int _numerator, _denominator;

        //RationalNumber(int numerator, int denominator)
        public RationalNumber(int numerator, int denominator) //resolucao0: alterada a linha
        {
            if (denominator == 0) throw new InvalidRationalException();
            _numerator = numerator;
            _denominator = denominator;
        }

        public static RationalNumber operator +(RationalNumber n1, RationalNumber n2)
        {
            int nn, nd;
            if (n1._denominator == n2._denominator)
            {
                nn = n1._numerator + n2._numerator;
                nd = n1._denominator;
            }
            else
            {
                nn = n1._numerator * n2._denominator + n2._numerator * n1._denominator;
                nd = n1._denominator * n2._denominator;
            }
            return new RationalNumber(nn, nd);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(_numerator);
            sb.Append('/');
            sb.Append(_denominator);
            return sb.ToString();
        }

        public int CompareTo(RationalNumber rn) //resolucao1: adicionado o método - implementar
        {
            return 0;
        }

        public bool Equals(RationalNumber rn) //resolucao1: adicionadas o método - implementar
        {
            return false;
        }
    }

    public struct Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //parte I - pergunta 1
            List<RationalNumber> rnumbers = new List<RationalNumber>
                                                {
                                                        new RationalNumber(1, 1),
                                                        new RationalNumber(2, 1),
                                                };
            var n = rnumbers.Where(r => r.Equals(new RationalNumber(1, 1)));
            foreach (var r in n)
                Console.WriteLine(r);
            rnumbers.Sort();
            foreach (var r in n)
                Console.WriteLine(r);

            //parte I - pergunta 3
            List<Book> books = new List<Book>
                    {
                        new Book {Title = "Ensaio sobre a cegueira", Author = "Saramago",
                                PublishDate = new DateTime(2005, 12, 3) },
                        new Book {Title = "Memorial do Convento", Author = "Saramago",
                                PublishDate = new DateTime(1984, 12, 3) },
                        new Book {Title = "Lusiadas", Author = "Camoes",
                                PublishDate = new DateTime(1600, 12, 3) }
                    };
        }
    }
}
