using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WebReflector
{
    // 3 Componentes Principais - Classe HandlerAnswer, Interface IHandler, Classe Excepcao HandlerException

    public class HandlerAnswer
    {
        public bool answerOk { get; set; }
        public HttpStatusCode answerCode { get; set; }
        public TagHTML answerContent { get; set; }
    }

    public interface IHandler
    {
        HandlerAnswer Handle(Dictionary<string, string> prms);
    }

    public class HandlerException : SystemException
    {
        public Dictionary<string, string> parametros { get; set; }

        public HandlerException() : base() { }
        public HandlerException(string message) : base(message) { }
        public HandlerException(string message, System.Exception inner) : base(message, inner) { }

        public HandlerException(string message, Dictionary<string, string> parametros)
            : base(message)
        {
            this.parametros = parametros;
        }

        public HandlerException(string message, System.Exception inner, Dictionary<string, string> parametros)
            : base(message, inner)
        {
            this.parametros = parametros;
        }
    }
}
