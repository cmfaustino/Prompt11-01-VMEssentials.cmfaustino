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
            @"C:\z_PROMPT\PROMPT\uc02\repos\PROMPT11-02-AdvancedProgramming\src\packages\NUnit.2.5.10.11092\tools"; // tem sub-pasta lib
        //@"C:\z_PROMPT\PROMPT\uc02\repos\PROMPT11-02-AdvancedProgramming\src\Mod02-AdvProgramming.Utils\bin\Debug";
        //@"C:\z_PROMPT\PROMPT\uc02\repos\PROMPT11-02-AdvancedProgramming\src";
        public const string URL_WEBSITE_PROCESSAMENTO = ""; // @"http://";

        #region 6 Metodos Publicos Auxiliares GetNamespacesFrom...

        //public static string GetNamespaceFromTypeName(this string tipoNome)
        //{
        //    if (tipoNome == null) return "";
        //    var tipoNamespace = tipoNome.Contains(".") ?
        //                                string.Join(".", tipoNome.Split('.').Reverse().Skip(1).Reverse())
        //                                : tipoNome;
        //    return tipoNamespace;
        //}

        public static string GetNamespaceFromTypeSimple(this Type tipo)
        {
            if (tipo == null) return "";
            var tipoNamespace = tipo.Namespace;
            return tipoNamespace ?? "";
        }
        
        public static string GetNamespaceFromTypeFullName(this Type tipo)
        {
            if (tipo == null) return "";
            var tipoFullName = tipo.FullName;
            if ((tipoFullName != null) && tipoFullName.Contains("."))
            {
                var tipoNamespace = tipoFullName.Split('.').ToList();
                tipoNamespace.Reverse();
                if (tipoNamespace.Count > 1)
                {
                    tipoNamespace.RemoveAt(0);
                }
                tipoNamespace.Reverse();
                return string.Join(".", tipoNamespace);
            }
            else
            {
                return "";
            }
        }

        public static string GetNamespaceFromTypeAssemblyQualifiedName(this Type tipo)
        {
            if (tipo == null) return "";
            var tipoAssemblyQualifiedName = tipo.AssemblyQualifiedName;
            if ((tipoAssemblyQualifiedName != null) && tipoAssemblyQualifiedName.Contains("."))
            {
                var tipoNamespace = tipoAssemblyQualifiedName.Split(',').ToList().ElementAt(0).Split('.').ToList();
                tipoNamespace.Reverse();
                if (tipoNamespace.Count > 1)
                {
                    tipoNamespace.RemoveAt(0);
                }
                tipoNamespace.Reverse();
                return string.Join(".", tipoNamespace);
            }
            else
            {
                return "";
            }
        }
        public static string GetNamespaceFromType(this Type tipo) // metodo escolhido foi o GetNamespaceFromTypeSimple
        {
            return GetNamespaceFromTypeSimple(tipo);
        }

        public static List<string> GetNamespacesFromTypes(this Type[] arrTypes, bool distinctNamespacesOnly)
        {
            var arrNamespaces = new List<string>();
            if((arrTypes != null) && (arrTypes.Length > 0))
            {
                foreach (var moduleTipo in arrTypes)
                {
                    if (moduleTipo == null) continue;
                    var moduleTipoNamespace = moduleTipo.GetNamespaceFromType();
                    if ((!distinctNamespacesOnly) || (!arrNamespaces.Contains(moduleTipoNamespace)))
                        arrNamespaces.Add(moduleTipoNamespace);
                }
            }
            return arrNamespaces;
        }

        public static List<string> GetNamespacesFromAssembly(this Assembly assem, bool includeResources, bool distinctNamespacesOnly)
        {
            var arrNamespaces = new List<string>();
            if (assem != null)
            {
                var modulesOfassembly = assem.GetModules(includeResources);
                foreach (var module in modulesOfassembly)
                {
                    foreach (var moduleTipoNamespace in module.GetTypes().GetNamespacesFromTypes(distinctNamespacesOnly))
                    {
                        if ((!distinctNamespacesOnly) || (!arrNamespaces.Contains(moduleTipoNamespace)))
                        {
                            arrNamespaces.Add(moduleTipoNamespace);
                        }
                    }
                }
            }
            return arrNamespaces;
        }

        #endregion

        #region Metodo Publico Auxiliar GetPublicKeyStringFrom...

        //public static string GetPublicKeyStringFromAssembly(this Assembly assem, string fullPathOfKeyFileToCreate)
        //{
        //    //StrongNameKeyPair(FileStream)
        //    //return BitConverter.ToString(new StrongNameKeyPair(File.Create(fullPathOfKeyFileToCreate)).PublicKey);
        //    //StrongNameKeyPair(SerializationInfo, StreamingContext)
        //    //FormatterConverter fc = new FormatterConverter();
        //    //SerializationInfo serialinfo = new SerializationInfo(assem.GetType(),fc);
        //    //StreamingContext streamcontext;
        //    //assem.GetObjectData(serialinfo, streamcontext);
        //    //return BitConverter.ToString(new StrongNameKeyPair(serialinfo, streamcontext).PublicKey);
        //    //var assemManifModuleToken = assem.ManifestModule.MetadataToken;
        //}

        #endregion
    }

    #region 3 Componentes Principais - Classe HandlerAnswer, Interface IHandler, Classe Excepcao HandlerException

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

    #endregion

    #region Componente Auxiliar - Classe Nulls Handler

    public class NullsHandler : IHandler
    {
        private string uriPedido;
        private bool IsHandlerNull_ElseContractsNull;

        public NullsHandler(string uriPedido, bool isHandlerNullElseContractsNull)
        {
            this.uriPedido = uriPedido;
            this.IsHandlerNull_ElseContractsNull = isHandlerNullElseContractsNull;
        }

        public HandlerAnswer Handle()
        {
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
            foreach (var subdir in dir.GetDirectories())
                tagsHTMLnested.Add(
                    Views.TR(
                        Views.TD(Views.A(true, string.Format("{0}/{1}", Handlers.URL_WEBSITE_PROCESSAMENTO, subdir.Name),
                                         Views.Text2TagHTML(string.Format("Contexto {0}", subdir.Name))
                                     )))
                    );
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

            string contextname;
            TagHTML tagsHTMLfull;
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
                                                 Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link / ]")))
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

            string contextname;
            TagHTML tagsHTMLfull;
            if (prms.TryGetValue("ctx", out contextname))
            {
                var tagsHTMLnested = new List<TagHTML>();
                var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                foreach (var fileOfDir in dir.GetFiles())
                {
                    try
                    {
                        var assemblyOfFileOfDir = Assembly.LoadFrom(fileOfDir.FullName); // .ReflectionOnlyLoadFrom .LoadFile .LoadFrom
                        tagsHTMLnested.Add(
                            Views.TR(
                                Views.TD(Views.A(true,
                                                 string.Format("{0}/{1}/as/{2}", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                               contextname, fileOfDir.Name),
                                                 Views.Text2TagHTML(string.Format("Assembly {1} do Contexto {0}",
                                                                                  contextname, fileOfDir.Name))
                                             )))
                            );
                    }
                    catch(PathTooLongException pe)
                    {
                        tagsHTMLnested.Add(
                            Views.TR(Views.TD(
                                Views.Text2TagHTML(string.Format(
                                    "Ficheiro {1} do Contexto {0} tem caminho muito longo!",
                                    contextname, fileOfDir.Name))
                                         ))
                            );
                        continue;
                    }
                    catch (Exception e)
                    {
                        tagsHTMLnested.Add(
                            Views.TR(Views.TD(
                                Views.Text2TagHTML(string.Format(
                                    "Houve erro com Ficheiro {1} do Contexto {0}!",
                                    contextname, fileOfDir.Name))
                                         ))
                            );
                        tagsHTMLnested.Add(
                            Views.TR(Views.TD(
                                Views.Text2TagHTML(string.Format(
                                    "{1} !!! StackTrace... {2}",
                                    e.Source, e.Message, e.StackTrace))
                                         ))
                            ); // Excepção {0}: 
                        continue;
                        //throw;
                    }
                }
                tagsHTMLfull = Views.HTML(
                    Views.HEAD(Views.TITLE(string.Format("Propriedades de lista de assemblies de contexto: /{0}/as",
                                                         contextname))),
                    Views.BODY(Views.H1(string.Format("Input = lista de assemblies de contexto: /{0}/as", contextname)),
                               Views.P(string.Format("(<=> {0})", dir.FullName)),
                               Views.P(Views.A(true, string.Format("/{0}", contextname),
                                               Views.Text2TagHTML(string.Format("[ link /{0} ]", contextname)))),
                               Views.TABLE(tagsHTMLnested.ToArray())
                        )
                    );
                // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
            }
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de lista de assemblies de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de lista de assemblies de contexto:"),
                                      Views.H3("Handler da Regra - ctx - as"),
                                      Views.P("Não se obteve valor do contexto,"),
                                      Views.P("apesar de haver mapeamento com ctx !"),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link /{0} ]", contextname))))
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    public class NamespacesContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            //Console.WriteLine("...NamespacesContextHandler...");

            string contextname;
            TagHTML tagsHTMLfull;
            if (prms.TryGetValue("ctx", out contextname))
            {
                var listNamespacesOfContext = new List<string>();
                var tagsHTMLnested = new List<TagHTML>();
                var dir = new DirectoryInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname);
                foreach (var fileOfDir in dir.GetFiles())
                {
                    Assembly assemblyOfFileOfDir;
                    try
                    {
                        assemblyOfFileOfDir = Assembly.LoadFrom(fileOfDir.FullName); // .ReflectionOnlyLoadFrom .LoadFile .LoadFrom
                        foreach (var assemblyNamespace in assemblyOfFileOfDir.GetNamespacesFromAssembly(false, true))
                        {
                            if (!listNamespacesOfContext.Contains(assemblyNamespace))
                            {
                                listNamespacesOfContext.Add(assemblyNamespace);
                            }
                        }
                    }
                    catch (PathTooLongException pe)
                    {
                        tagsHTMLnested.Add(
                            Views.TR(Views.TD(
                                Views.Text2TagHTML(string.Format(
                                    "Ficheiro {1} do Contexto {0} tem caminho muito longo!",
                                    contextname, fileOfDir.Name))
                                         ))
                            );
                        continue;
                    }
                    catch (Exception e)
                    {
                        tagsHTMLnested.Add(
                            Views.TR(Views.TD(
                                Views.Text2TagHTML(string.Format(
                                    "Houve erro com Ficheiro {1} do Contexto {0}!",
                                    contextname, fileOfDir.Name))
                                         ))
                            );
                        tagsHTMLnested.Add(
                            Views.TR(Views.TD(
                                Views.Text2TagHTML(string.Format(
                                    "{1} !!! StackTrace... {2}",
                                    e.Source, e.Message, e.StackTrace))
                                         ))
                            ); // Excepção {0}: 
                        continue;
                        //throw;
                    }
                }
                foreach (var namespaceOfContext in listNamespacesOfContext)
                {
                    tagsHTMLnested.Add(
                        Views.TR(
                            Views.TD(Views.A(true,
                                             string.Format("{0}/{1}/ns/{2}", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                           contextname, namespaceOfContext),
                                             Views.Text2TagHTML(string.Format("Namespace {1} do Contexto {0}",
                                                                              contextname, namespaceOfContext))
                                         )))
                        );
                }
                tagsHTMLfull = Views.HTML(
                    Views.HEAD(Views.TITLE(string.Format("Propriedades de lista de namespaces de contexto: /{0}/ns",
                                                         contextname))),
                    Views.BODY(Views.H1(string.Format("Input = lista de namespaces de contexto: /{0}/ns", contextname)),
                               Views.P(string.Format("(<=> {0})", dir.FullName)),
                               Views.P(Views.A(true, string.Format("/{0}", contextname),
                                               Views.Text2TagHTML(string.Format("[ link /{0} ]", contextname)))),
                               Views.TABLE(tagsHTMLnested.ToArray())
                        )
                    );
                // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                return new HandlerAnswer() { answerOk = true, answerCode = HttpStatusCode.OK, answerContent = tagsHTMLfull };
            }
            tagsHTMLfull =
                Views.HTML(Views.HEAD(Views.TITLE("Erro nas Propriedades de lista de namespaces de contexto")),
                           Views.BODY(Views.H1("Erro nas Propriedades de lista de namespaces de contexto:"),
                                      Views.H3("Handler da Regra - ctx - ns"),
                                      Views.P("Não se obteve valor do contexto,"),
                                      Views.P("apesar de haver mapeamento com ctx !"),
                                      Views.P(Views.A(true, string.Format("/{0}", contextname),
                                                      Views.Text2TagHTML(string.Format("[ link /{0} ]", contextname))))
                               ));
            // HTTP status 409 = Conflict (the request could not be carried out because of a conflict on the server)
            return new HandlerAnswer() { answerOk = false, answerCode = HttpStatusCode.Conflict, answerContent = tagsHTMLfull };
        }
    }

    public class AssemblyNameContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            //Console.WriteLine("...AssemblyNameContextHandler...");
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string assemblyname;
                if (prms.TryGetValue("assemblyName", out assemblyname))
                {
                    var textHTMLnested = new List<TagHTML>();
                    var fileOfDir = new FileInfo(Handlers.DIRECTORIA_RAIZ + "/" + contextname + "/" + assemblyname);
                    Assembly assemblyOfFileOfDir;
                    try
                    {
                        assemblyOfFileOfDir = Assembly.LoadFrom(fileOfDir.FullName);

                        textHTMLnested.Add(
                            Views.TR( Views.TD(
                                                            Views.Text2TagHTML(string.Format("Frindly Name do Assembly {1} do Contexto {0}",
                                                                                  contextname, assemblyname))
                                    ))
                            );
                        textHTMLnested.Add(
                            Views.TR( Views.TD(
                                                            Views.Text2TagHTML(string.Format(" = {0}",
                                                                                  assemblyOfFileOfDir.GetName().Name))
                                    ))
                            );
                        textHTMLnested.Add(
                            Views.TR(Views.TD(
                                                            Views.Text2TagHTML(string.Format("Public Key do Assembly {1} do Contexto {0}",
                                                                                  contextname, assemblyname))
                                    ))
                            );
                        textHTMLnested.Add(
                            Views.TR(Views.TD(
                                                            Views.Text2TagHTML(string.Format(" = {0}", BitConverter.ToString(
                                                                                                    assemblyOfFileOfDir.GetName().GetPublicKey()
                                                                                )))
                                    ))
                            );
                        var listTypesOfAssembly = assemblyOfFileOfDir.GetTypes().OrderBy(t => t.FullName).ToArray(); // .Distinct() antes de ToArray
                        var listNamespacesOfAssembly = new List<string>();
                        var listTypeNamesOfAssembly = new List<string>();
                        foreach (var tipo in listTypesOfAssembly)
                        {
                            var namespaceOfTypeOfAssembly = tipo.GetNamespaceFromType();
                            if (!listNamespacesOfAssembly.Contains(namespaceOfTypeOfAssembly))
                            {
                                listNamespacesOfAssembly.Add(namespaceOfTypeOfAssembly);
                                textHTMLnested.Add(
                                    Views.TR(Views.TD(Views.A(true,
                                                         string.Format("{0}/{1}/ns/{2}", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                                       contextname, namespaceOfTypeOfAssembly),
                                                         Views.Text2TagHTML(string.Format("Namespace {1} do Contexto {0} contém tipo(s):",
                                                                                          contextname, namespaceOfTypeOfAssembly))
                                                     )))
                                    );
                            }
                            if(!listTypeNamesOfAssembly.Contains(tipo.FullName))
                            {
                                listTypeNamesOfAssembly.Add(tipo.FullName);
                                textHTMLnested.Add(
                                    Views.TR( Views.TD(Views.A(true,
                                                         string.Format("{0}/{1}/ns/{2}/{3}", Handlers.URL_WEBSITE_PROCESSAMENTO,
                                                                       contextname, namespaceOfTypeOfAssembly, tipo.Name),
                                                         Views.Text2TagHTML(string.Format("Tipo {2} do Namespace {1} do Contexto {0}",
                                                                                          contextname, namespaceOfTypeOfAssembly, tipo.Name))
                                                     )))
                                    );
                            }
                        }
                    }
                    catch (PathTooLongException pe)
                    {
                        textHTMLnested.Add(
                            Views.TR(Views.TD(
                                                             Views.Text2TagHTML(string.Format("Assembly {1} do Contexto {0} tem caminho muito longo!",
                                                                                            contextname, fileOfDir.Name))
                                    ))
                        );
                    }
                    catch (Exception e)
                    {
                        textHTMLnested.Add(
                            Views.TR(Views.TD(
                                                             Views.Text2TagHTML(string.Format("Assembly {1} do Contexto {0} é inválido como assembly,"
                                                                                                + " ou inexistente,"
                                                                                                + " ou não tem permissões de acesso/leitura!",
                                                                                            contextname, fileOfDir.Name))
                                    ))
                        );
                        //throw;
                    }
                    var textHTMLfull = Views.processTagHTML2string(Views.HTML(
                                                    Views.HEAD(Views.TITLE(string.Format("Propriedades de assembly de contexto: /{0}/as/{1}",
                                                                                        contextname, assemblyname))),
                                                    Views.BODY(Views.H1(string.Format("Input = lista de informações de assembly de contexto:"
                                                                                        + " /{1}/as/{2} (<=> {0}/{1}/as/{2})",
                                                                                            Handlers.DIRECTORIA_RAIZ, contextname, assemblyname)),
                                                                Views.TABLE(textHTMLnested.ToArray())
                                                            )
                                        ));
                    //...falta...
                }
                //...falta...
            }
            //...falta...
            return new HandlerAnswer();
        }
    }

    #endregion

    #region 1 Especificacao -Enunciado- NamespacePrefixContextHandler

    public class NamespacePrefixContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            //Console.WriteLine("...NamespacePrefixContextHandler...");
            string contextname;
            if (prms.TryGetValue("ctx", out contextname))
            {
                string namespacePrefix;
                if (prms.TryGetValue("namespacePrefix", out namespacePrefix))
                {
                    var textHTMLnested = new List<TagHTML>();
                    //...falta...
                }
                //...falta...
            }
            //...falta...
            return new HandlerAnswer();
        }
    }

    #endregion

    #region 3 Especificacoes -Enunciado- TypeShortNameNamespaceContext... MethodNameTypeNamespaceContext... ConstructsTypeNamespaceContext...

    public class TypeShortNameNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            //Console.WriteLine("...TypeShortNameNamespaceContextHandler...");
            return new HandlerAnswer();
        }
    }

    public class MethodNameTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            //Console.WriteLine("...MethodNameTypeNamespaceContextHandler...");
            return new HandlerAnswer();
        }
    }

    public class ConstructsTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            //Console.WriteLine("...ConstructsTypeNamespaceContextHandler...");
            return new HandlerAnswer();
        }
    }

    #endregion

    #region 3 Especificacoes -Enunciado- FieldTypeNamespaceContext... PropTypeNamespaceContext... EventTypeNamespaceContext...

    public class FieldTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            //Console.WriteLine("...FieldTypeNamespaceContextHandler...");
            return new HandlerAnswer();
        }
    }

    public class PropTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            //Console.WriteLine("...PropTypeNamespaceContextHandler...");
            return new HandlerAnswer();
        }
    }

    public class EventTypeNamespaceContextHandler : IHandler
    {
        public HandlerAnswer Handle(Dictionary<string, string> prms)
        {
            //Console.WriteLine("...EventTypeNamespaceContextHandler...");
            return new HandlerAnswer();
        }
    }

    #endregion
}
