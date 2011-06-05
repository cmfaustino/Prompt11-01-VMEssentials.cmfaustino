using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text.RegularExpressions;

namespace WebReflector
{
    class NodeArvoreTemplates_old
    {
        public string parteConstante;
        public string parteVariavel;
        public List<NodeArvoreTemplates_old> subNodes;

        public NodeArvoreTemplates_old(string c, string v)
        {
            parteConstante = c;
            parteVariavel = v;
            subNodes = new List<NodeArvoreTemplates_old>();
        }

        public void AddNodeArvoreTemplates(NodeArvoreTemplates_old n)
        {
            subNodes.Add(n);
        }
    }

    class Contracts_outroModo
    {
        public NodeArvoreTemplates_old arvoreTemplates;
        public Dictionary<string, NodeArvoreTemplates_old> listaTemplates;

        public Contracts_outroModo()
        {
            listaTemplates = new Dictionary<string, NodeArvoreTemplates_old>();
        }

        public Contracts_outroModo(string[] arrstr)
        {
            listaTemplates = new Dictionary<string, NodeArvoreTemplates_old>();

            for (int i = 0; i < arrstr.Length; i++)
            {
                var s = arrstr[i];
                AddTemplate(s);
            }
        }

        public bool IsParteVariavel(string s)
        {
            //return ((s.Length > 0) && (s[0] == '?') && (s[s.Length-1] == '?'));
            return ((s.Length > 0) && (s[0] == '?'));
        }

        public bool IsParteConstante(string s)
        {
            return (!IsParteVariavel(s));
        }

        public void AddTemplate(string s)
        {
            if (listaTemplates.ContainsKey(s)) return;
            var arrstr = s.Split('/');
            NodeArvoreTemplates_old n;
        }
    }
}
