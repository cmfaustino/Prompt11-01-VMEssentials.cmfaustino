using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace WebReflector
{
    class WebReflectorApp
    {
        static void Main(string[] args)
        {
            var contracts = new Contracts();
            contracts.AddRule("/", new RootHandler());
            contracts.AddRule("/{ctx}", new ContextNameHandler());
            contracts.AddRule("/{ctx}/as", new AssembliesContextHandler());
            contracts.AddRule("/{ctx}/ns", new NamespacesContextHandler());
            contracts.AddRule("/{ctx}/as/{assemblyName}", new AssemblyNameContextHandler());
            contracts.AddRule("/{ctx}/ns/{namespacePrefix}", new NamespacePrefixContextHandler());
            contracts.AddRule("/{ctx}/ns/{namespace}/{shortName}", new TypeShortNameNamespaceContextHandler());
            contracts.AddRule("/{ctx}/ns/{namespace}/{shortName}/m/{methodName}", new MethodNameTypeNamespaceContextHandler());
            contracts.AddRule("/{ctx}/ns/{namespace}/{shortName}/c", new ConstructsTypeNamespaceContextHandler());
            contracts.AddRule("/{ctx}/ns/{namespace}/{shortName}/f/{fieldName}", new FieldNameTypeNamespaceContextHandler());
            contracts.AddRule("/{ctx}/ns/{namespace}/{shortName}/p/{propName}", new PropNameTypeNamespaceContextHandler());
            contracts.AddRule("/{ctx}/ns/{namespace}/{shortName}/e/{eventName}", new EventNameTypeNamespaceContextHandler());

            //var statusCodesHTTP = new Dictionary<int, HttpStatusCode>();
            //statusCodesHTTP.Add(400,HttpStatusCode.BadRequest);
            //statusCodesHTTP.Add(501,HttpStatusCode.NotImplemented);
            //statusCodesHTTP.Add(200,HttpStatusCode.OK);
            //HttpStatusCode statusCodeHTTP;

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }
            else
            {
                var listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:8080/"); // tem de terminar com barra, de acordo com MSDN de HttpListener Class
                //listener.Prefixes.Add("http://127.0.0.1:8080/"); // tem de terminar com barra, de acordo com MSDN de HttpListener Class
                ////listener.Prefixes.Add("http://::1:8080/"); // tem de terminar com barra, de acordo com MSDN de HttpListener Class
                //listener.Prefixes.Add("http://*:8080/"); // terminar com barra, de acordo com MSDN de HttpListener Class, e com port, host pode ser *
                listener.Start();

                bool fimDeServico = false;

                string htmlDeSaida;
                byte[] buffer;

                while (!fimDeServico)
                {
                    var listenerContext = listener.GetContext();
                    var request = listenerContext.Request;
                    var response = listenerContext.Response;
                    var output = response.OutputStream; // listener.(get)context.response já tem stream de output (para cliente) inicializado
                    //var requestUrl = request.Url;
                    // UnescapeDataString Converts to unescaped representation // EscapeDataString converts to hexadecimal representation
                    var requestUrlText = Uri.UnescapeDataString(request.RawUrl); // UrlDecode // request.Url.AbsolutePath // request.RawUrl
                    if (requestUrlText == "/-") // SAIDA
                    {
                        htmlDeSaida =
                            Views.processTagHTML2string(
                                Views.HTML(Views.HEAD(Views.TITLE("Encerramento da Aplicação Servidora")),
                                           Views.BODY(Views.H1("Encerramento da Aplicação Servidora ( /- ):"),
                                                      Views.H3(
                                                          "Obrigado por utilizar esta Aplicação Servidora!"),
                                                      Views.P(
                                                          "Para possibilitar o atendimento de novos pedidos,"),
                                                      Views.P(
                                                          "será necessário executá-la novamente, a partir do servidor,"),
                                                      Views.P("e abrir um browser para"),
                                                      Views.TABLE(Views.TR(Views.TD(Views.A(true, "/",
                                                                                            Views.Text2TagHTML(
                                                                                                "o endereço deste servidor.")))))
                                               )),
                                Views.Text2TagHTML("HTTP STATUS CODE: " + HttpStatusCode.OK));

                        buffer = System.Text.Encoding.UTF8.GetBytes(htmlDeSaida);
                        // HTTP status 200 = OK (request succeeded and that the requested information is in the response)
                        response.StatusCode = (int)HttpStatusCode.OK;

                        response.ContentType = "text/html";
                        response.ContentLength64 = buffer.Length;
                        output.Write(buffer, 0, buffer.Length);
                        response.Close();

                        fimDeServico = true; // alterar variavel, para se fazer o fim do servico, quebrando o ciclo while
                    }
                    else // NAO SAIR, CONTINUA O ATENDIMENTO DE PEDIDOS NO CICLO WHILE
                    {
                        HandlerAnswer respostaHTMLdeSaida;
                        try
                        {
                            respostaHTMLdeSaida = Router.MatchAndExecute(contracts, requestUrlText);
                            htmlDeSaida =
                                Views.processTagHTML2string(
                                    Views.A(true, "/-", Views.Text2TagHTML("PARA ENCERRAR ESTA APLICAÇÃO SERVIDORA, CLIQUE AQUI = /-")),
                                    respostaHTMLdeSaida.answerContent,
                                    Views.Text2TagHTML("HTTP STATUS CODE: " + respostaHTMLdeSaida.answerCode));
                        }
                        catch (Exception exception) // EXCEPCAO
                        {
                            var innerTagHTML = new List<TagHTML>();
                            if (exception.Data != null)
                            {
                                innerTagHTML.Add(Views.P("------ Detalhes Extra ------"));
                                var innerTableHTML = new List<TagHTML>();
                                innerTableHTML.Add(Views.TR(Views.TD(Views.Text2TagHTML("Key")),
                                                            Views.TD(Views.Text2TagHTML("Value"))));
                                foreach (DictionaryEntry de in exception.Data)
                                {
                                    innerTableHTML.Add(Views.TR(Views.TD(Views.Text2TagHTML((string) de.Key)),
                                                                Views.TD(Views.Text2TagHTML((string) de.Value))));
                                }
                                innerTagHTML.Add(Views.TABLE(innerTableHTML.ToArray()));
                            }
                            innerTagHTML.Reverse();
                            innerTagHTML.Add(Views.PRE(exception.StackTrace));
                            innerTagHTML.Add(Views.P(exception.Message));
                            //innerTagHTML.Add(Views.H3(exception.Source));
                            innerTagHTML.Add(Views.H3(exception.GetType().FullName));
                            innerTagHTML.Add(Views.H1("Excepção Ocorrida:"));
                            innerTagHTML.Add(Views.P(Views.A(true, "/", Views.Text2TagHTML("[ link root / ]"))));
                            innerTagHTML.Add(Views.A(true, "/-", Views.Text2TagHTML("PARA ENCERRAR ESTA APLICAÇÃO SERVIDORA, CLIQUE AQUI = /-")));
                            innerTagHTML.Reverse();
                            // HTTP status 500 = InternalServerError (generic error has occurred on the server)
                            respostaHTMLdeSaida = new HandlerAnswer()
                                                      {
                                                          answerOk = false,
                                                          answerCode = HttpStatusCode.InternalServerError,
                                                          answerContent =
                                                              Views.HTML(Views.HEAD(Views.TITLE("Excepção Ocorrida")),
                                                                         Views.BODY(innerTagHTML.ToArray()))
                                                      };
                            htmlDeSaida = Views.processTagHTML2string(respostaHTMLdeSaida.answerContent,
                                    Views.Text2TagHTML("HTTP STATUS CODE: " + respostaHTMLdeSaida.answerCode));

                            //throw exception; // poder-se-ia fazer um ficheiro de log, com os dados da excepcao
                        }

                        buffer = System.Text.Encoding.UTF8.GetBytes(htmlDeSaida);

                        //if (!respostaHTMLdeSaida.answerOk)
                        //{
                        //    response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        //}
                        //else
                        //{
                        response.StatusCode = (int)respostaHTMLdeSaida.answerCode;
                        //}

                        response.ContentType = "text/html";
                        response.ContentLength64 = buffer.Length;
                        output.Write(buffer, 0, buffer.Length);
                        response.Close();
                    }
                }
                listener.Stop();
                listener.Close();
            }
        }
    }
}
