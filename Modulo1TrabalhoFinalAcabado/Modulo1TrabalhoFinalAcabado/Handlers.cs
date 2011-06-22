using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Net;

namespace WebReflector
{
    public static class Handlers
    {
        //public const string DIRECTORIA_RAIZ = @"C:\Program Files";
        public const string DIRECTORIA_RAIZ =
            @"C:\z_PROMPT\PROMPT\uc01\repos\Prompt11-01-VMEssentials.cmfaustino\packages\NUnit.2.5.10.11092\tools"; // tem sub-pasta lib
            //@"C:\z_PROMPT\PROMPT\uc02\repos\PROMPT11-02-AdvancedProgramming\src\packages\NUnit.2.5.10.11092\tools"; // tem sub-pasta lib
        //@"C:\z_PROMPT\PROMPT\uc02\repos\PROMPT11-02-AdvancedProgramming\src\Mod02-AdvProgramming.Utils\bin\Debug";
        //@"C:\z_PROMPT\PROMPT\uc02\repos\PROMPT11-02-AdvancedProgramming\src";
        public const string URL_WEBSITE_PROCESSAMENTO = ""; // @"http://";
    }

    #region Componente Auxiliar - Classe Nulls Handler

    public class NullsHandler : IHandler
    {
        private readonly string uriPedido;
        private readonly bool IsHandlerNull_ElseContractsNull;

        public NullsHandler(string uriPedido, bool isHandlerNullElseContractsNull)
        {
            this.uriPedido = uriPedido ?? "";
            this.IsHandlerNull_ElseContractsNull = isHandlerNullElseContractsNull;
        }

        public HandlerAnswer Handle()
        {
            Console.WriteLine("...NullsHandler - sem parâmetros...");

            if (IsHandlerNull_ElseContractsNull)
            { // HTTP status 400 = BadRequest (request could not be understood by the server)
                return new HandlerAnswer
                           {
                               answerOk = false,
                               answerCode = HttpStatusCode.BadRequest,
                               answerContent =
                                   Views.HTML(Views.HEAD(Views.TITLE("Propriedades de endereço: " + uriPedido)),
                                              Views.BODY(Views.H1("Input = endereço de formato desconhecido:"),
                                                         Views.H3(uriPedido),
                                                         Views.P(
                                                             "Handler nulo, não há template correspondente ao endereço indicado!")
                                                  )
                                   )
                           };
            }
            else
            { // HTTP status 501 = NotImplemented (the server does not support the requested function)
                return new HandlerAnswer
                           {
                               answerOk = false,
                               answerCode = HttpStatusCode.NotImplemented,
                               answerContent =
                                   Views.HTML(Views.HEAD(Views.TITLE("Propriedades de endereço: " + uriPedido)),
                                              Views.BODY(Views.H1("Input = endereço de formato desconhecido:"),
                                                         Views.H3(uriPedido),
                                                         Views.P(
                                                             "Lista de Templates nula, não há template correspondente ao endereço indicado!")
                                                  )
                                   )
                           };
            }
        }

        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...NullsHandler...");

            return this.Handle();
        }
    }

    #endregion

    #region 5 Especificacoes -Enunciado- RootHandler ContextNameHandler AssembliesContextHandler NamespacesContextHandler AssemblyNameContextHandler

    public class RootHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...RootHandler...");

            var tagsHTMLnested = new List<TagHTML>();
            var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ);
            bool isDataAvailable = false;
            foreach (var subdir in dir.EnumerateDirectories()) // GetDirectories()
            {
                if (!isDataAvailable)
                {
                    isDataAvailable = true;
                }
                tagsHTMLnested.Add(
                    Views.TR(
                        Views.TD(Views.A(true, string.Format("{0}/{1}", Handlers.URL_WEBSITE_PROCESSAMENTO, subdir.Name),
                                         Views.Text2TagHTML(string.Format("Contexto {0}", subdir.Name))
                                     ))
                    ));
            }
            if (!isDataAvailable)
            {
                tagsHTMLnested.Add(
                    Views.TR(
                        Views.TD(Views.Text2TagHTML("Não há contextos (sub-directorias) na directoria raiz!"))
                    ));
            }
            var tagsHTMLfull = Views.HTML(
                Views.HEAD(Views.TITLE("Propriedades de raiz: /")),
                Views.BODY(Views.H1("Input = raiz: /"), Views.P(string.Format("(<=> {0})", dir.FullName)),
                           Views.TABLE(tagsHTMLnested.ToArray())
                    )
                );
            // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
            return new HandlerAnswer() {answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull};
        }
    }

    public class ContextNameHandler : IHandler // se contexto pode conter vários níveis de pastas, então, o que impede .../ns de ser contexto , por ex. ?
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...ContextNameHandler...");

            TagHTML tagsHTMLfull;
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                tagsHTMLfull = Views.HTML(
                    Views.HEAD(Views.TITLE(string.Format("Propriedades de contexto: /{0}", contextname))),
                    Views.BODY(Views.H1(string.Format("Input = contexto: /{0}", contextname)),
                               Views.P(string.Format("(<=> {0})", dir.FullName)),
                               Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link / ]"))),
                               Views.TABLE(
                                   Views.TR(
                                       Views.TD(Views.A(true,
                                                        string.Format("{0}/{1}/as", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                                      contextname),
                                                        Views.Text2TagHTML(string.Format("Assemblies do Contexto {0}",
                                                                                         contextname))
                                                    ))),
                                   Views.TR(
                                       Views.TD(Views.A(true,
                                                        string.Format("{0}/{1}/ns", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                                      contextname),
                                                        Views.Text2TagHTML(string.Format("Namespaces do Contexto {0}",
                                                                                         contextname))
                                                    )))
                                   )
                        )
                    );
                // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
            }
            tagsHTMLfull = Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de contexto")),
                                      Views.BODY(Views.H1("Erro nas Propriedades de contexto:"),
                                                 Views.H3("Handler da Regra - ctx"),
                                                 Views.P("Não se obteve valor do contexto,"),
                                                 Views.P("apesar de haver mapeamento com ctx !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]")))
                                          ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    public class AssembliesContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...AssembliesContextHandler...");

            TagHTML tagsHTMLfull;
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                var listAssembliesNamesOfContext = new List<string>();
                var tagsHTMLnested = new List<TagHTML>();
                var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                dir.GetAssembliesNamesFromDir(contextname, listAssembliesNamesOfContext, tagsHTMLnested); // povoar
                listAssembliesNamesOfContext.Sort(); // ordenacao alfabetica de strings
                if (listAssembliesNamesOfContext.Count > 0)
                {
                    foreach (var assemblyNameOfContext in listAssembliesNamesOfContext)
                    {
                        tagsHTMLnested.Add(
                            Views.TR(
                                Views.TD(Views.A(true,
                                                 string.Format("{0}/{1}/as/{2}", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                               contextname, assemblyNameOfContext),
                                                 Views.Text2TagHTML(string.Format("Assembly {1} do Contexto {0}",
                                                                                  contextname, assemblyNameOfContext))
                                             )))
                            );
                    }
                }
                else
                {
                    tagsHTMLnested.Add(
                        Views.TR(
                            Views.TD(Views.Text2TagHTML("Não há ficheiros (assemblies ou outros) no contexto especificado!"))
                        ));
                }
                tagsHTMLfull = Views.HTML(
                    Views.HEAD(Views.TITLE(string.Format("Propriedades de lista de assemblies de contexto: /{0}/as",
                                                         contextname))),
                    Views.BODY(Views.H1(string.Format("Input = lista de assemblies de contexto: /{0}/as", contextname)),
                               Views.P(string.Format("(<=> {0})", dir.FullName)),
                               Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                               Views.P(Views.A(true, string.Format("/{0}", contextname),
                                               Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                               Views.TABLE(tagsHTMLnested.ToArray())
                        )
                    );
                // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
            }
            contextname = contextname ?? "";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de lista de assemblies de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de lista de assemblies de contexto:"),
                                      Views.H3("Handler da Regra - ctx - as"),
                                      Views.P("Não se obteve valor do contexto,"),
                                      Views.P("apesar de haver mapeamento com ctx !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname))))
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    public class NamespacesContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...NamespacesContextHandler...");

            TagHTML tagsHTMLfull;
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                var listNamespacesOfContext = new List<string>();
                var tagsHTMLnested = new List<TagHTML>();
                var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                var listAssembliesOfContext = new List<Assembly>();
                dir.GetAssembliesFromDir(contextname, listAssembliesOfContext, tagsHTMLnested, true); // povoar
                foreach (var assemblyNamespace in listAssembliesOfContext.GetNamespacesFromAssemblies(false, true, true))
                {
                    if (!listNamespacesOfContext.Contains(assemblyNamespace))
                    {
                        listNamespacesOfContext.Add(assemblyNamespace);
                    }
                }
                listNamespacesOfContext.Sort(); // ordenacao alfabetica de strings
                if (listNamespacesOfContext.Count > 0)
                {
                    foreach (var namespaceOfContext in listNamespacesOfContext)
                    {
                        tagsHTMLnested.Add(
                            Views.TR(
                                Views.TD(Views.A(true,
                                                 string.Format("{0}/{1}/ns/{2}", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                               contextname,
                                                               namespaceOfContext.IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                   ()),
                                                 Views.Text2TagHTML(string.Format("Namespace \"{1}\" do Contexto {0}",
                                                                                  contextname, namespaceOfContext))
                                             )))
                            );
                    }
                }
                else
                {
                    tagsHTMLnested.Add(
                        Views.TR(
                            Views.TD(Views.Text2TagHTML("Não há namespaces no contexto especificado!"))
                        ));
                }
                tagsHTMLfull = Views.HTML(
                    Views.HEAD(Views.TITLE(string.Format("Propriedades de lista de namespaces de contexto: /{0}/ns",
                                                         contextname))),
                    Views.BODY(Views.H1(string.Format("Input = lista de namespaces de contexto: /{0}/ns", contextname)),
                               Views.P(string.Format("(<=> {0})", dir.FullName)),
                               Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                               Views.P(Views.A(true, string.Format("/{0}", contextname),
                                               Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                               Views.TABLE(tagsHTMLnested.ToArray())
                        )
                    );
                // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
            }
            contextname = contextname ?? "";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de lista de namespaces de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de lista de namespaces de contexto:"),
                                      Views.H3("Handler da Regra - ctx - ns"),
                                      Views.P("Não se obteve valor do contexto,"),
                                      Views.P("apesar de haver mapeamento com ctx !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname))))
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    public class AssemblyNameContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...AssemblyNameContextHandler...");

            TagHTML tagsHTMLfull;
            string messageRangeOfError = null;
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string assemblyname;
                if (prms.TryGetValue("assemblyName", out assemblyname))
                {
                    var tagsHTMLnested = new List<TagHTML>();
                    var fileOfDir = new FileInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname + "/" + assemblyname);
                    Assembly assemblyOfFileOfDir;
                    try
                    {
                        assemblyOfFileOfDir = fileOfDir.FullName.LoadAndGetAssembly();

                        tagsHTMLnested.Add(Views.TR(
                            Views.TD(Views.Text2TagHTML(string.Format("Frindly Name do Assembly {1} do Contexto {0}",
                                                                      contextname, assemblyname))),
                            Views.TD(Views.Text2TagHTML(string.Format(" = {0}",
                                                                      assemblyOfFileOfDir.GetFriendlyNameFromAssembly())))
                                               ));
                        tagsHTMLnested.Add(Views.TR(
                            Views.TD(Views.Text2TagHTML(string.Format("Public Key do Assembly {1} do Contexto {0}",
                                                                      contextname, assemblyname))),
                            Views.TD(Views.Text2TagHTML(string.Format(" = {0}",
                                                                      assemblyOfFileOfDir.GetPublicKeyStringFromAssembly())))
                                               ));
                        var dictNamespacesTypeNamesOfAssembly =
                            assemblyOfFileOfDir.GetDictNamespacesTypeFullNamesFromAssembly(false, true);
                        if (dictNamespacesTypeNamesOfAssembly.Count > 0)
                        {
                            foreach (var pairsOfTypeOfAssembly in dictNamespacesTypeNamesOfAssembly.OrderBy(elem => elem.Key))
                            {
                                var namespaceOfTypeOfAssembly = pairsOfTypeOfAssembly.Key;
                                var listTypeNamesOfAssembly = pairsOfTypeOfAssembly.Value;
                                if (listTypeNamesOfAssembly.Count > 0)
                                {
                                    tagsHTMLnested.Add(Views.TR(
                                        Views.TD(Views.A(true,
                                                         string.Format("{0}/{1}/ns/{2}", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                                       contextname,
                                                                       namespaceOfTypeOfAssembly.
                                                                           IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                                         Views.Text2TagHTML(
                                                             string.Format("Namespace \"{1}\" do Contexto {0}",
                                                                           contextname, namespaceOfTypeOfAssembly))
                                                     )),
                                        Views.TD(
                                            Views.Text2TagHTML(string.Format(" contém {0} tipo(s):",
                                                                             listTypeNamesOfAssembly.Count)))
                                                           ));
                                    listTypeNamesOfAssembly.Sort();
                                    foreach (var nameOfTypeOfAssembly in listTypeNamesOfAssembly)
                                    {
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.Text2TagHTML(
                                                string.Format("Namespace = \"{1}\"",
                                                              contextname, namespaceOfTypeOfAssembly, nameOfTypeOfAssembly))),
                                            Views.TD(Views.A(true,
                                                             string.Format("{0}/{1}/ns/{2}/{3}",
                                                                           Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                                           contextname,
                                                                           namespaceOfTypeOfAssembly.
                                                                               IsNameOfNamespaceEmptyThenConvertToSpecial(),
                                                                           nameOfTypeOfAssembly),
                                                             Views.Text2TagHTML(
                                                                 string.Format("Tipo {2} do Namespace \"{1}\" do Contexto {0}",
                                                                               contextname, namespaceOfTypeOfAssembly,
                                                                               nameOfTypeOfAssembly))
                                                         ))
                                                               ));
                                    }
                                }
                                else
                                {
                                    tagsHTMLnested.Add(Views.TR(
                                        Views.TD(Views.A(true,
                                                         string.Format("{0}/{1}/ns/{2}", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                                       contextname,
                                                                       namespaceOfTypeOfAssembly.
                                                                           IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                                         Views.Text2TagHTML(
                                                             string.Format("Namespace \"{1}\" do Contexto {0}",
                                                                           contextname, namespaceOfTypeOfAssembly))
                                                     )),
                                        Views.TD(
                                            Views.Text2TagHTML(string.Format(" não contém quaisquer tipo(s)!",
                                                                             listTypeNamesOfAssembly.Count)))
                                                           ));
                                }
                            }
                        }
                        else
                        {
                            tagsHTMLnested.Add(Views.TR(
                                Views.TD(Views.Text2TagHTML(
                                    string.Format("Assembly \"{1}\" do Contexto {0}",
                                                  contextname, assemblyname))),
                                Views.TD(
                                    Views.Text2TagHTML(string.Format(" não contém quaisquer namespace(s)!",
                                                                     dictNamespacesTypeNamesOfAssembly.Count)))
                                                   ));
                        }
                    }
                    catch (PathTooLongException pe)
                    {
                        tagsHTMLnested.Add(Views.TR(
                            Views.TD(
                                Views.Text2TagHTML(string.Format(
                                    "Assembly {1} do Contexto {0} tem caminho muito longo!", contextname, fileOfDir.Name))),
                            Views.TD(
                                Views.Text2TagHTML(string.Format("Excepção {3}: {1} !!!", pe.Source, pe.Message,
                                                                 pe.StackTrace, pe.GetType().FullName)))
                                               ));
                    }
                    catch (Exception e)
                    {
                        tagsHTMLnested.Add(Views.TR(
                            Views.TD(
                                Views.Text2TagHTML(
                                    string.Format("Assembly {1} do Contexto {0} é inválido como assembly,"
                                                  + " ou inexistente, ou não tem permissões de acesso/leitura!",
                                                  contextname, fileOfDir.Name))),
                            Views.TD(
                                Views.Text2TagHTML(string.Format("Excepção {3}: {1} !!!", e.Source, e.Message,
                                                                 e.StackTrace, e.GetType().FullName)))
                                               ));
                        //throw;
                    }
                    tagsHTMLfull = Views.HTML(
                        Views.HEAD(Views.TITLE(string.Format("Propriedades de assembly de contexto: /{0}/as/{1}",
                                                             contextname, assemblyname))),
                        Views.BODY(Views.H1(string.Format("Input = lista de informações de assembly de contexto: /{0}/as/{1}",
                                                             contextname, assemblyname)),
                                   Views.P(string.Format("(<=> {0}/{1}/as/{2})", Handlers.DIRECTORIA_RAIZ, contextname, assemblyname)),
                                   Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                   Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                   Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                   Views.P(Views.A(true, string.Format("/{0}/as", contextname),
                                                   Views.Text2TagHTML(string.Format("[ link assemblies contexto /{0}/as ]", contextname)))),
                                   Views.TABLE(tagsHTMLnested.ToArray())
                            )
                        );
                    // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                    return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
                }
                assemblyname = assemblyname ?? "";
                messageRangeOfError = "assembly";
            }
            contextname = contextname ?? "";
            messageRangeOfError = messageRangeOfError ?? "contexto";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de assembly de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de assembly de contexto:"),
                                      Views.H3("Handler da Regra - ctx - as - assemblyName"),
                                      Views.P(string.Format("Não se obteve valor do {0},", messageRangeOfError)),
                                      Views.P("apesar de haver mapeamento com ctx e assemblyName !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                      Views.P(Views.A(true, string.Format("/{0}/as", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link assemblies contexto /{0}/as ]", contextname))))
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    #endregion

    #region 1 Especificacao -Enunciado- NamespacePrefixContextHandler

    public class NamespacePrefixContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...NamespacePrefixContextHandler...");

            TagHTML tagsHTMLfull;
            string messageRangeOfError = null;
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string namespacePrefix;
                if (prms.TryGetValue("namespacePrefix", out namespacePrefix))
                {
                    namespacePrefix = namespacePrefix.IsNameOfNamespaceSpecialThenConvertToEmpty(); // conversao interna de normalizacao
                    var tagsHTMLnested = new List<TagHTML>();
                    var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                    var listAssembliesOfContext = new List<Assembly>();
                    dir.GetAssembliesFromDir(contextname, listAssembliesOfContext, tagsHTMLnested, false); // povoar
                    var dictNamespacesTypeNamesOfContext =
                        listAssembliesOfContext.ToArray().GetDictNamespacesTypeFullNamesFromAssemblies(false, true).
                            OrderBy(elem => elem.Key).ToDictionary(elem => elem.Key, elem => elem.Value);
                    string namespaceOfContext;
                    List<string> listTypeNamesOfNamespace;
                    Dictionary<string, List<string>> dictNames;
                    string messageItems;
                    string messageItem;
                    string messageRelationOfItem;
                    for (int i = 0; i < 2; i++) // hierarquia de namespaces - namespaces parents e sub-namespaces
                    {
                        switch (i)
                        {
                            case 0: // namespaces parents
                                dictNames =
                                    dictNamespacesTypeNamesOfContext.GetDictParentNamespacesTypeFullNamesFromNamespace(
                                        namespacePrefix);
                                messageItems = "Namespaces acima";
                                messageItem = "Namespace";
                                messageRelationOfItem = "acima";
                                break;
                            default: // sub-namespaces
                                dictNames =
                                    dictNamespacesTypeNamesOfContext.GetDictSubNamespacesTypeFullNamesFromNamespace(
                                        namespacePrefix);
                                messageItems = "Sub-Namespaces (isto é, abaixo)";
                                messageItem = "Sub-Namespace";
                                messageRelationOfItem = "abaixo";
                                break;
                        }
                        if (dictNames.Count > 0)
                        {
                            tagsHTMLnested.Add(Views.TR(
                                Views.TD(Views.P(Views.Text2TagHTML(
                                    string.Format("Lista de {2} do Namespace \"{1}\" no Contexto {0} :",
                                                  contextname, namespacePrefix, messageItems)))
                                    )));
                            foreach (var pairsOfContext in dictNames)
                            {
                                namespaceOfContext = pairsOfContext.Key;
                                listTypeNamesOfNamespace = pairsOfContext.Value;
                                tagsHTMLnested.Add(Views.TR(
                                    Views.TD(Views.A(true,
                                                     string.Format("{0}/{1}/ns/{2}", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                                   contextname,
                                                                   namespaceOfContext.
                                                                       IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                       ()),
                                                     Views.Text2TagHTML(
                                                         string.Format(
                                                             "{3} \"{1}\" {4} do Namespace \"{2}\" no Contexto {0}",
                                                             contextname, namespaceOfContext, namespacePrefix,
                                                             messageItem, messageRelationOfItem))
                                                 ),
                                             Views.Text2TagHTML(string.Format(" contém {0} tipo(s).",
                                                                              listTypeNamesOfNamespace.Count)))
                                                       ));
                            }
                        }
                        else
                        {
                            tagsHTMLnested.Add(Views.TR(
                                Views.TD(Views.P(Views.Text2TagHTML(
                                    string.Format("Não há {2} do Namespace \"{1}\" no Contexto {0} !",
                                                  contextname, namespacePrefix, messageItems)))
                                    )));
                        }
                    }
                    // o proprio namespace
                    bool isDataAvailable = false;
                    if (dictNamespacesTypeNamesOfContext.ContainsKey(namespacePrefix))
                    {
                        isDataAvailable = true;
                        namespaceOfContext = namespacePrefix;
                        listTypeNamesOfNamespace = dictNamespacesTypeNamesOfContext[namespacePrefix];
                        if (listTypeNamesOfNamespace.Count > 0)
                        {
                            tagsHTMLnested.Add(Views.TR(
                                Views.TD(Views.P(
                                    Views.Text2TagHTML(string.Format("Namespace requerido \"{1}\" no Contexto {0}",
                                                                     contextname, namespaceOfContext)),
                                    Views.Text2TagHTML(string.Format(" contém {0} tipo(s) :",
                                                                     listTypeNamesOfNamespace.Count)))
                                    )));
                            listTypeNamesOfNamespace.Sort();
                            foreach (var nameOfTypeOfNamespace in listTypeNamesOfNamespace)
                            {
                                tagsHTMLnested.Add(Views.TR(
                                    Views.TD(Views.A(true,
                                                     string.Format("{0}/{1}/ns/{2}/{3}",
                                                                   Handlers.URL_WEBSITE_PROCESSAMENTO, contextname,
                                                                   namespaceOfContext.
                                                                       IsNameOfNamespaceEmptyThenConvertToSpecial(),
                                                                   nameOfTypeOfNamespace),
                                                     Views.Text2TagHTML(
                                                         string.Format(
                                                             "Tipo {2} do Namespace \"{1}\" do Contexto {0}",
                                                             contextname, namespaceOfContext,
                                                             nameOfTypeOfNamespace))
                                                 ))
                                                       ));
                            }
                        }
                        else
                        {
                            tagsHTMLnested.Add(Views.TR(
                                Views.TD(Views.P(
                                    Views.Text2TagHTML(string.Format("O Namespace requerido \"{1}\" no Contexto {0}",
                                                                     contextname, namespaceOfContext)),
                                    Views.Text2TagHTML(string.Format(" não contém quaisquer tipo(s)!",
                                                                     listTypeNamesOfNamespace.Count)))
                                    )));
                        }
                    }
                    else
                    {
                        tagsHTMLnested.Add(Views.TR(
                            Views.TD(Views.P(Views.Text2TagHTML(
                                string.Format("Não existe o Namespace requerido \"{1}\" no Contexto {0} !",
                                              contextname, namespacePrefix)))
                                )));
                    }
                    tagsHTMLfull = Views.HTML(
                        Views.HEAD(Views.TITLE(string.Format("Propriedades de namespace de contexto: /{0}/ns/{1}",
                                                             contextname, namespacePrefix))),
                        Views.BODY(Views.H1(string.Format("Input = lista de informações de namespace de contexto: /{0}/ns/{1}",
                                                             contextname, namespacePrefix)),
                                   Views.P(string.Format("(<=> {0}/{1}/ns/{2})", Handlers.DIRECTORIA_RAIZ, contextname, namespacePrefix)),
                                   Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                   Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                   Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                   Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                   Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                   Views.TABLE(tagsHTMLnested.ToArray())
                            )
                        );
                    if (isDataAvailable)
                    {
                        // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                        return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
                    }
                    else
                    {
                        // HTTP status 404 = NotFound (the requested resource does not exist on the server)
                        return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.NotFound, answerContent = tagsHTMLfull };
                    }
                }
                namespacePrefix = namespacePrefix ?? "";
                messageRangeOfError = "namespace";
            }
            contextname = contextname ?? "";
            messageRangeOfError = messageRangeOfError ?? "contexto";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de namespace de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de namespace de contexto:"),
                                      Views.H3("Handler da Regra - ctx - ns - namespacePrefix"),
                                      Views.P(string.Format("Não se obteve valor do {0},", messageRangeOfError)),
                                      Views.P("apesar de haver mapeamento com ctx e namespacePrefix !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                      Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname))))
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    #endregion

    #region 3 Especificacoes -Enunciado- TypeShortNameNamespaceContext... MethodNameTypeNamespaceContext... ConstructsTypeNamespaceContext...

    public class TypeShortNameNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...TypeShortNameNamespaceContextHandler...");

            TagHTML tagsHTMLfull;
            string messageRangeOfError = null;
            TagHTML tagHTMLextra = Views.Text2TagHTML("");
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string namespaceName;
                if (prms.TryGetValue("namespace", out namespaceName))
                {
                    namespaceName = namespaceName.IsNameOfNamespaceSpecialThenConvertToEmpty(); // conversao interna de normalizacao
                    string typeShortName;
                    if (prms.TryGetValue("shortName", out typeShortName))
                    {
                        var tagsHTMLnested = new List<TagHTML>();
                        var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                        var listAssembliesOfContext = new List<Assembly>();
                        dir.GetAssembliesFromDir(contextname, listAssembliesOfContext, tagsHTMLnested, false); // povoar
                        var dictNamespacesTypeNamesOfContext =
                            listAssembliesOfContext.ToArray().GetDictNamespacesTypeFullNamesFromAssemblies(false, true).
                                OrderBy(elem => elem.Key).ToDictionary(elem => elem.Key, elem => elem.Value);
                        string namespaceOfContext;
                        List<string> listTypeNamesOfNamespace;
                        // o proprio namespace
                        bool isDataAvailable = false;
                        if (dictNamespacesTypeNamesOfContext.ContainsKey(namespaceName))
                        {
                            namespaceOfContext = namespaceName;
                            listTypeNamesOfNamespace = dictNamespacesTypeNamesOfContext[namespaceName];
                            if (listTypeNamesOfNamespace.Count > 0)
                            {
                                if (listTypeNamesOfNamespace.Contains(typeShortName))
                                {
                                    var listTypesEqualType =
                                        listAssembliesOfContext.GetTypesFromAssemblies(false, true, true).Where(
                                            t => t.GetFullNameFromType().Equals(typeShortName));
                                    if (listTypesEqualType.Count() > 0)
                                    {
                                        isDataAvailable = true;
                                        var tipo = listTypesEqualType.First();
                                        var tipoFullName = tipo.GetFullNameFromType();
                                        var tipoNamespace = tipo.GetNamespaceFromType();
                                        var tipoDeclType = tipo.DeclaringTypeByCatchingAnyException();
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.Text2TagHTML(
                                                string.Format("Type Full Name do Namespace {1} do Contexto {0} :",
                                                              contextname, namespaceOfContext))),
                                            Views.TD(Views.Text2TagHTML(string.Format(" = {0}", tipoFullName)))
                                                               ));
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                "UnderlyingSystemType Full Name do Namespace {1} do Contexto {0} :",
                                                contextname, namespaceOfContext))),
                                            Views.TD(Views.Text2TagHTML(
                                                string.Format(" = {0}", tipo.UnderlyingSystemType.GetFullNameFromType())))
                                                               ));
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                "Características do Type do Namespace {1} do Contexto {0} :",
                                                contextname, namespaceOfContext, tipoFullName))),
                                            Views.TD(Views.Text2TagHTML(" Visible = " + tipo.IsVisible +
                                                                        " ;" +
                                                                        (tipo.IsPublic ? " Public" : "") +
                                                                        (tipo.IsNotPublic ? " NotPublic" : "") +
                                                                        (tipo.IsAbstract ? " Abstract" : "") +
                                                                        (tipo.IsSealed ? " Sealed" : "") +
                                                                        (tipo.IsPrimitive ? " Primitive" : "") +
                                                                        (tipo.IsPointer ? " Pointer" : "") +
                                                                        (tipo.IsEnum ? " Enum" : "") +
                                                                        (tipo.IsGenericType ? " GenericType" : "") +
                                                                        (tipo.IsClass ? " Class_RefType" : "") +
                                                                        (tipo.IsValueType ? " ValueType" : "") +
                                                                        (tipo.IsInterface ? " Interface" : "") +
                                                                        (tipo.IsSpecialName ? " SpecialName" : "")
                                                         ))
                                                               ));
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                "Nome Simples do Assembly do Type {0}", tipoFullName))),
                                            Views.TD(
                                                Views.Text2TagHTML(string.Format(" = {0}", tipo.Assembly.GetName().Name)))
                                                               ));
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                "Nome Completo do Assembly do Type {0}", tipoFullName))),
                                            Views.TD(Views.Text2TagHTML(string.Format(" = {0}", tipo.Assembly.FullName)))
                                                               ));
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.Text2TagHTML(string.Format("Link do Módulo do Type {0} :",
                                                                                      tipoFullName))),
                                            Views.TD(Views.A(true,
                                                             string.Format("/{0}/as/{1}", contextname,
                                                                           tipo.Module.ScopeName),
                                                             Views.Text2TagHTML(string.Format(
                                                                 "Assembly {1} do Contexto {0}", contextname,
                                                                 tipo.Module.ScopeName))))
                                                               ));
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.Text2TagHTML(string.Format("Link do Namespace do Type {0} :",
                                                                                      tipoFullName))),
                                            Views.TD(Views.A(true,
                                                             string.Format("/{0}/ns/{1}", contextname,
                                                                           tipoNamespace.
                                                                               IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                               ()),
                                                             Views.Text2TagHTML(string.Format(
                                                                 "Namespace {1} do Contexto {0}", contextname,
                                                                 tipoNamespace))))
                                                               ));
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(
                                                Views.Text2TagHTML(string.Format("Type {0} é Nested?", tipoFullName))),
                                            Views.TD(
                                                Views.Text2TagHTML(string.Format(
                                                    " = {0}{1}", tipo.IsNestedByCatchingAnyException(),
                                                    ((tipoDeclType != null) ? (" ; de tipo ") : ""))),
                                                (tipoDeclType != null)
                                                    ? Views.A(true,
                                                              string.Format("/{0}/ns/{1}/{2}", contextname,
                                                                            tipoDeclType.GetNamespaceFromType().
                                                                                IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                                (), tipoDeclType.GetFullNameFromType()),
                                                              Views.Text2TagHTML(
                                                                  string.Format("{0}",
                                                                                tipoDeclType.GetFullNameFromType())))
                                                    : Views.Text2TagHTML(""),
                                                Views.Text2TagHTML(" ; Visible = " + tipo.IsVisible)
                                                )
                                                               ));
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                "Construtores de Type {0} :", tipoFullName)))),
                                            Views.TD(Views.A(true,
                                                             string.Format("/{0}/ns/{1}/{2}/c", contextname,
                                                                           tipoNamespace.
                                                                               IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                               (), tipoFullName),
                                                             Views.Text2TagHTML(
                                                                 string.Format(" [link de métodos construtores]",
                                                                               contextname, tipoNamespace,
                                                                               tipoFullName))))
                                                               ));
                                        // Type.GetNestedTypes - public
                                        var nestedTypes =
                                            tipo.GetNestedTypesByCatchingAnyException().Where(t => (t != null)).OrderBy(
                                                t => t.FullName);
                                        var nestedTypesCount = nestedTypes.Count(); // nestedTypes.Length;
                                        if (nestedTypesCount > 0)
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" tem {0} Nested Type(s) PUBLIC(s):", nestedTypesCount))))
                                                                   ));
                                            foreach (var nestedType in nestedTypes)
                                            {
                                                var nestedTypeFullName = nestedType.GetFullNameFromType();
                                                var nestedTypeNamespace = nestedType.GetNamespaceFromType();
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        "Nested Type PUBLIC:", nestedTypeFullName)))),
                                                    Views.TD(Views.A(true,
                                                                     string.Format("/{0}/ns/{1}/{2}", contextname,
                                                                                   nestedTypeNamespace.
                                                                                       IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                                       (), nestedTypeFullName),
                                                                     Views.Text2TagHTML(
                                                                         string.Format(" Type {2}",
                                                                                       contextname, nestedTypeNamespace,
                                                                                       nestedTypeFullName))))
                                                                       ));
                                            }
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" não tem quaisquer Nested Type(s) PUBLIC(s)!",
                                                                  nestedTypesCount))))
                                                                   ));
                                        }
                                        // Type.GetMethods - public
                                        var methods =
                                            tipo.GetMethods().Where(m => (m != null)).OrderBy(m => m.Name).Select(
                                                m => m.Name).Distinct();
                                        var methodsCount = methods.Count(); // methods.Length;
                                        if (methodsCount > 0)
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                    " tem {0} (famílias de) nomes diferentes de Method(s) PUBLIC(s):",
                                                    methodsCount))))
                                                                   ));
                                            foreach (var method in methods)
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        "Method PUBLIC:", method)))),
                                                    Views.TD(Views.A(true,
                                                                     string.Format("/{0}/ns/{1}/{2}/m/{3}", contextname,
                                                                                   tipoNamespace.
                                                                                       IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                                       (), tipoFullName, method),
                                                                     Views.Text2TagHTML(
                                                                         string.Format(" (família de) nome de Method {2}",
                                                                                       contextname, tipoNamespace,
                                                                                       method))))
                                                                       ));
                                            }
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" não tem quaisquer Method(s) PUBLIC(s)!",
                                                                  methodsCount))))
                                                                   ));
                                        }
                                        // Type.GetConstructors - public
                                        var constructors =
                                            tipo.GetConstructors().Where(c => (c != null)).OrderBy(c => c.Name).Select(
                                                c => c.Name).Distinct();
                                        var constructorsCount = constructors.Count(); // constructors.Length;
                                        if (constructorsCount > 0)
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                    " tem {0} (famílias de) nomes diferentes de Constructor(s) PUBLIC(s):",
                                                    constructorsCount))))
                                                                   ));
                                            foreach (var constructor in constructors)
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        "Constructor PUBLIC:", constructor)))),
                                                    Views.TD(Views.A(true,
                                                                     string.Format("/{0}/ns/{1}/{2}/c", contextname,
                                                                                   tipoNamespace.
                                                                                       IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                                       (), tipoFullName, constructor),
                                                                     Views.Text2TagHTML(
                                                                         string.Format(" (família de) nome de Constructor {2}",
                                                                                       contextname, tipoNamespace,
                                                                                       constructor))))
                                                                       ));
                                            }
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" não tem quaisquer Constructor(s) PUBLIC(s)!",
                                                                  constructorsCount))))
                                                                   ));
                                        }
                                        // Type.GetFields - public
                                        var fields =
                                            tipo.GetFields().Where(f => (f != null)).OrderBy(f => f.Name).Select(
                                                f => f.Name).Distinct();
                                        var fieldsCount = fields.Count(); // fields.Length;
                                        if (fieldsCount > 0)
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" tem {0} Field(s) PUBLIC(s):", fieldsCount))))
                                                                   ));
                                            foreach (var field in fields)
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        "Field PUBLIC:", field)))),
                                                    Views.TD(Views.A(true,
                                                                     string.Format("/{0}/ns/{1}/{2}/f/{3}", contextname,
                                                                                   tipoNamespace.
                                                                                       IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                                       (), tipoFullName, field),
                                                                     Views.Text2TagHTML(
                                                                         string.Format(" Field {2}",
                                                                                       contextname, tipoNamespace,
                                                                                       field))))
                                                                       ));
                                            }
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" não tem quaisquer Field(s) PUBLIC(s)!",
                                                                  fieldsCount))))
                                                                   ));
                                        }
                                        // Type.GetProperties - public
                                        var properties =
                                            tipo.GetProperties().Where(p => (p != null)).OrderBy(p => p.Name).Select(
                                                p => p.Name).Distinct();
                                        var propertiesCount = properties.Count(); // properties.Length;
                                        if (propertiesCount > 0)
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" tem {0} Property(ies) PUBLIC(s):", propertiesCount))))
                                                                   ));
                                            foreach (var property in properties)
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        "Property PUBLIC:", property)))),
                                                    Views.TD(Views.A(true,
                                                                     string.Format("/{0}/ns/{1}/{2}/p/{3}", contextname,
                                                                                   tipoNamespace.
                                                                                       IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                                       (), tipoFullName, property),
                                                                     Views.Text2TagHTML(
                                                                         string.Format(" Property {2}",
                                                                                       contextname, tipoNamespace,
                                                                                       property))))
                                                                       ));
                                            }
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" não tem quaisquer Property(ies) PUBLIC(s)!",
                                                                  propertiesCount))))
                                                                   ));
                                        }
                                        // Type.GetEvents - public
                                        var events =
                                            tipo.GetEvents().Where(e => (e != null)).OrderBy(e => e.Name).Select(
                                                e => e.Name).Distinct();
                                        var eventsCount = events.Count(); // events.Length;
                                        if (eventsCount > 0)
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" tem {0} Event(s) PUBLIC(s):", eventsCount))))
                                                                   ));
                                            foreach (var evento in events)
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        "Event PUBLIC:", evento)))),
                                                    Views.TD(Views.A(true,
                                                                     string.Format("/{0}/ns/{1}/{2}/e/{3}", contextname,
                                                                                   tipoNamespace.
                                                                                       IsNameOfNamespaceEmptyThenConvertToSpecial
                                                                                       (), tipoFullName, evento),
                                                                     Views.Text2TagHTML(
                                                                         string.Format(" Event {2}",
                                                                                       contextname, tipoNamespace,
                                                                                       evento))))
                                                                       ));
                                            }
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" não tem quaisquer Event(s) PUBLIC(s)!",
                                                                  eventsCount))))
                                                                   ));
                                        }
                                    }
                                    else
                                    {
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.P(
                                                Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                 contextname, namespaceOfContext)),
                                                Views.Text2TagHTML(string.Format(" deveria conter, mas não contém o Tipo requerido \"{0}\" !",
                                                                                 typeShortName)))
                                                )));
                                    }
                                }
                                else
                                {
                                    tagsHTMLnested.Add(Views.TR(
                                        Views.TD(Views.P(
                                            Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                             contextname, namespaceOfContext)),
                                            Views.Text2TagHTML(string.Format(" não contém o Tipo requerido \"{0}\" !",
                                                                             typeShortName)))
                                            )));
                                }
                            }
                            else
                            {
                                tagsHTMLnested.Add(Views.TR(
                                    Views.TD(Views.P(
                                        Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                         contextname, namespaceOfContext)),
                                        Views.Text2TagHTML(string.Format(" não contém quaisquer tipo(s)!",
                                                                         listTypeNamesOfNamespace.Count)))
                                        )));
                            }
                        }
                        else
                        {
                            tagsHTMLnested.Add(Views.TR(
                                Views.TD(Views.P(Views.Text2TagHTML(
                                    string.Format("Não existe o Namespace especificado \"{1}\" no Contexto {0} !",
                                                  contextname, namespaceName)))
                                    )));
                        }
                        tagsHTMLfull = Views.HTML(
                            Views.HEAD(Views.TITLE(string.Format("Propriedades de tipo de namespace de contexto: /{0}/ns/{1}/{2}",
                                                                 contextname, namespaceName, typeShortName))),
                            Views.BODY(Views.H1(string.Format("Input = lista de informações de tipo de namespace de contexto: /{0}/ns/{1}/{2}",
                                                                 contextname, namespaceName, typeShortName)),
                                       Views.P(string.Format("(<=> {0}/{1}/ns/{2}/{3})", Handlers.DIRECTORIA_RAIZ,
                                                                contextname, namespaceName, typeShortName)),
                                       Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                       Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                       Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                       Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                       Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                       Views.P(Views.A(true, string.Format("/{0}/ns/{1}",
                                                                       contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                                       Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                                   contextname, namespaceName)))),
                                       Views.TABLE(tagsHTMLnested.ToArray())
                                )
                            );
                        if (isDataAvailable)
                        {
                            // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                            return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
                        }
                        else
                        {
                            // HTTP status 404 = NotFound (the requested resource does not exist on the server)
                            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.NotFound, answerContent = tagsHTMLfull };
                        }
                    }
                    //typeShortName = typeShortName ?? "";
                    messageRangeOfError = "tipo";
                    tagHTMLextra =
                        Views.P(Views.A(true,
                                        string.Format("/{0}/ns/{1}", contextname,
                                                      namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                        Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                         contextname, namespaceName))));
                }
                //namespaceName = namespaceName ?? "";
                messageRangeOfError = messageRangeOfError ?? "namespace";
            }
            contextname = contextname ?? "";
            messageRangeOfError = messageRangeOfError ?? "contexto";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de tipo de namespace de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de tipo de namespace de contexto:"),
                                      Views.H3("Handler da Regra - ctx - ns - namespace - shortName"),
                                      Views.P(string.Format("Não se obteve valor do {0},", messageRangeOfError)),
                                      Views.P("apesar de haver mapeamento com ctx e namespace e shortName !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                      Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                        tagHTMLextra
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    public class MethodNameTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...MethodNameTypeNamespaceContextHandler...");

            TagHTML tagsHTMLfull;
            string messageRangeOfError = null;
            TagHTML tagHTMLextra = Views.Text2TagHTML("");
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string namespaceName;
                if (prms.TryGetValue("namespace", out namespaceName))
                {
                    namespaceName = namespaceName.IsNameOfNamespaceSpecialThenConvertToEmpty(); // conversao interna de normalizacao
                    string typeShortName;
                    if (prms.TryGetValue("shortName", out typeShortName))
                    {
                        string methodName;
                        if (prms.TryGetValue("methodName", out methodName))
                        {
                            var tagsHTMLnested = new List<TagHTML>();
                            var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                            var listAssembliesOfContext = new List<Assembly>();
                            dir.GetAssembliesFromDir(contextname, listAssembliesOfContext, tagsHTMLnested, false); // povoar
                            var dictNamespacesTypeNamesOfContext =
                                listAssembliesOfContext.ToArray().GetDictNamespacesTypeFullNamesFromAssemblies(false, true).
                                    OrderBy(elem => elem.Key).ToDictionary(elem => elem.Key, elem => elem.Value);
                            string namespaceOfContext;
                            List<string> listTypeNamesOfNamespace;
                            // o proprio namespace
                            bool isDataAvailable = false;
                            if (dictNamespacesTypeNamesOfContext.ContainsKey(namespaceName))
                            {
                                namespaceOfContext = namespaceName;
                                listTypeNamesOfNamespace = dictNamespacesTypeNamesOfContext[namespaceName];
                                if (listTypeNamesOfNamespace.Count > 0)
                                {
                                    if (listTypeNamesOfNamespace.Contains(typeShortName))
                                    {
                                        var listTypesEqualType =
                                            listAssembliesOfContext.GetTypesFromAssemblies(false, true, true).Where(
                                                t => t.GetFullNameFromType().Equals(typeShortName));
                                        if (listTypesEqualType.Count() > 0)
                                        {
                                            isDataAvailable = true;
                                            var tipo = listTypesEqualType.First();
                                            var tipoFullName = tipo.GetFullNameFromType();
                                            var tipoNamespace = tipo.GetNamespaceFromType();
                                            // Type.GetMethods - public
                                            var methods =
                                                tipo.GetMethods().Where(m => ((m != null) && (m.Name.Equals(methodName))));
                                            var methodsCount = methods.Count(); // methods.Length;
                                            if (methodsCount > 0)
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML(string.Format("Method {1} do Type {0}", tipoFullName, methodName))),
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        " tem {0} definição/definições diferente(s) PUBLIC(s):",
                                                        methodsCount))))
                                                                       ));
                                                foreach (var method in methods)
                                                {
                                                    var methodReturnType = method.ReturnType;
                                                    var methodReturnParam = method.ReturnParameter;
                                                    var methodParams = method.GetParameters();
                                                    var methodParamsCount = methodParams.Count(); // methodParams.Length;
                                                    var methodInstanceName = method.Name;
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.Text2TagHTML("Method PUBLIC:")),
                                                        Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                            " = {0}", methodInstanceName))))
                                                                           ));
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            "Características do Method do Type {2} do Namespace {1} do Contexto {0} :",
                                                            contextname, namespaceOfContext, tipoFullName, methodInstanceName))),
                                                        Views.TD(Views.Text2TagHTML((method.IsPublic ? " Public" : "") +
                                                                                    (method.IsPrivate ? " Private" : "") +
                                                                                    (method.IsAbstract ? " Abstract" : "") +
                                                                                    (method.IsFinal ? " Final" : "") +
                                                                                    (method.IsVirtual ? " Virtual" : "") +
                                                                                    (method.IsStatic ? " Static" : "") +
                                                                                    (method.IsGenericMethod ? " GenericMethod" : "") +
                                                                                    (method.IsGenericMethodDefinition ? " GenericMethodDefinition" : "") +
                                                                                    (method.ContainsGenericParameters ? " ContainsGenericParameters" : "") +
                                                                                    (method.IsSpecialName ? " SpecialName" : "") +
                                                                                    (method.IsSecurityCritical ? " SecurityCritical" : "") +
                                                                                    (method.IsSecuritySafeCritical ? " SecuritySafeCritical" : "") +
                                                                                    (method.IsSecurityTransparent ? " SecurityTransparent" : "")
                                                                     ))
                                                                           ));
                                                    //if (methodReturnType != null)
                                                    //{
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("ReturnType:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0}", methodReturnType.FullName)))
                                                                               ));
                                                    //}
                                                    if (methodReturnParam != null)
                                                    {
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("ReturnParameter:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0} de tipo {1}", methodReturnParam.Name,
                                                                methodReturnParam.ParameterType)))
                                                                               ));
                                                    }
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.P(Views.Text2TagHTML("GetParameters Count:"))),
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            " = {0} Parameter(s).", methodParamsCount)))
                                                                           ));
                                                    foreach (var methodParam in methodParams)
                                                    {
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("Parameter:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0} de tipo {1}", methodParam.Name,
                                                                methodParam.ParameterType)))
                                                                               ));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                    Views.TD(Views.P(Views.Text2TagHTML(
                                                        string.Format(" não tem quaisquer Method(s) PUBLIC(s) com o nome requerido {0} !",
                                                                      methodName))))
                                                                       ));
                                            }
                                            // Type.GetMethods - public - END
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.P(
                                                    Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                     contextname, namespaceOfContext)),
                                                    Views.Text2TagHTML(string.Format(" deveria conter, mas não contém o Tipo requerido \"{0}\" !",
                                                                                     typeShortName)))
                                                    )));
                                        }
                                    }
                                    else
                                    {
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.P(
                                                Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                 contextname, namespaceOfContext)),
                                                Views.Text2TagHTML(string.Format(" não contém o Tipo requerido \"{0}\" !",
                                                                                 typeShortName)))
                                                )));
                                    }
                                }
                                else
                                {
                                    tagsHTMLnested.Add(Views.TR(
                                        Views.TD(Views.P(
                                            Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                             contextname, namespaceOfContext)),
                                            Views.Text2TagHTML(string.Format(" não contém quaisquer tipo(s)!",
                                                                             listTypeNamesOfNamespace.Count)))
                                            )));
                                }
                            }
                            else
                            {
                                tagsHTMLnested.Add(Views.TR(
                                    Views.TD(Views.P(Views.Text2TagHTML(
                                        string.Format("Não existe o Namespace especificado \"{1}\" no Contexto {0} !",
                                                      contextname, namespaceName)))
                                        )));
                            }
                            tagsHTMLfull = Views.HTML(
                                Views.HEAD(Views.TITLE(string.Format("Propriedades de método(s) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/m/{3}",
                                                                     contextname, namespaceName, typeShortName, methodName))),
                                Views.BODY(Views.H1(string.Format("Input = lista de informações de método(s) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/m/{3}",
                                                                     contextname, namespaceName, typeShortName, methodName)),
                                           Views.P(string.Format("(<=> {0}/{1}/ns/{2}/{3}/m/{4})", Handlers.DIRECTORIA_RAIZ,
                                                                    contextname, namespaceName, typeShortName, methodName)),
                                           Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                           Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                           Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                           Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns/{1}",
                                                                           contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                                           Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                                       contextname, namespaceName)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns/{1}/{2}",
                                                                           contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial(),
                                                                           typeShortName),
                                                           Views.Text2TagHTML(string.Format("[ link tipo namespace contexto /{0}/ns/{1}/{2} ]",
                                                                                       contextname, namespaceName, typeShortName)))),
                                           Views.TABLE(tagsHTMLnested.ToArray())
                                    )
                                );
                            if (isDataAvailable)
                            {
                                // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                                return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
                            }
                            else
                            {
                                // HTTP status 404 = NotFound (the requested resource does not exist on the server)
                                return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.NotFound, answerContent = tagsHTMLfull };
                            }
                        }
                        //methodName = methodName ?? "";
                        messageRangeOfError = "método";
                        tagHTMLextra =
                            Views.P(Views.A(true,
                                            string.Format("/{0}/ns/{1}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                            Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                             contextname, namespaceName))),
                                    Views.A(true,
                                            string.Format("/{0}/ns/{1}/{2}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial(), typeShortName),
                                            Views.Text2TagHTML(string.Format("[ link tipo namespace contexto /{0}/ns/{1}/{2} ]",
                                                                             contextname, namespaceName, typeShortName))));
                    }
                    //typeShortName = typeShortName ?? "";
                    messageRangeOfError = messageRangeOfError ?? "tipo";
                    if (messageRangeOfError.Equals("tipo"))
                    {
                        tagHTMLextra =
                            Views.P(Views.A(true,
                                            string.Format("/{0}/ns/{1}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                            Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                             contextname, namespaceName))));
                    }
                }
                //namespaceName = namespaceName ?? "";
                messageRangeOfError = messageRangeOfError ?? "namespace";
            }
            contextname = contextname ?? "";
            messageRangeOfError = messageRangeOfError ?? "contexto";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de método de tipo de namespace de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de método de tipo de namespace de contexto:"),
                                      Views.H3("Handler da Regra - ctx - ns - namespace - shortName - m - methodName"),
                                      Views.P(string.Format("Não se obteve valor do {0},", messageRangeOfError)),
                                      Views.P("apesar de haver mapeamento com ctx e namespace e shortName e methodName !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                      Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                        tagHTMLextra
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    public class ConstructsTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...ConstructsTypeNamespaceContextHandler...");

            TagHTML tagsHTMLfull;
            string messageRangeOfError = null;
            TagHTML tagHTMLextra = Views.Text2TagHTML("");
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string namespaceName;
                if (prms.TryGetValue("namespace", out namespaceName))
                {
                    namespaceName = namespaceName.IsNameOfNamespaceSpecialThenConvertToEmpty(); // conversao interna de normalizacao
                    string typeShortName;
                    if (prms.TryGetValue("shortName", out typeShortName))
                    {
                        var tagsHTMLnested = new List<TagHTML>();
                        var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                        var listAssembliesOfContext = new List<Assembly>();
                        dir.GetAssembliesFromDir(contextname, listAssembliesOfContext, tagsHTMLnested, false); // povoar
                        var dictNamespacesTypeNamesOfContext =
                            listAssembliesOfContext.ToArray().GetDictNamespacesTypeFullNamesFromAssemblies(false, true).
                                OrderBy(elem => elem.Key).ToDictionary(elem => elem.Key, elem => elem.Value);
                        string namespaceOfContext;
                        List<string> listTypeNamesOfNamespace;
                        // o proprio namespace
                        bool isDataAvailable = false;
                        if (dictNamespacesTypeNamesOfContext.ContainsKey(namespaceName))
                        {
                            namespaceOfContext = namespaceName;
                            listTypeNamesOfNamespace = dictNamespacesTypeNamesOfContext[namespaceName];
                            if (listTypeNamesOfNamespace.Count > 0)
                            {
                                if (listTypeNamesOfNamespace.Contains(typeShortName))
                                {
                                    var listTypesEqualType =
                                        listAssembliesOfContext.GetTypesFromAssemblies(false, true, true).Where(
                                            t => t.GetFullNameFromType().Equals(typeShortName));
                                    if (listTypesEqualType.Count() > 0)
                                    {
                                        isDataAvailable = true;
                                        var tipo = listTypesEqualType.First();
                                        var tipoFullName = tipo.GetFullNameFromType();
                                        var tipoNamespace = tipo.GetNamespaceFromType();
                                        // Type.GetConstructors - public
                                        var constructors =
                                            tipo.GetConstructors().Where(c => (c != null)); // ( && (c.Name.Equals(".ctor")))
                                        var constructorsCount = constructors.Count(); // constructors.Length;
                                        if (constructorsCount > 0)
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Constructor do Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                    " tem {0} definição/definições diferente(s) PUBLIC(s):",
                                                    constructorsCount))))
                                                                   ));
                                            foreach (var constructor in constructors)
                                            {
                                                //var constructorReturnType = constructor.ReturnType;
                                                //var constructorReturnParam = constructor.ReturnParameter;
                                                var constructorParams = constructor.GetParameters();
                                                var constructorParamsCount = constructorParams.Count(); // methodParams.Length;
                                                var constructorName = constructor.Name;
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML("Constructor PUBLIC:")),
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        " = {0}", constructorName))))
                                                                       ));
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML(string.Format(
                                                        "Características do Constructor do Type {2} do Namespace {1} do Contexto {0} :",
                                                        contextname, namespaceOfContext, tipoFullName, constructorName))),
                                                    Views.TD(Views.Text2TagHTML((constructor.IsPublic ? " Public" : "") +
                                                                                (constructor.IsPrivate ? " Private" : "") +
                                                                                (constructor.IsAbstract ? " Abstract" : "") +
                                                                                (constructor.IsFinal ? " Final" : "") +
                                                                                (constructor.IsVirtual ? " Virtual" : "") +
                                                                                (constructor.IsStatic ? " Static" : "") +
                                                                                (constructor.IsGenericMethod ? " GenericMethod" : "") +
                                                                                (constructor.IsGenericMethodDefinition ? " GenericMethodDefinition" : "") +
                                                                                (constructor.ContainsGenericParameters ? " ContainsGenericParameters" : "") +
                                                                                (constructor.IsSpecialName ? " SpecialName" : "") +
                                                                                (constructor.IsSecurityCritical ? " SecurityCritical" : "") +
                                                                                (constructor.IsSecuritySafeCritical ? " SecuritySafeCritical" : "") +
                                                                                (constructor.IsSecurityTransparent ? " SecurityTransparent" : "")
                                                                 ))
                                                                       ));
                                                ////if (constructorReturnType != null)
                                                ////{
                                                //tagsHTMLnested.Add(Views.TR(
                                                //    Views.TD(Views.P(Views.Text2TagHTML("ReturnType:"))),
                                                //    Views.TD(Views.Text2TagHTML(string.Format(
                                                //        " = {0}", constructorReturnType.FullName)))
                                                //                       ));
                                                ////}
                                                //if (constructorReturnParam != null)
                                                //{
                                                //    tagsHTMLnested.Add(Views.TR(
                                                //        Views.TD(Views.P(Views.Text2TagHTML("ReturnParameter:"))),
                                                //        Views.TD(Views.Text2TagHTML(string.Format(
                                                //            " = {0} de tipo {1}", constructorReturnParam.Name,
                                                //            constructorReturnParam.ParameterType)))
                                                //                           ));
                                                //}
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.P(Views.Text2TagHTML("GetParameters Count:"))),
                                                    Views.TD(Views.Text2TagHTML(string.Format(
                                                        " = {0} Parameter(s).", constructorParamsCount)))
                                                                       ));
                                                foreach (var constructorParam in constructorParams)
                                                {
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.P(Views.Text2TagHTML("Parameter:"))),
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            " = {0} de tipo {1}", constructorParam.Name,
                                                            constructorParam.ParameterType)))
                                                                           ));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                Views.TD(Views.P(Views.Text2TagHTML(
                                                    string.Format(" não tem quaisquer Constructor(s) PUBLIC(s) !"
                                                                  ))))
                                                                   )); // com o nome requerido {0}, methodName
                                        }
                                        // Type.GetConstructors - public - END
                                    }
                                    else
                                    {
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.P(
                                                Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                 contextname, namespaceOfContext)),
                                                Views.Text2TagHTML(string.Format(" deveria conter, mas não contém o Tipo requerido \"{0}\" !",
                                                                                 typeShortName)))
                                                )));
                                    }
                                }
                                else
                                {
                                    tagsHTMLnested.Add(Views.TR(
                                        Views.TD(Views.P(
                                            Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                             contextname, namespaceOfContext)),
                                            Views.Text2TagHTML(string.Format(" não contém o Tipo requerido \"{0}\" !",
                                                                             typeShortName)))
                                            )));
                                }
                            }
                            else
                            {
                                tagsHTMLnested.Add(Views.TR(
                                    Views.TD(Views.P(
                                        Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                         contextname, namespaceOfContext)),
                                        Views.Text2TagHTML(string.Format(" não contém quaisquer tipo(s)!",
                                                                         listTypeNamesOfNamespace.Count)))
                                        )));
                            }
                        }
                        else
                        {
                            tagsHTMLnested.Add(Views.TR(
                                Views.TD(Views.P(Views.Text2TagHTML(
                                    string.Format("Não existe o Namespace especificado \"{1}\" no Contexto {0} !",
                                                  contextname, namespaceName)))
                                    )));
                        }
                        tagsHTMLfull = Views.HTML(
                            Views.HEAD(Views.TITLE(string.Format("Propriedades de construtor(es) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/c",
                                                                 contextname, namespaceName, typeShortName))),
                            Views.BODY(Views.H1(string.Format("Input = lista de informações de construtor(es) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/c",
                                                                 contextname, namespaceName, typeShortName)),
                                       Views.P(string.Format("(<=> {0}/{1}/ns/{2}/{3}/c)", Handlers.DIRECTORIA_RAIZ,
                                                                contextname, namespaceName, typeShortName)),
                                       Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                       Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                       Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                       Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                       Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                       Views.P(Views.A(true, string.Format("/{0}/ns/{1}",
                                                                       contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                                       Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                                   contextname, namespaceName)))),
                                       Views.P(Views.A(true, string.Format("/{0}/ns/{1}/{2}",
                                                                       contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial(),
                                                                       typeShortName),
                                                       Views.Text2TagHTML(string.Format("[ link tipo namespace contexto /{0}/ns/{1}/{2} ]",
                                                                                   contextname, namespaceName, typeShortName)))),
                                       Views.TABLE(tagsHTMLnested.ToArray())
                                )
                            );
                        if (isDataAvailable)
                        {
                            // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                            return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
                        }
                        else
                        {
                            // HTTP status 404 = NotFound (the requested resource does not exist on the server)
                            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.NotFound, answerContent = tagsHTMLfull };
                        }
                    }
                    //typeShortName = typeShortName ?? "";
                    messageRangeOfError = "tipo";
                    tagHTMLextra =
                        Views.P(Views.A(true,
                                        string.Format("/{0}/ns/{1}", contextname,
                                                      namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                        Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                         contextname, namespaceName))));
                }
                //namespaceName = namespaceName ?? "";
                messageRangeOfError = messageRangeOfError ?? "namespace";
            }
            contextname = contextname ?? "";
            messageRangeOfError = messageRangeOfError ?? "contexto";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de métodos construtores de tipo de namespace de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de métodos construtores de tipo de namespace de contexto:"),
                                      Views.H3("Handler da Regra - ctx - ns - namespace - shortName - c"),
                                      Views.P(string.Format("Não se obteve valor do {0},", messageRangeOfError)),
                                      Views.P("apesar de haver mapeamento com ctx e namespace e shortName !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                      Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                        tagHTMLextra
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    #endregion

    #region 3 Especificacoes -Enunciado- FieldNameTypeNamespaceContext... PropNameTypeNamespaceContext... EventNameTypeNamespaceContext...

    public class FieldNameTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...FieldNameTypeNamespaceContextHandler...");

            TagHTML tagsHTMLfull;
            string messageRangeOfError = null;
            TagHTML tagHTMLextra = Views.Text2TagHTML("");
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string namespaceName;
                if (prms.TryGetValue("namespace", out namespaceName))
                {
                    namespaceName = namespaceName.IsNameOfNamespaceSpecialThenConvertToEmpty(); // conversao interna de normalizacao
                    string typeShortName;
                    if (prms.TryGetValue("shortName", out typeShortName))
                    {
                        string fieldName;
                        if (prms.TryGetValue("fieldName", out fieldName))
                        {
                            var tagsHTMLnested = new List<TagHTML>();
                            var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                            var listAssembliesOfContext = new List<Assembly>();
                            dir.GetAssembliesFromDir(contextname, listAssembliesOfContext, tagsHTMLnested, false); // povoar
                            var dictNamespacesTypeNamesOfContext =
                                listAssembliesOfContext.ToArray().GetDictNamespacesTypeFullNamesFromAssemblies(false, true).
                                    OrderBy(elem => elem.Key).ToDictionary(elem => elem.Key, elem => elem.Value);
                            string namespaceOfContext;
                            List<string> listTypeNamesOfNamespace;
                            // o proprio namespace
                            bool isDataAvailable = false;
                            if (dictNamespacesTypeNamesOfContext.ContainsKey(namespaceName))
                            {
                                namespaceOfContext = namespaceName;
                                listTypeNamesOfNamespace = dictNamespacesTypeNamesOfContext[namespaceName];
                                if (listTypeNamesOfNamespace.Count > 0)
                                {
                                    if (listTypeNamesOfNamespace.Contains(typeShortName))
                                    {
                                        var listTypesEqualType =
                                            listAssembliesOfContext.GetTypesFromAssemblies(false, true, true).Where(
                                                t => t.GetFullNameFromType().Equals(typeShortName));
                                        if (listTypesEqualType.Count() > 0)
                                        {
                                            isDataAvailable = true;
                                            var tipo = listTypesEqualType.First();
                                            var tipoFullName = tipo.GetFullNameFromType();
                                            var tipoNamespace = tipo.GetNamespaceFromType();
                                            // Type.GetFields - public
                                            var fields =
                                                tipo.GetFields().Where(f => ((f != null) && (f.Name.Equals(fieldName))));
                                            var fieldsCount = fields.Count(); // fields.Length;
                                            if (fieldsCount > 0)
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML(string.Format("Field {1} do Type {0}", tipoFullName, fieldName))),
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        " tem {0} definição/definições diferente(s) PUBLIC(s):",
                                                        fieldsCount))))
                                                                       ));
                                                foreach (var field in fields)
                                                {
                                                    var fieldType = field.FieldType;
                                                    //var methodReturnParam = method.ReturnParameter;
                                                    //var fieldRawConstantValue = field.GetRawConstantValue();
                                                    //var methodParamsCount = methodParams.Count(); // methodParams.Length;
                                                    var fieldInstanceName = field.Name;
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.Text2TagHTML("Field PUBLIC:")),
                                                        Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                            " = {0}", fieldInstanceName))))
                                                                           ));
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            "Características do Field do Type {2} do Namespace {1} do Contexto {0} :",
                                                            contextname, namespaceOfContext, tipoFullName, fieldInstanceName))),
                                                        Views.TD(Views.Text2TagHTML((field.IsPublic ? " Public" : "") +
                                                                                    (field.IsPrivate ? " Private" : "") +
                                                                                    (field.IsStatic ? " Static" : "") +
                                                                                    (field.IsLiteral ? " Literal" : "") +
                                                                                    (field.IsInitOnly ? " InitOnly" : "") +
                                                                                    (field.IsSpecialName ? " SpecialName" : "") +
                                                                                    (field.IsSecurityCritical ? " SecurityCritical" : "") +
                                                                                    (field.IsSecuritySafeCritical ? " SecuritySafeCritical" : "") +
                                                                                    (field.IsSecurityTransparent ? " SecurityTransparent" : "")
                                                                     ))
                                                                           ));
                                                    //if (fieldType != null)
                                                    //{
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.P(Views.Text2TagHTML("Field Type:"))),
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            " = {0}", fieldType.FullName)))
                                                                           ));
                                                    //}
                                                    ////if (fieldRawConstantValue != null)
                                                    ////{
                                                    //    tagsHTMLnested.Add(Views.TR(
                                                    //        Views.TD(Views.P(Views.Text2TagHTML("Field Raw Constant Value:"))),
                                                    //        Views.TD(Views.Text2TagHTML(string.Format(
                                                    //            " = {0}", fieldRawConstantValue)))
                                                    //                           ));
                                                    ////}
                                                    //tagsHTMLnested.Add(Views.TR(
                                                    //    Views.TD(Views.P(Views.Text2TagHTML("GetParameters Count:"))),
                                                    //    Views.TD(Views.Text2TagHTML(string.Format(
                                                    //        " = {0} Parameter(s).", methodParamsCount)))
                                                    //                       ));
                                                    //foreach (var methodParam in methodParams)
                                                    //{
                                                    //    tagsHTMLnested.Add(Views.TR(
                                                    //        Views.TD(Views.P(Views.Text2TagHTML("Parameter:"))),
                                                    //        Views.TD(Views.Text2TagHTML(string.Format(
                                                    //            " = {0} de tipo {1}", methodParam.Name,
                                                    //            methodParam.ParameterType)))
                                                    //                           ));
                                                    //}
                                                }
                                            }
                                            else
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                    Views.TD(Views.P(Views.Text2TagHTML(
                                                        string.Format(" não tem quaisquer Field(s) PUBLIC(s) com o nome requerido {0} !",
                                                                      fieldName))))
                                                                       ));
                                            }
                                            // Type.GetFields - public - END
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.P(
                                                    Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                     contextname, namespaceOfContext)),
                                                    Views.Text2TagHTML(string.Format(" deveria conter, mas não contém o Tipo requerido \"{0}\" !",
                                                                                     typeShortName)))
                                                    )));
                                        }
                                    }
                                    else
                                    {
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.P(
                                                Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                 contextname, namespaceOfContext)),
                                                Views.Text2TagHTML(string.Format(" não contém o Tipo requerido \"{0}\" !",
                                                                                 typeShortName)))
                                                )));
                                    }
                                }
                                else
                                {
                                    tagsHTMLnested.Add(Views.TR(
                                        Views.TD(Views.P(
                                            Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                             contextname, namespaceOfContext)),
                                            Views.Text2TagHTML(string.Format(" não contém quaisquer tipo(s)!",
                                                                             listTypeNamesOfNamespace.Count)))
                                            )));
                                }
                            }
                            else
                            {
                                tagsHTMLnested.Add(Views.TR(
                                    Views.TD(Views.P(Views.Text2TagHTML(
                                        string.Format("Não existe o Namespace especificado \"{1}\" no Contexto {0} !",
                                                      contextname, namespaceName)))
                                        )));
                            }
                            tagsHTMLfull = Views.HTML(
                                Views.HEAD(Views.TITLE(string.Format("Propriedades de campo(s) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/f/{3}",
                                                                     contextname, namespaceName, typeShortName, fieldName))),
                                Views.BODY(Views.H1(string.Format("Input = lista de informações de campo(s) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/f/{3}",
                                                                     contextname, namespaceName, typeShortName, fieldName)),
                                           Views.P(string.Format("(<=> {0}/{1}/ns/{2}/{3}/f/{4})", Handlers.DIRECTORIA_RAIZ,
                                                                    contextname, namespaceName, typeShortName, fieldName)),
                                           Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                           Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                           Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                           Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns/{1}",
                                                                           contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                                           Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                                       contextname, namespaceName)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns/{1}/{2}",
                                                                           contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial(),
                                                                           typeShortName),
                                                           Views.Text2TagHTML(string.Format("[ link tipo namespace contexto /{0}/ns/{1}/{2} ]",
                                                                                       contextname, namespaceName, typeShortName)))),
                                           Views.TABLE(tagsHTMLnested.ToArray())
                                    )
                                );
                            if (isDataAvailable)
                            {
                                // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                                return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
                            }
                            else
                            {
                                // HTTP status 404 = NotFound (the requested resource does not exist on the server)
                                return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.NotFound, answerContent = tagsHTMLfull };
                            }
                        }
                        //fieldName = fieldName ?? "";
                        messageRangeOfError = "campo";
                        tagHTMLextra =
                            Views.P(Views.A(true,
                                            string.Format("/{0}/ns/{1}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                            Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                             contextname, namespaceName))),
                                    Views.A(true,
                                            string.Format("/{0}/ns/{1}/{2}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial(), typeShortName),
                                            Views.Text2TagHTML(string.Format("[ link tipo namespace contexto /{0}/ns/{1}/{2} ]",
                                                                             contextname, namespaceName, typeShortName))));
                    }
                    //typeShortName = typeShortName ?? "";
                    messageRangeOfError = messageRangeOfError ?? "tipo";
                    if (messageRangeOfError.Equals("tipo"))
                    {
                        tagHTMLextra =
                            Views.P(Views.A(true,
                                            string.Format("/{0}/ns/{1}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                            Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                             contextname, namespaceName))));
                    }
                }
                //namespaceName = namespaceName ?? "";
                messageRangeOfError = messageRangeOfError ?? "namespace";
            }
            contextname = contextname ?? "";
            messageRangeOfError = messageRangeOfError ?? "contexto";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de campo de tipo de namespace de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de campo de tipo de namespace de contexto:"),
                                      Views.H3("Handler da Regra - ctx - ns - namespace - shortName - f - fieldName"),
                                      Views.P(string.Format("Não se obteve valor do {0},", messageRangeOfError)),
                                      Views.P("apesar de haver mapeamento com ctx e namespace e shortName e fieldName !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                      Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                        tagHTMLextra
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    public class PropNameTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...PropNameTypeNamespaceContextHandler...");

            TagHTML tagsHTMLfull;
            string messageRangeOfError = null;
            TagHTML tagHTMLextra = Views.Text2TagHTML("");
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string namespaceName;
                if (prms.TryGetValue("namespace", out namespaceName))
                {
                    namespaceName = namespaceName.IsNameOfNamespaceSpecialThenConvertToEmpty(); // conversao interna de normalizacao
                    string typeShortName;
                    if (prms.TryGetValue("shortName", out typeShortName))
                    {
                        string propName;
                        if (prms.TryGetValue("propName", out propName))
                        {
                            var tagsHTMLnested = new List<TagHTML>();
                            var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                            var listAssembliesOfContext = new List<Assembly>();
                            dir.GetAssembliesFromDir(contextname, listAssembliesOfContext, tagsHTMLnested, false); // povoar
                            var dictNamespacesTypeNamesOfContext =
                                listAssembliesOfContext.ToArray().GetDictNamespacesTypeFullNamesFromAssemblies(false, true).
                                    OrderBy(elem => elem.Key).ToDictionary(elem => elem.Key, elem => elem.Value);
                            string namespaceOfContext;
                            List<string> listTypeNamesOfNamespace;
                            // o proprio namespace
                            bool isDataAvailable = false;
                            if (dictNamespacesTypeNamesOfContext.ContainsKey(namespaceName))
                            {
                                namespaceOfContext = namespaceName;
                                listTypeNamesOfNamespace = dictNamespacesTypeNamesOfContext[namespaceName];
                                if (listTypeNamesOfNamespace.Count > 0)
                                {
                                    if (listTypeNamesOfNamespace.Contains(typeShortName))
                                    {
                                        var listTypesEqualType =
                                            listAssembliesOfContext.GetTypesFromAssemblies(false, true, true).Where(
                                                t => t.GetFullNameFromType().Equals(typeShortName));
                                        if (listTypesEqualType.Count() > 0)
                                        {
                                            isDataAvailable = true;
                                            var tipo = listTypesEqualType.First();
                                            var tipoFullName = tipo.GetFullNameFromType();
                                            var tipoNamespace = tipo.GetNamespaceFromType();
                                            // Type.GetProperties - public
                                            var properties =
                                                tipo.GetProperties().Where(p => ((p != null) && (p.Name.Equals(propName))));
                                            var propertiesCount = properties.Count(); // properties.Length;
                                            if (propertiesCount > 0)
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML(string.Format("Property {1} do Type {0}", tipoFullName, propName))),
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        " tem {0} definição/definições diferente(s) PUBLIC(s):",
                                                        propertiesCount))))
                                                                       ));
                                                foreach (var property in properties)
                                                {
                                                    var propertyType = property.PropertyType;
                                                    //var propertyConstantValue = property.GetConstantValue();
                                                    //var propertyRawConstantValue = property.GetRawConstantValue();
                                                    var propertyAccessors = property.GetAccessors();
                                                    var propertyAccessorsCount = propertyAccessors.Count(); // propertyAccessors.Length;
                                                    var propertyIndexParams = property.GetIndexParameters();
                                                    var propertyIndexParamsCount = propertyIndexParams.Count(); // propertyIndexParameters.Length;
                                                    var propertyName = property.Name;
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.Text2TagHTML("Property PUBLIC:")),
                                                        Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                            " = {0}", propertyName))))
                                                                           ));
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            "Características da Property do Type {2} do Namespace {1} do Contexto {0} :",
                                                            contextname, namespaceOfContext, tipoFullName, propertyName))),
                                                        Views.TD(Views.Text2TagHTML((property.CanRead ? " CanRead" : "") +
                                                                                    (property.CanWrite ? " CanWrite" : "") +
                                                                                    (property.GetGetMethod() != null ? " has_Get_Method_Accessor" : "") +
                                                                                    (property.GetSetMethod() != null ? " has_Set_Method_Accessor" : "") +
                                                                                    (property.IsSpecialName ? " SpecialName" : "")
                                                                     ))
                                                                           ));
                                                    //if (propertyType != null)
                                                    //{
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("Property Type:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0}", propertyType.FullName)))
                                                                               ));
                                                    //}
                                                    //if (propertyRawConstantValue != null)
                                                    //{
                                                    //    tagsHTMLnested.Add(Views.TR(
                                                    //        Views.TD(Views.P(Views.Text2TagHTML("Property Raw Constant Value:"))),
                                                    //        Views.TD(Views.Text2TagHTML(string.Format(
                                                    //            " = {0}", propertyRawConstantValue)))
                                                    //                           ));
                                                    //}
                                                    //if (propertyConstantValue != null)
                                                    //{
                                                    //    tagsHTMLnested.Add(Views.TR(
                                                    //        Views.TD(Views.P(Views.Text2TagHTML("Property Constant Value:"))),
                                                    //        Views.TD(Views.Text2TagHTML(string.Format(
                                                    //            " = {0}", propertyConstantValue)))
                                                    //                           ));
                                                    //}
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.P(Views.Text2TagHTML("GetAccessors Count:"))),
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            " = {0} Accessor(s).", propertyAccessorsCount)))
                                                                           ));
                                                    foreach (var propertyAccessor in propertyAccessors)
                                                    {
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("Accessor:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0}", propertyAccessor)))
                                                                               ));
                                                    }
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.P(Views.Text2TagHTML("GetIndexParameters Count:"))),
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            " = {0} IndexParameter(s).", propertyIndexParamsCount)))
                                                                           ));
                                                    foreach (var propertyIndexParam in propertyIndexParams)
                                                    {
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("IndexParameter:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0} de tipo {1}", propertyIndexParam.Name,
                                                                propertyIndexParam.ParameterType)))
                                                                               ));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                    Views.TD(Views.P(Views.Text2TagHTML(
                                                        string.Format(" não tem quaisquer Property(ies) PUBLIC(s) com o nome requerido {0} !",
                                                                      propName))))
                                                                       ));
                                            }
                                            // Type.GetProperties - public - END
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.P(
                                                    Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                     contextname, namespaceOfContext)),
                                                    Views.Text2TagHTML(string.Format(" deveria conter, mas não contém o Tipo requerido \"{0}\" !",
                                                                                     typeShortName)))
                                                    )));
                                        }
                                    }
                                    else
                                    {
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.P(
                                                Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                 contextname, namespaceOfContext)),
                                                Views.Text2TagHTML(string.Format(" não contém o Tipo requerido \"{0}\" !",
                                                                                 typeShortName)))
                                                )));
                                    }
                                }
                                else
                                {
                                    tagsHTMLnested.Add(Views.TR(
                                        Views.TD(Views.P(
                                            Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                             contextname, namespaceOfContext)),
                                            Views.Text2TagHTML(string.Format(" não contém quaisquer tipo(s)!",
                                                                             listTypeNamesOfNamespace.Count)))
                                            )));
                                }
                            }
                            else
                            {
                                tagsHTMLnested.Add(Views.TR(
                                    Views.TD(Views.P(Views.Text2TagHTML(
                                        string.Format("Não existe o Namespace especificado \"{1}\" no Contexto {0} !",
                                                      contextname, namespaceName)))
                                        )));
                            }
                            tagsHTMLfull = Views.HTML(
                                Views.HEAD(Views.TITLE(string.Format("Propriedades de property(s) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/p/{3}",
                                                                     contextname, namespaceName, typeShortName, propName))),
                                Views.BODY(Views.H1(string.Format("Input = lista de informações de property(s) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/p/{3}",
                                                                     contextname, namespaceName, typeShortName, propName)),
                                           Views.P(string.Format("(<=> {0}/{1}/ns/{2}/{3}/p/{4})", Handlers.DIRECTORIA_RAIZ,
                                                                    contextname, namespaceName, typeShortName, propName)),
                                           Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                           Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                           Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                           Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns/{1}",
                                                                           contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                                           Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                                       contextname, namespaceName)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns/{1}/{2}",
                                                                           contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial(),
                                                                           typeShortName),
                                                           Views.Text2TagHTML(string.Format("[ link tipo namespace contexto /{0}/ns/{1}/{2} ]",
                                                                                       contextname, namespaceName, typeShortName)))),
                                           Views.TABLE(tagsHTMLnested.ToArray())
                                    )
                                );
                            if (isDataAvailable)
                            {
                                // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                                return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
                            }
                            else
                            {
                                // HTTP status 404 = NotFound (the requested resource does not exist on the server)
                                return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.NotFound, answerContent = tagsHTMLfull };
                            }
                        }
                        //propName = propName ?? "";
                        messageRangeOfError = "nomeDEpropriedade";
                        tagHTMLextra =
                            Views.P(Views.A(true,
                                            string.Format("/{0}/ns/{1}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                            Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                             contextname, namespaceName))),
                                    Views.A(true,
                                            string.Format("/{0}/ns/{1}/{2}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial(), typeShortName),
                                            Views.Text2TagHTML(string.Format("[ link tipo namespace contexto /{0}/ns/{1}/{2} ]",
                                                                             contextname, namespaceName, typeShortName))));
                    }
                    //typeShortName = typeShortName ?? "";
                    messageRangeOfError = messageRangeOfError ?? "tipo";
                    if (messageRangeOfError.Equals("tipo"))
                    {
                        tagHTMLextra =
                            Views.P(Views.A(true,
                                            string.Format("/{0}/ns/{1}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                            Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                             contextname, namespaceName))));
                    }
                }
                //namespaceName = namespaceName ?? "";
                messageRangeOfError = messageRangeOfError ?? "namespace";
            }
            contextname = contextname ?? "";
            messageRangeOfError = messageRangeOfError ?? "contexto";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de propriedade de tipo de namespace de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de propriedade de tipo de namespace de contexto:"),
                                      Views.H3("Handler da Regra - ctx - ns - namespace - shortName - p - propName"),
                                      Views.P(string.Format("Não se obteve valor do {0},", messageRangeOfError)),
                                      Views.P("apesar de haver mapeamento com ctx e namespace e shortName e propName !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                      Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                        tagHTMLextra
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    public class EventNameTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            Console.WriteLine("...EventNameTypeNamespaceContextHandler...");

            TagHTML tagsHTMLfull;
            string messageRangeOfError = null;
            TagHTML tagHTMLextra = Views.Text2TagHTML("");
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string namespaceName;
                if (prms.TryGetValue("namespace", out namespaceName))
                {
                    namespaceName = namespaceName.IsNameOfNamespaceSpecialThenConvertToEmpty(); // conversao interna de normalizacao
                    string typeShortName;
                    if (prms.TryGetValue("shortName", out typeShortName))
                    {
                        string eventName;
                        if (prms.TryGetValue("eventName", out eventName))
                        {
                            var tagsHTMLnested = new List<TagHTML>();
                            var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                            var listAssembliesOfContext = new List<Assembly>();
                            dir.GetAssembliesFromDir(contextname, listAssembliesOfContext, tagsHTMLnested, false); // povoar
                            var dictNamespacesTypeNamesOfContext =
                                listAssembliesOfContext.ToArray().GetDictNamespacesTypeFullNamesFromAssemblies(false, true).
                                    OrderBy(elem => elem.Key).ToDictionary(elem => elem.Key, elem => elem.Value);
                            string namespaceOfContext;
                            List<string> listTypeNamesOfNamespace;
                            // o proprio namespace
                            bool isDataAvailable = false;
                            if (dictNamespacesTypeNamesOfContext.ContainsKey(namespaceName))
                            {
                                namespaceOfContext = namespaceName;
                                listTypeNamesOfNamespace = dictNamespacesTypeNamesOfContext[namespaceName];
                                if (listTypeNamesOfNamespace.Count > 0)
                                {
                                    if (listTypeNamesOfNamespace.Contains(typeShortName))
                                    {
                                        var listTypesEqualType =
                                            listAssembliesOfContext.GetTypesFromAssemblies(false, true, true).Where(
                                                t => t.GetFullNameFromType().Equals(typeShortName));
                                        if (listTypesEqualType.Count() > 0)
                                        {
                                            isDataAvailable = true;
                                            var tipo = listTypesEqualType.First();
                                            var tipoFullName = tipo.GetFullNameFromType();
                                            var tipoNamespace = tipo.GetNamespaceFromType();
                                            // Type.GetEvents - public
                                            var events =
                                                tipo.GetEvents().Where(e => ((e != null) && (e.Name.Equals(eventName))));
                                            var eventsCount = events.Count(); // events.Length;
                                            if (eventsCount > 0)
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML(string.Format("Event {1} do Type {0}", tipoFullName, eventName))),
                                                    Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                        " tem {0} definição/definições diferente(s) PUBLIC(s):",
                                                        eventsCount))))
                                                                       ));
                                                foreach (var evento in events)
                                                {
                                                    var eventType = evento.GetType();
                                                    var eventAddMethod = evento.GetAddMethod();
                                                    var eventRaiseMethod = evento.GetRaiseMethod();
                                                    var eventRemoveMethod = evento.GetRemoveMethod();
                                                    var eventOtherMethods = evento.GetOtherMethods();
                                                    var eventOtherMethodsCount = eventOtherMethods.Count(); // eventOtherMethods.Length;
                                                    var eventInstanceName = evento.Name;
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.Text2TagHTML("Event PUBLIC:")),
                                                        Views.TD(Views.P(Views.Text2TagHTML(string.Format(
                                                            " = {0}", eventInstanceName))))
                                                                           ));
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            "Características do Event do Type {2} do Namespace {1} do Contexto {0} :",
                                                            contextname, namespaceOfContext, tipoFullName, eventInstanceName))),
                                                        Views.TD(Views.Text2TagHTML((evento.IsMulticast ? " IsMulticast" : "") +
                                                                                    (evento.IsSpecialName ? " SpecialName" : "")
                                                                     ))
                                                                           ));
                                                    //if (eventType != null)
                                                    //{
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.P(Views.Text2TagHTML("Event Type:"))),
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            " = {0}", eventType.FullName)))
                                                                           ));
                                                    //}
                                                    //if (eventAddMethod != null)
                                                    //{
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("Event Add Method:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0}", eventAddMethod)))
                                                                               ));
                                                    //}
                                                    if (eventRaiseMethod != null)
                                                    {
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("Event Raise Method:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0}", eventRaiseMethod)))
                                                                               ));
                                                    }
                                                    //if (eventRemoveMethod != null)
                                                    //{
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("Event Remove Method:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0}", eventRemoveMethod)))
                                                                               ));
                                                    //}
                                                    tagsHTMLnested.Add(Views.TR(
                                                        Views.TD(Views.P(Views.Text2TagHTML("GetOtherMethods Count:"))),
                                                        Views.TD(Views.Text2TagHTML(string.Format(
                                                            " = {0} OtherMethods(s).", eventOtherMethodsCount)))
                                                                           ));
                                                    foreach (var eventOtherMethod in eventOtherMethods)
                                                    {
                                                        tagsHTMLnested.Add(Views.TR(
                                                            Views.TD(Views.P(Views.Text2TagHTML("OtherMethod:"))),
                                                            Views.TD(Views.Text2TagHTML(string.Format(
                                                                " = {0}", eventOtherMethod)))
                                                                               ));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                tagsHTMLnested.Add(Views.TR(
                                                    Views.TD(Views.Text2TagHTML(string.Format("Type {0}", tipoFullName))),
                                                    Views.TD(Views.P(Views.Text2TagHTML(
                                                        string.Format(" não tem quaisquer Event(s) PUBLIC(s) com o nome requerido {0} !",
                                                                      eventName))))
                                                                       ));
                                            }
                                            // Type.GetEvents - public - END
                                        }
                                        else
                                        {
                                            tagsHTMLnested.Add(Views.TR(
                                                Views.TD(Views.P(
                                                    Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                     contextname, namespaceOfContext)),
                                                    Views.Text2TagHTML(string.Format(" deveria conter, mas não contém o Tipo requerido \"{0}\" !",
                                                                                     typeShortName)))
                                                    )));
                                        }
                                    }
                                    else
                                    {
                                        tagsHTMLnested.Add(Views.TR(
                                            Views.TD(Views.P(
                                                Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                                 contextname, namespaceOfContext)),
                                                Views.Text2TagHTML(string.Format(" não contém o Tipo requerido \"{0}\" !",
                                                                                 typeShortName)))
                                                )));
                                    }
                                }
                                else
                                {
                                    tagsHTMLnested.Add(Views.TR(
                                        Views.TD(Views.P(
                                            Views.Text2TagHTML(string.Format("O Namespace especificado \"{1}\" no Contexto {0}",
                                                                             contextname, namespaceOfContext)),
                                            Views.Text2TagHTML(string.Format(" não contém quaisquer tipo(s)!",
                                                                             listTypeNamesOfNamespace.Count)))
                                            )));
                                }
                            }
                            else
                            {
                                tagsHTMLnested.Add(Views.TR(
                                    Views.TD(Views.P(Views.Text2TagHTML(
                                        string.Format("Não existe o Namespace especificado \"{1}\" no Contexto {0} !",
                                                      contextname, namespaceName)))
                                        )));
                            }
                            tagsHTMLfull = Views.HTML(
                                Views.HEAD(Views.TITLE(string.Format("Propriedades de evento(s) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/e/{3}",
                                                                     contextname, namespaceName, typeShortName, eventName))),
                                Views.BODY(Views.H1(string.Format("Input = lista de informações de evento(s) de tipo de namespace de contexto: /{0}/ns/{1}/{2}/e/{3}",
                                                                     contextname, namespaceName, typeShortName, eventName)),
                                           Views.P(string.Format("(<=> {0}/{1}/ns/{2}/{3}/e/{4})", Handlers.DIRECTORIA_RAIZ,
                                                                    contextname, namespaceName, typeShortName, eventName)),
                                           Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                           Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                           Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                           Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns/{1}",
                                                                           contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                                           Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                                       contextname, namespaceName)))),
                                           Views.P(Views.A(true, string.Format("/{0}/ns/{1}/{2}",
                                                                           contextname, namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial(),
                                                                           typeShortName),
                                                           Views.Text2TagHTML(string.Format("[ link tipo namespace contexto /{0}/ns/{1}/{2} ]",
                                                                                       contextname, namespaceName, typeShortName)))),
                                           Views.TABLE(tagsHTMLnested.ToArray())
                                    )
                                );
                            if (isDataAvailable)
                            {
                                // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                                return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
                            }
                            else
                            {
                                // HTTP status 404 = NotFound (the requested resource does not exist on the server)
                                return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.NotFound, answerContent = tagsHTMLfull };
                            }
                        }
                        //eventName = eventName ?? "";
                        messageRangeOfError = "evento";
                        tagHTMLextra =
                            Views.P(Views.A(true,
                                            string.Format("/{0}/ns/{1}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                            Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                             contextname, namespaceName))),
                                    Views.A(true,
                                            string.Format("/{0}/ns/{1}/{2}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial(), typeShortName),
                                            Views.Text2TagHTML(string.Format("[ link tipo namespace contexto /{0}/ns/{1}/{2} ]",
                                                                             contextname, namespaceName, typeShortName))));
                    }
                    //typeShortName = typeShortName ?? "";
                    messageRangeOfError = messageRangeOfError ?? "tipo";
                    if (messageRangeOfError.Equals("tipo"))
                    {
                        tagHTMLextra =
                            Views.P(Views.A(true,
                                            string.Format("/{0}/ns/{1}", contextname,
                                                          namespaceName.IsNameOfNamespaceEmptyThenConvertToSpecial()),
                                            Views.Text2TagHTML(string.Format("[ link namespace contexto /{0}/ns/{1} ]",
                                                                             contextname, namespaceName))));
                    }
                }
                //namespaceName = namespaceName ?? "";
                messageRangeOfError = messageRangeOfError ?? "namespace";
            }
            contextname = contextname ?? "";
            messageRangeOfError = messageRangeOfError ?? "contexto";
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de evento de tipo de namespace de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de evento de tipo de namespace de contexto:"),
                                      Views.H3("Handler da Regra - ctx - ns - namespace - shortName - e - eventName"),
                                      Views.P(string.Format("Não se obteve valor do {0},", messageRangeOfError)),
                                      Views.P("apesar de haver mapeamento com ctx e namespace e shortName e eventName !"),
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link contexto /{0} ]", contextname)))),
                                      Views.P(Views.A(true, string.Format("/{0}/ns", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link namespaces contexto /{0}/ns ]", contextname)))),
                                        tagHTMLextra
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    #endregion
}
