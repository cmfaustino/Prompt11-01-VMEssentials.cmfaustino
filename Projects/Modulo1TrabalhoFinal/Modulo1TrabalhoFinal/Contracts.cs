using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebReflector
{
    #region 3 Classes Rule...

    internal abstract class RuleComponent
    {
        public string content { get; private set; }

        protected RuleComponent(string content)
        {
            this.content = content ?? ""; // se null, transforma-se em vazia
        }

        public abstract bool MatchString(string s);
    }

    internal class RuleVariable : RuleComponent
    {
        public RuleVariable(string content) : base(content) { } // deveria verificar se content tem formato de variavel, se nao, lancar excepcao ???
        public override bool MatchString(string s)
        {
            return !string.IsNullOrEmpty(s); // true; // s tem de ter conteudo
        }
    }

    internal class RuleConstant : RuleComponent
    {
        public RuleConstant(string content) : base(content) { } // deveria verificar se content tem formato de constante, se nao, lancar excepcao ???
        public override bool MatchString(string s)
        {
            return ( !string.IsNullOrEmpty(s) && content.ToLower().Equals(s.ToLower()) ); // s tem de ter conteudo, e igualar como case-insensitive
        }
    }

    #endregion

    public class Contracts
    {
        private Dictionary<List<RuleComponent>, IHandler> listRulesHandlers;

        private char[] SEPARATOR_OF_RULES = { '/' };
        private string BEGINNING_OF_RULES = "/";
        private bool ENFORCE_BEGINNING_OF_RULES = true;
        private string BEGINNING_OF_VARIABLE_RULES = "{";
        private string ENDING_OF_VARIABLE_RULES = "}";

        #region 2 Construtores de Contracts

        public Contracts() // construtor default
        {
            listRulesHandlers = new Dictionary<List<RuleComponent>, IHandler>();
        }

        public Contracts(char[] SeparatorsOfRules, string BeginningOfRules, bool EnforceBeginningOfRules,
            string BeginningOfVariableValues, string EndingOfVariableValues) // construtor para redefinir todos os campos
        {
            listRulesHandlers = new Dictionary<List<RuleComponent>, IHandler>();
            SEPARATOR_OF_RULES = SeparatorsOfRules ?? "".ToArray();
            BEGINNING_OF_RULES = BeginningOfRules ?? "";
            ENFORCE_BEGINNING_OF_RULES = EnforceBeginningOfRules;
            BEGINNING_OF_VARIABLE_RULES = BeginningOfVariableValues ?? "";
            ENDING_OF_VARIABLE_RULES = EndingOfVariableValues ?? "";
        }

        #endregion

        #region 4 Metodos Auxiliares Is... e CheckFor...

        private bool IsVariable(string s) // é variável se tem o início definido e o final definido e conteudo adicional
        {
            return ( (s != null) && (s.Length > (BEGINNING_OF_VARIABLE_RULES.Length + ENDING_OF_VARIABLE_RULES.Length))
                && s.StartsWith(BEGINNING_OF_VARIABLE_RULES) && s.EndsWith(ENDING_OF_VARIABLE_RULES) );
        }

        //private bool IsConstant(string s)
        //{
        //    return ( (s != null) && !IsVariable(s) );
        //}

        //private bool CheckForBothNullsOrBothNotNulls(Object object1, Object object2)
        //{
        //    if ( ((object1 == null) && (object2 != null)) || ((object1 != null) && (object2 == null)) ) // um null e outro nao null
        //    {
        //        return false;
        //    }
        //    if ((object1 == null) && (object2 == null)) // dois nulls
        //    {
        //        return true;
        //    }
        //    return true; // dois nao nulls
        //}

        private bool CheckForCollectionsBothNullsOrWithSameCount(ICollection col1, ICollection col2)
        {
            if ( ((col1 == null) && (col2 != null)) || ((col1 != null) && (col2 == null)) ) // um null e outro nao null
            {
                return false;
            }
            if (col1 == null) // dois nulls
            {
                return true;
            }
            return (col1.Count == col2.Count); // dois nao nulls
        }

        #endregion

        #region 3 Metodos Auxiliares para o Metodo AddRule

        private List<RuleComponent> GenerateRuleComponents(List<string> listStrings)
        {
            if (listStrings==null)
            {
                return null;
            }
            var rule = new List<RuleComponent>();
            foreach (var s in listStrings)
            {
                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }
                if (IsVariable(s))
                {
                    rule.Add(new RuleVariable(s));
                }
                else
                {
                    rule.Add(new RuleConstant(s));
                }
            }
            return rule;
        }

        private bool EqualRules(List<RuleComponent> rule1, List<RuleComponent> rule2)
        {
            if (!CheckForCollectionsBothNullsOrWithSameCount(rule1, rule2)) // um null e outro nao null, ou dois nao nulls com counts diferentes
            {
                return false;
            }
            if (rule1 == null) // dois nulls
            {
                return true;
            }
            for (var i = 0; i < rule1.Count; i++) // dois nao nulls com counts iguais
            {
                if (!string.IsNullOrEmpty(rule1[i].content) && !string.IsNullOrEmpty(rule2[i].content)
                    && !rule1[i].content.Equals(rule2[i].content))
                {
                    return false;
                }
            }
            return true;
        }

        private bool RulesContains(List<RuleComponent> rule)
        {
            if(rule == null)
            {
                return false;
            }
            foreach (var ruleInList in listRulesHandlers.Keys)
            {
                if (EqualRules(rule, ruleInList))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Metodo Publico AddRule

        public void AddRule(string s, IHandler h)
        {
            if ((s == null) || (h == null)) // template tem de ser nao null, e handler tambem (se for necessario, define-se um handler que nada faz)
            {
                return;
            }
            var listStrings = s.Split(SEPARATOR_OF_RULES).Where((sExpr) => sExpr.Length > 0).ToList();
            if (ENFORCE_BEGINNING_OF_RULES && s.StartsWith(BEGINNING_OF_RULES)) // adicionar barra no inicio, para ser considerada tb RuleComponent
            {
                listStrings.Reverse();
                listStrings.Add(BEGINNING_OF_RULES);
                listStrings.Reverse();
            }

            var rule = GenerateRuleComponents(listStrings);
            //if ((rule != null) && (rule.Count > 0) && (!listRulesHandlers.ContainsKey(rule)))
            if ((rule != null) && (rule.Count > 0) && (!RulesContains(rule)))
            {
                listRulesHandlers.Add(rule, h);
            }
        }

        #endregion

        #region 2 Metodos Auxiliares para o Metodo ResolveUri

        private bool MatchRule(List<RuleComponent> rule, List<string> listStrings)
        {
            if (!CheckForCollectionsBothNullsOrWithSameCount(rule, listStrings)) // um null e outro nao null, ou dois nao nulls com counts diferentes
            {
                return false;
            }
            if (rule == null) // dois nulls
            {
                return true;
            }
            for (var i = 0; i < listStrings.Count; i++) // dois nao nulls com counts iguais
            {
                if ((rule[i] == null) || string.IsNullOrEmpty(rule[i].content) || string.IsNullOrEmpty(listStrings[i]))
                {
                    return false;
                }
                if (!rule[i].MatchString(listStrings[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private List<RuleComponent> MatchUri(string uri, out List<string> listStringsReturned, out IHandler handlerReturned)
        { // se contexto pode conter vários níveis de pastas, então, o que impede .../ns de fazer parte do contexto, por ex.? Prof diz que nao pode.
            if(string.IsNullOrEmpty(uri)) // uri sem conteudo
            {
                listStringsReturned = null;
                handlerReturned = null;
                return null;
            }
            var listStrings = uri.Split(SEPARATOR_OF_RULES).Where((s) => s.Length > 0).ToList();
            if (ENFORCE_BEGINNING_OF_RULES && uri.StartsWith(BEGINNING_OF_RULES)) // adicionar barra no inicio, para ser considerada tb RuleComponent
            {
                listStrings.Reverse();
                listStrings.Add(BEGINNING_OF_RULES);
                listStrings.Reverse();
            }
            listStringsReturned = listStrings;
            foreach (var rule in listRulesHandlers) // uri com conteudo
            {
                if (MatchRule(rule.Key, listStrings)) // encontrou match rule
                {
                    handlerReturned = rule.Value;
                    return rule.Key;
                }
            }
            handlerReturned = null; // nao encontrou match rule
            return null;
        }

        #endregion

        #region Metodo Publico ResolveUri

        public Dictionary<string, string> ResolveUri(string uriExtern, out IHandler handlerReturned)
        {
            List<string> listStrings;
            var rule = MatchUri(uriExtern, out listStrings, out handlerReturned);
            if (rule == null)
            {
                return null;
            }
            var paramsVarsReturned = new Dictionary<string, string>(); // parametros = pares de variaveis e valores
            for (int i = 0; i < rule.Count; i++)
            {
                if (IsVariable(rule[i].content))
                { // remover delimitadores definidos
                    var varNameWithoutDelimitators = rule[i].content.ToList();
                    varNameWithoutDelimitators.RemoveRange(0, BEGINNING_OF_VARIABLE_RULES.Length); // inicial
                    varNameWithoutDelimitators.Reverse();
                    varNameWithoutDelimitators.RemoveRange(0, ENDING_OF_VARIABLE_RULES.Length); // final
                    varNameWithoutDelimitators.Reverse();
                    paramsVarsReturned.Add(new string(varNameWithoutDelimitators.ToArray()), listStrings[i]); // adiciona variavel e valor concreto
                }
            }
            return paramsVarsReturned;
        }

        #endregion

        // metodos apenas para serem visiveis nos testes
        #region Metodos Auxiliares para serem visiveis e usados nos Testes apenas

        public int Testes_GetRulesCount()
        {
            return listRulesHandlers.Count;
        }

        //public List<RuleComponent> Testes_Match(string uriExterno, out List<string> lststr, out IHandler handlerRetornado)
        public void Testes_Match(string uriExterno, out List<string> lststr, out IHandler handlerRetornado)
        {
            //return MatchUri(uriExterno, out lststr, out handlerRetornado);
            var rule = MatchUri(uriExterno, out lststr, out handlerRetornado);
        }

        public bool Testes_IsVariable(string s)
        {
            return IsVariable(s);
        }

        #endregion
    }
}
