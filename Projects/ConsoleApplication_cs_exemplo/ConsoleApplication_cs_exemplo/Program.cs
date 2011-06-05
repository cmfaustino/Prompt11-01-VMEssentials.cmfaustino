using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication_cs_exemplo
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "abc";
            string r = s;
            s += "123";
            Console.WriteLine(s);
            Console.WriteLine(r);

            StringBuilder sb1 = new StringBuilder("abc");
            StringBuilder sb2 = sb1;
            sb1.Append("123");
            Console.WriteLine(sb2);

            Console.WriteLine("--- tipos ---");
            Type t = sb2.GetType();
            Console.WriteLine(t);
            Console.WriteLine(t.Name);
            Console.WriteLine(t.BaseType.Name);
            Console.WriteLine("--- foreach ---");
            foreach (var field in t.GetFields())
            {
                Console.WriteLine(field.FieldType.Name +" "+ field.Name);
            }

            Console.WriteLine("--- foreach ---");
            foreach (var field in s.GetType().GetFields())
            {
                Console.WriteLine(s.GetType().Name + " - " + field.FieldType.Name + " - " + field.Name);
            }

            Console.ReadLine();
        }
    }
}
