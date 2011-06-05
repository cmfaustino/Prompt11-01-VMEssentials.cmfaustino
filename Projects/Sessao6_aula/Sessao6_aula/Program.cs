using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace Sessao6_aula //GC.Examples
{
    class A
    {
        ~A()
        {
            Console.WriteLine("Finalizer running A");
        }
    }

    class B : IDisposable
    {
        //StringBuilder sname;
        string sname;

        public B(string s)
        {
            //sname = new StringBuilder();
            //sname.Append(s);
            sname = s;
            Console.WriteLine("criacao de {0}", sname);
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose of {0}",sname);
        }

        ~B()
        {
            Console.WriteLine("Finalizer running B");
        }
    }

    class Program
    {
        static void WeakReferenceExample()
        {
            var o = new Object();
            var wr = new WeakReference(o);
            for (int i = 0; i < 10; ++i)
            {
                if (i == 5) o = null;
                System.GC.Collect();
                Console.WriteLine("iter {0}, alive = {1}, target = {2}", i, wr.IsAlive, wr.Target);
            }
        }

        static void FinalizerExample()
        {
            A a = new A();
            a = null;
            //while (true)
            for (int j = 0; j < 20; ++j)
            {
                Console.WriteLine("about to sleep");
                Thread.Sleep(100);
                System.GC.Collect();
            }
        }

        static void UsingExample()
        {
            using (var b2 = new B("b2"))
            using (var b = new B("b"))
            {
                //throw new Exception();
                Console.WriteLine("About to end using");
                //b2 = null;
            }

            var b3 = new B("b3");
            //try
            //{
            //}
            //finally
            //{
            //}
        }

        static void HttpListenerExamples()
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8080/");
                listener.Start();
                while (true)
                {
                    var ctx = listener.GetContext();
                    Console.WriteLine("Context received");
                    Console.WriteLine("URL: {0}", ctx.Request.Url);
                    Console.WriteLine("Method: {0}", ctx.Request.HttpMethod);
                    ;
                }
            }
        }

        static void Main(string[] args)
        {
            //WeakReferenceExample();
            //FinalizerExample();
            //UsingExample();
            HttpListenerExamples();
        }
    }
}
