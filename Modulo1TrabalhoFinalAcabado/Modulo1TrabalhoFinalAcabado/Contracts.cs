using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebReflector
{
    #region 3 Classes Rule...

    internal abstract class RuleComponent
    {
        public string Content { get; private set; }

        protected RuleComponent(string content)
        {
            this.Content = content ?? ""; // se null, transforma-se em vazia
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
            return ( !string.IsNullOrEmpty(s) && Content.ToLower().Equals(s.ToLower()) ); // s tem de ter conteudo, e igualar como case-insensitive
        }
    }

    #endregion

    public class Contracts
    {
        private Dictionary<List<RuleComponent>, IHandler> listRulesHandlers;

        private readonly char[] SEPARATOR_OF_RULES = { '/' };
        private readonly string BEGINNING_OF_RULES = "/";
        private readonly bool ENFORCE_BEGINNING_OF_RULES = true;
        private readonly string BEGINNING_OF_VARIABLE_RULES = "{";
        private readonly string ENDING_OF_VARIABLE_RULES = "}";
        private readonly string BEGINNING_OF_CONSTANT_RULES = ""; //":";
        private readonly string ENDING_OF_CONSTANT_RULES = ""; //":";

        #region 2 Construtores de Contracts

        public Contracts() // construtor default
        {
            listRulesHandlers = new Dictionary<List<RuleComponent>, IHandler>();
        }

        public Contracts(char[] separatorsOfRules, string beginningOfRules, bool enforceBeginningOfRules,
            string beginningOfVariableValues, string endingOfVariableValues,
            string beginningOfConstantRules, string endingOfConstantRules) // construtor para redefinir todos os campos
        {
            listRulesHandlers = new Dictionary<List<RuleComponent>, IHandler>();
            SEPARATOR_OF_RULES = separatorsOfRules ?? "".ToArray();
            BEGINNING_OF_RULES = beginningOfRules ?? "";
            ENFORCE_BEGINNING_OF_RULES = enforceBeginningOfRules;
            BEGINNING_OF_VARIABLE_RULES = beginningOfVariableValues ?? "";
            ENDING_OF_VARIABLE_RULES = endingOfVariableValues ?? "";
            BEGINNING_OF_CONSTANT_RULES = beginningOfConstantRules ?? "";
            ENDING_OF_CONSTANT_RULES = endingOfConstantRules ?? "";
        }

        #endregion

        #region 2 Metodos Auxiliares Is... _ e 2 CheckFor... em MetodosEstaticos.cs

        private bool IsVariable(string s) // é variável se tem o início definido e o final definido e conteudo adicional
        {
            return ( (s != null) && (s.Length > (BEGINNING_OF_VARIABLE_RULES.Length + ENDING_OF_VARIABLE_RULES.Length))
                && s.StartsWith(BEGINNING_OF_VARIABLE_RULES) && s.EndsWith(ENDING_OF_VARIABLE_RULES) );
        }

        private bool IsConstant(string s)
        {
            //return ((s != null) && !IsVariable(s));
            return ((s != null) && (s.Length > (BEGINNING_OF_CONSTANT_RULES.Length + ENDING_OF_CONSTANT_RULES.Length))
                && s.StartsWith(BEGINNING_OF_CONSTANT_RULES) && s.EndsWith(ENDING_OF_CONSTANT_RULES));
        }

        #endregion

        #region 3 Metodos Auxiliares para o Metodo AddRule _ GenerateRuleComponents e EqualRules e RulesContains

        private List<RuleComponent> GenerateRuleComponents(IEnumerable<string> enumStrings)
        {
            if (enumStrings==null)
            {
                return null;
            }
            var rule = new List<RuleComponent>();
            foreach (var s in enumStrings)
            {
                if (string.IsNullOrEmpty(s))
                {
                    continue; //ignorar conteúdo nulo ou vazio
                }
                if (IsVariable(s))
                {
                    rule.Add(new RuleVariable(s));
                }
                else if (IsConstant(s))
                {
                    rule.Add(new RuleConstant(s));
                }
                else
                {
                    continue; // talvez, em vez de se ignorar conteúdo inválido, atirar excepção?!
                }
            }
            return rule;
        }

        private static bool EqualRules(List<RuleComponent> rule1, List<RuleComponent> rule2)
        {
            if (!MetodosEstaticos.CheckForCollectionsBothNullsOrWithSameCount(rule1, rule2))
            { // um null e outro nao null, ou dois nao nulls com counts diferentes
                return false;
            }
            if (rule1 == null) // dois nulls
            {
                return true;
            }
            for (var i = 0; i < rule1.Count; i++) // dois nao nulls com counts iguais
            {
                if (!string.IsNullOrEmpty(rule1[i].Content) && !string.IsNullOrEmpty(rule2[i].Content)
                    && !rule1[i].Content.Equals(rule2[i].Content))
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
            var listStrings = s.Split(SEPARATOR_OF_RULES).Where(sExpr => sExpr.Length > 0).ToList();
            if (ENFORCE_BEGINNING_OF_RULES && s.StartsWith(BEGINNING_OF_RULES)) // adicionar barra no inicio, para ser considerada tb RuleComponent
            {
                listStrings.Reverse();
                listStrings.Add(BEGINNING_OF_RULES);
                listStrings.Reverse();
            }

            var rule = GenerateRuleComponents(listStrings);
            //var rule = GenerateRuleComponents(listStrings.Where(s => (IsVariable(s) || IsConstant(s))));
            //if ((rule != null) && (rule.Count > 0) && (!listRulesHandlers.ContainsKey(rule)))
            if ((rule != null) && (rule.Count > 0) && (!RulesContains(rule)))
            {
                listRulesHandlers.Add(rule, h);
            }
        }

        #endregion

        #region 2 Metodos Auxiliares para o Metodo ResolveUri _ MatchRule e MatchUri

        private static bool MatchRule(List<RuleComponent> rule, List<string> listStrings)
        {
            if (!MetodosEstaticos.CheckForCollectionsBothNullsOrWithSameCount(rule, listStrings))
            { // um null e outro nao null, ou dois nao nulls com counts diferentes
                return false;
            }
            if (rule == null) // dois nulls
            {
                return true;
            }
            for (var i = 0; i < listStrings.Count; i++) // dois nao nulls com counts iguais
            {
                if ((rule[i] == null) || string.IsNullOrEmpty(rule[i].Content) || string.IsNullOrEmpty(listStrings[i]))
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
            var listStrings = uri.Split(SEPARATOR_OF_RULES).Where(s => s.Length > 0).ToList();
            if (ENFORCE_BEGINNING_OF_RULES && uri.StartsWith(BEGINNING_OF_RULES)) // adicionar barra no inicio, para ser considerada tb RuleComponent
            {
                listStrings.Reverse();
                listStrings.Add(BEGINNING_OF_RULES);
                listStrings.Reverse();
            }
            listStringsReturned = listStrings;
            //listStringsReturned = listStrings.Where(s => (IsVariable(s) || IsConstant(s))).ToList();
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
                if (IsVariable(rule[i].Content))
                { // remover delimitadores definidos
                    var varNameWithoutDelimitators = rule[i].Content.ToList();
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

        public bool Testes_IsConstant(string s)
        {
            return IsConstant(s);
        }

        #endregion
    }
}
