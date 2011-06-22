using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebReflector
{
    static public class Router
    {
        //private Contracts contracts;

        #region 3 Construtores de Router - nao utilizados

        //public Router()
        //{
        //    this.contracts = new Contracts();
        //}

        //public Router(Contracts contracts)
        //{
        //    this.contracts = contracts ?? new Contracts();
        //}

        //public Router(Contracts contracts, List<string> listTemplates, List<IHandler> listHandlers)
        //{
        //    this.contracts = contracts ?? new Contracts();
        //    if (((listTemplates != null) && (listHandlers != null)) && (listTemplates.Count == listHandlers.Count))
        //    {
        //        for (int index = 0; index < listTemplates.Count; index++)
        //        {
        //            this.contracts.AddRule(listTemplates[index], listHandlers[index]);
        //        }
        //    }
        //}

        #endregion

        #region 2 Metodos Publicos AddRule e AddRules - nao utilizados

        //public void AddRule(string s, IHandler h)
        //{
        //    contracts.AddRule(s,h);
        //}

        //public void AddRules(List<string> listTemplates, List<IHandler> listHandlers)
        //{
        //    if (((listTemplates != null) && (listHandlers != null)) && (listTemplates.Count == listHandlers.Count))
        //    {
        //        for (int index = 0; index < listTemplates.Count; index++)
        //        {
        //            contracts.AddRule(listTemplates[index], listHandlers[index]);
        //        }
        //    }
        //}

        #endregion

        #region Metodo Publico MatchAndExecute

        //public HandlerAnswer MatchAndExecute(string uriPedido)
        static public HandlerAnswer MatchAndExecute(Contracts contracts, string uriPedido)
        {
            IHandler handlerToExecute;
            if(contracts == null)
            {
                //throw new HandlerException("Lista de Templates nula, não há template correspondente ao endereço indicado: " + uriPedido);
                return (new NullsHandler(uriPedido,false)).Handle();
            }
            else
            {
                Dictionary<string, string> paramsVarsReturned = contracts.ResolveUri(uriPedido, out handlerToExecute);
                if (handlerToExecute == null)
                {
                    //throw new HandlerException("Handler nulo, não há template correspondente ao endereço indicado: " + uriPedido);
                    handlerToExecute = new NullsHandler(uriPedido,true);
                }
                return handlerToExecute.Handle(paramsVarsReturned);
            }
        }

        #endregion
    }
}
