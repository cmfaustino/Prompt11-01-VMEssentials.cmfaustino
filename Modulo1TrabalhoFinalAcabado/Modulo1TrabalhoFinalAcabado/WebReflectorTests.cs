using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace WebReflector
{
    [TestFixture]
    class WebReflectorTests
    {
        //[Test]
        //public void OLD_addtemplate_nao_da_excepcao()
        //{
        //    var c = new Contracts_old();
        //    Assert.DoesNotThrow(() => c.AddTemplate("/"));
        //}

        [Test]
        public void add_rule_raiz_fica_rules_count_igual_a_1()
        {
            var c = new Contracts();
            c.AddRule("/", new RootHandler());
            Assert.AreEqual(c.Testes_GetRulesCount(), 1);
        }

        [Test]
        public void add_rule_espaco_fica_rules_count_igual_a_1()
        {
            var c = new Contracts();
            c.AddRule(" ", new RootHandler());
            Assert.AreEqual(c.Testes_GetRulesCount(), 1);
        }

        [Test]
        public void add_rule_vazia_fica_rules_count_igual_a_0()
        {
            var c = new Contracts();
            c.AddRule("", new RootHandler());
            Assert.AreEqual(c.Testes_GetRulesCount(), 0);
        }

        [Test]
        public void add_rule_raiz_espaco_fica_rules_count_igual_a_1()
        {
            var c = new Contracts();
            c.AddRule("/ ", new RootHandler());
            Assert.AreEqual(c.Testes_GetRulesCount(), 1);
        }

        [Test]
        public void add_rule_raiz_2x_fica_rules_count_igual_a_1()
        {
            var c = new Contracts();
            c.AddRule("/", new RootHandler());
            c.AddRule("/", new RootHandler());
            Assert.AreEqual(c.Testes_GetRulesCount(), 1);
        }

        [Test]
        public void add_rules_varias_especiais_fica_rules_count_igual_a_5()
        {
            var c = new Contracts();
            c.AddRule("", new RootHandler()); // regra vazia sera ignorada
            c.AddRule(" ", new RootHandler()); // regras nao vazias serao adicionadas
            c.AddRule("/", new RootHandler());
            c.AddRule("/ ", new RootHandler());
            c.AddRule(" / ", new RootHandler());
            c.AddRule(" a ", new RootHandler());
            c.AddRule("", new RootHandler()); // regra vazia sera ignorada
            c.AddRule(" ", new RootHandler()); // regras duplicadas serao ignoradas
            c.AddRule("/", new RootHandler());
            c.AddRule("/ ", new RootHandler());
            c.AddRule(" / ", new RootHandler());
            c.AddRule(" a ", new RootHandler());
            Assert.AreEqual(c.Testes_GetRulesCount(), 5);
        }

        [Test]
        public void isvariable_ctx_com_chavetas_fica_true()
        {
            var c = new Contracts();
            Assert.True(c.Testes_IsVariable("{ctx}"));
        }

        [Test]
        public void isvariable_ctx_sem_chaveta_direita_final_fica_false()
        {
            var c = new Contracts();
            Assert.False(c.Testes_IsVariable("{ctx"));
        }

        [Test]
        public void isvariable_ctx_sem_chaveta_esquerda_inicial_fica_false()
        {
            var c = new Contracts();
            Assert.False(c.Testes_IsVariable("ctx}"));
        }

        [Test]
        public void isvariable_ctx_sem_chavetas_fica_false()
        {
            var c = new Contracts();
            Assert.False(c.Testes_IsVariable("ctx"));
        }

        [Test]
        public void isvariable_vazia_fica_false()
        {
            var c = new Contracts();
            Assert.False(c.Testes_IsVariable(""));
        }

        [Test]
        public void match_entre_regra_ctx_e_uri_ola_ambos_ok_fica_handler_nao_nulo()
        {
            var c = new Contracts();
            IHandler h = new NamespacePrefixContextHandler();
            c.AddRule("/{ctx}", h);
            IHandler hres;
            List<string> sres;
            c.Testes_Match("/ola", out sres, out hres);
            Assert.NotNull(hres);
        }

        [Test]
        public void resolve_entre_regra_ctx_e_uri_ola_ambos_ok_fica_handler_esperado_igual_ao_retornado()
        {
            var c = new Contracts();
            IHandler h = new NamespacePrefixContextHandler();
            c.AddRule("/{ctx}", h);
            IHandler hres;
            var d = c.ResolveUri("/ola", out hres);
            Assert.AreEqual(h, hres);
        }

        [Test]
        public void resolve_entre_regra_ctx_e_uri_ola_com_subdir_a_ambos_com_barra_antes_fica_handler_esperado_diferente_do_retornado_nulo()
        {
            var c = new Contracts();
            IHandler h = new NamespacePrefixContextHandler();
            c.AddRule("/{ctx}", h);
            IHandler hres;
            var d = c.ResolveUri("/ola/a", out hres);
            Assert.Null(hres);
            Assert.AreNotEqual(h, hres);
        }

        [Test]
        public void resolve_entre_regras_ctx_e_assemblyName_e_uris_ok_ficam_handlers_esperados_iguais_ao_retornados_e_ctx_ola_assemblyName_adeus()
        {
            var c = new Contracts();
            IHandler hroot = new RootHandler();
            IHandler hcontext = new ContextNameHandler();
            IHandler hnamespace = new NamespacePrefixContextHandler();
            IHandler hassembly = new AssemblyNameContextHandler();
            c.AddRule("/", hroot);
            c.AddRule("/ ", hnamespace); // exemplo de um terceiro handler, apenas para efeitos de testes
            c.AddRule("/{ctx}", hcontext); // colocar o esperado em ultimo na lista de regrashandlers
            IHandler hres, hres2;
            var d = c.ResolveUri("/ola", out hres);
            Assert.NotNull(hres);
            Assert.AreEqual(hcontext, hres);
            Assert.AreEqual("ctx", d.First().Key);
            Assert.AreEqual("ola", d.First().Value);
            c.AddRule("/{ctx}/as/{assemblyName}", hassembly); // colocar o esperado em ultimo na lista de regrashandlers
            var d2 = c.ResolveUri("/ola/as/adeus", out hres2);
            Assert.NotNull(hres2);
            Assert.AreEqual(hassembly, hres2);
            Assert.AreEqual("ctx", d2.First().Key);
            Assert.AreEqual("ola", d2.First().Value);
            Assert.AreEqual("assemblyName", d2.Last().Key);
            Assert.AreEqual("adeus", d2.Last().Value);
        }

        [Test]
        public void string_vazia_endswith_vazia_fica_true()
        {
            Assert.True("".EndsWith(""));
        }

        [Test]
        public void string_nao_vazia_endswith_vazia_fica_true()
        {
            Assert.True(" ".EndsWith(""));
        }

        [Test]
        public void isconstant_ns_com_doispontos_no_inicio_e_fim_fica_true()
        {
            var c = new Contracts();
            Assert.True(c.Testes_IsConstant(":ns:"));
        }

        [Test]
        public void isconstant_ns_sem_doispontos_direito_final_fica_true()
        {
            var c = new Contracts();
            //Assert.False(c.Testes_IsConstant(":ns"));
            Assert.True(c.Testes_IsConstant(":ns"));
        }

        [Test]
        public void isconstant_ns_sem_doispontos_esquerdo_inicial_fica_true()
        {
            var c = new Contracts();
            //Assert.False(c.Testes_IsConstant("ns:"));
            Assert.True(c.Testes_IsConstant("ns:"));
        }

        [Test]
        public void isconstant_ns_sem_doispontos_nem_no_inicio_nem_no_fim_fica_true()
        {
            var c = new Contracts();
            //Assert.False(c.Testes_IsConstant("ns"));
            Assert.True(c.Testes_IsConstant("ns"));
        }

        [Test]
        public void isconstant_vazia_fica_false()
        {
            var c = new Contracts();
            Assert.False(c.Testes_IsConstant(""));
        }

        [Test]
        public void type_delimiter_e_ponto_sao_iguais()
        {
            Assert.AreEqual(Type.Delimiter, '.');
        }

        [Test]
        public void typeof_string_Name_e_String_sao_iguais()
        {
            Assert.AreEqual(typeof(string).Name, "String");
        }
    }
}
