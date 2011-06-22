using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Reflection;

namespace WebReflector
{
    public static class MetodosEstaticos // the main features of a static class: Is sealed.
    {
        public const string NAMESPACE_SPECIAL = "*";

        #region 2 Metodos CheckForBothNullsOrBothNotNulls e CheckForCollectionsBothNullsOrWithSameCount _ private em Contracts _ para Contracts.cs

        public static bool CheckForBothNullsOrBothNotNulls(this Object object1, Object object2)
        {
            //if (((object1 == null) && (object2 != null)) || ((object1 != null) && (object2 == null))) // um null e outro nao null
            //{
            //    return false;
            //}
            //if ((object1 == null) && (object2 == null)) // dois nulls
            //{
            //    return true;
            //}
            //return true; // dois nao nulls
            return (((object1 == null) && (object2 == null)) || ((object1 != null) && (object2 != null)));
        }

        public static bool CheckForCollectionsBothNullsOrWithSameCount(this ICollection col1, ICollection col2)
        {
            if (!CheckForBothNullsOrBothNotNulls(col1, col2)) // um null e outro nao null
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

        #region Metodo LoadAndGetAssembly _ para carregar ficheiro como assembly com varias hipoteses comentadas _ Publico Auxiliar para Handlers.cs

        public static Assembly LoadAndGetAssembly(this string pathOfFile) // metodo escolhido Assembly.LoadFile(String) path of the file to load.
        {
            //return Assembly.LoadFrom(pathDoFicheiro); // The LoadFrom method has the following disadvantages.
            return Assembly.LoadFile(pathOfFile);
            //return Assembly.ReflectionOnlyLoadFrom(pathDoFicheiro); // Dependencies are not automatically loaded into the reflection-only context.
        }

        #endregion

        #region 2 Metodos Get...FromDir _ para povoar objectos necessarios _ Publico Auxiliar para Handlers.cs

        public static void GetAssembliesNamesFromDir(this DirectoryInfo dir, string contextName,
                                                        List<string> listAssembliesNames, List<TagHTML> tagsHTMLnested)
        {
            Assembly assemblyOfFileOfDir;
            foreach (var fileOfDir in dir.EnumerateFiles()) // GetFiles()
            {
                try
                {
                    assemblyOfFileOfDir = fileOfDir.FullName.LoadAndGetAssembly();
                    if (!listAssembliesNames.Contains(fileOfDir.Name)) // listAssembliesNamesOfContext
                    {
                        listAssembliesNames.Add(fileOfDir.Name); // listAssembliesNamesOfContext
                    }
                }
                catch (PathTooLongException) // pe
                {
                    tagsHTMLnested.Add(
                        Views.TR(Views.TD(
                            Views.Text2TagHTML(string.Format(
                                "Ficheiro {1} do Contexto {0} tem caminho muito longo!",
                                contextName, fileOfDir.Name))
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
                                contextName, fileOfDir.Name))
                                     ))
                        );
                    tagsHTMLnested.Add(
                        Views.TR(Views.TD(
                            Views.Text2TagHTML(string.Format(
                                "Excepção {3}: {1} !!!",
                                e.Source, e.Message, e.StackTrace, e.GetType().FullName))
                                     ))
                        ); // Excepção {0}: -mscorlib- StackTrace... {2}
                    continue;
                    //throw;
                }
            }
        }

        public static void GetAssembliesFromDir(this DirectoryInfo dir, string contextName,
                                                List<Assembly> listAssemblies, List<TagHTML> tagsHTMLnested, bool isExceptionDetailed)
        {
            Assembly assemblyOfFileOfDir;
            foreach (var fileOfDir in dir.EnumerateFiles()) // GetFiles()
            {
                try
                {
                    assemblyOfFileOfDir = fileOfDir.FullName.LoadAndGetAssembly();
                    listAssemblies.Add(assemblyOfFileOfDir); // listAssembliesOfContext
                }
                catch (PathTooLongException) // pe
                {
                    tagsHTMLnested.Add(
                        Views.TR(Views.TD(
                            Views.Text2TagHTML(string.Format(
                                "Ficheiro {1} do Contexto {0} tem caminho muito longo!",
                                contextName, fileOfDir.Name))
                                     ))
                        );
                    continue;
                }
                //catch (ReflectionTypeLoadException re)
                //{
                //    tagsHTMLnested.Add(
                //        Views.TR(Views.TD(
                //            Views.Text2TagHTML(string.Format(
                //                "Houve erro com Ficheiro {1} do Contexto {0}!",
                //                contextname, fileOfDir.Name))
                //                     ))
                //        );
                //    tagsHTMLnested.Add(
                //        Views.TR(Views.TD(
                //            Views.Text2TagHTML(string.Format(
                //                "Excepção {3}: {1} !!!",
                //                re.Source, re.Message, re.StackTrace, re.GetType().FullName))
                //                     ))
                //        ); // Excepção {0}: -mscorlib- StackTrace... {2}
                //    //foreach (var rle in re.LoaderExceptions)
                //    //{
                //    //    tagsHTMLnested.Add(
                //    //        Views.TR(Views.TD(
                //    //            Views.Text2TagHTML(string.Format(
                //    //                "==> Excepção {3}: {1} !!!",
                //    //                rle.Source, rle.Message, rle.StackTrace, rle.GetType().FullName))
                //    //                     ))
                //    //        ); // Excepção {0}: -mscorlib- StackTrace... {2}
                //    //}
                //    continue;
                //    //throw;
                //}
                catch (Exception e)
                {
                    tagsHTMLnested.Add(
                        Views.TR(Views.TD(
                            Views.Text2TagHTML(string.Format(
                                "Houve erro com Ficheiro {1} do Contexto {0}!",
                                contextName, fileOfDir.Name))
                                     ))
                        );
                    if (isExceptionDetailed)
                    {
                        tagsHTMLnested.Add(
                            Views.TR(Views.TD(
                                Views.Text2TagHTML(string.Format(
                                    "Excepção {3}: {1} !!!",
                                    e.Source, e.Message, e.StackTrace, e.GetType().FullName))
                                         ))
                            ); // Excepção {0}: -mscorlib- StackTrace... {2}
                    }
                    continue;
                    //throw;
                }
            }
        }

        #endregion

        #region Metodo GetFullNameFromType... _ para obter FullName de Type _ Publico Auxiliar para Handlers.cs

        public static string GetFullNameFromType(this Type type)
        {
            if (type == null) return "";
            string typeFullName;
            try
            {
                typeFullName = type.FullName;
            }
            catch (Exception)
            {
                typeFullName = "";
                //continue;
                //throw;
            }
            return typeFullName ?? "";
        }

        #endregion

        #region 3 Metodos para tratar possivel Exception no caso de NestedTypes _ Publicos Auxiliares para Handlers.cs

        public static bool IsNestedByCatchingAnyException(this Type type)
        {
            try
            {
                return type.IsNested;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }

        public static Type DeclaringTypeByCatchingAnyException(this Type type)
        {
            try
            {
                return type.DeclaringType;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }
        }

        public static Type[] GetNestedTypesByCatchingAnyException(this Type type)
        {
            try
            {
                return type.GetNestedTypes();
            }
            catch (Exception)
            {
                return new Type[0];
                //throw;
            }
        }

        #endregion

        #region 2 Metodos GetHierarchyFlatListTypes...FromType _ Publicos Auxiliares para Handlers.cs

        public static List<Type> GetHierarchyFlatListTypesSubNestedFromType(this Type type)
        {
            var types = new List<Type>();
            var subTypes = (type != null) ? type.GetNestedTypesByCatchingAnyException().Where(t => (t != null)).ToList() : new List<Type>();
            List<Type> typesToAnalize;
            while (subTypes.Count > 0)
            {
                typesToAnalize = new List<Type>();
                foreach (var subType in subTypes)
                {
                    if (!types.Contains(subType))
                    {
                        types.Add(subType);
                        typesToAnalize.AddRange(subType.GetNestedTypesByCatchingAnyException().Where(t => (t != null)));
                    }
                }
                subTypes = typesToAnalize;
            }
            return types;
        }

        public static List<Type> GetHierarchyFlatListTypesParentDeclaringFromType(this Type type)
        {
            var types = new List<Type>();
            var subType = type;
            Type typeToAnalize;
            while ((subType != null) && (subType.IsNestedByCatchingAnyException()))
            {
                typeToAnalize = subType.DeclaringTypeByCatchingAnyException();
                if ((typeToAnalize != null) && !types.Contains(typeToAnalize))
                {
                    types.Add(typeToAnalize);
                }
                subType = typeToAnalize;
            }
            return types;
        }

        #endregion

        #region 3 Metodos GetTypesFrom... Assembly Assemblies _ Publicos Auxiliares para Handlers.cs

        public static Type[] GetTypesFromAssembly(this Assembly assem, bool useAssemblyModules, bool includeResources,
                                                                        bool includeNestedTypes, bool includeParentTypes)
        {
            Type[] types;
            var listTypes = new List<Type>();
            if (!useAssemblyModules)
            {
                try
                {
                    types = assem.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types; // .Where(t => (t != null)).ToArray()
                    //continue;
                    //throw;
                }
                foreach (var type in types.Where(t => (t != null))) // .ToArray()
                {
                    if (!listTypes.Contains(type))
                    {
                        listTypes.Add(type);
                    }
                    if (includeNestedTypes)
                    {
                        foreach (var listType in type.GetHierarchyFlatListTypesSubNestedFromType())
                        {
                            if (!listTypes.Contains(listType))
                            {
                                listTypes.Add(listType);
                            }
                        }
                    }
                    if (includeParentTypes)
                    {
                        foreach (var listType in type.GetHierarchyFlatListTypesParentDeclaringFromType())
                        {
                            if (!listTypes.Contains(listType))
                            {
                                listTypes.Add(listType);
                            }
                        }
                    }
                }
            }
            else
            {
                var modulesOfassembly = assem.GetModules(includeResources);
                foreach (var module in modulesOfassembly)
                {
                    try
                    {
                        types = module.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        types = e.Types; // .Where(t => (t != null)).ToArray()
                        //continue;
                        //throw;
                    }
                    foreach (var type in types.Where(t => (t != null))) // .ToArray()
                    {
                        if (!listTypes.Contains(type))
                        {
                            listTypes.Add(type);
                        }
                        if (includeNestedTypes)
                        {
                            foreach (var listType in type.GetHierarchyFlatListTypesSubNestedFromType())
                            {
                                if (!listTypes.Contains(listType))
                                {
                                    listTypes.Add(listType);
                                }
                            }
                        }
                        if (includeParentTypes)
                        {
                            foreach (var listType in type.GetHierarchyFlatListTypesParentDeclaringFromType())
                            {
                                if (!listTypes.Contains(listType))
                                {
                                    listTypes.Add(listType);
                                }
                            }
                        }
                    }
                }
            }
            return listTypes.ToArray();
        }

        public static Type[] GetTypesFromAssembly(this Assembly assem, bool useAssemblyModules, bool includeResources)
        {
            // caso de nested types já foi tratado de forma mediana, pois não origina excepção, pelo que, colocam-se os 2 ultimos parametros a true
            //return GetTypesFromAssembly(assem, useAssemblyModules, includeResources, false, false);
            return GetTypesFromAssembly(assem, useAssemblyModules, includeResources, true, true);
        }


        public static List<Type> GetTypesFromAssemblies(this IEnumerable<Assembly> assemblies, bool useAssemblyModules, bool includeResources,
                                                                                            bool distinctTypesOnly)
        {
            var typesRes = new List<Type>();
            Type[] types;
            foreach (var assem in assemblies)
            {
                types = assem.GetTypesFromAssembly(useAssemblyModules, includeResources);
                foreach (var type in types)
                {
                    //if (type == null) continue;
                    if ((!distinctTypesOnly) || (!typesRes.Contains(type)))
                        typesRes.Add(type);
                }
            }
            return typesRes;
        }

        #endregion

        #region 3 Metodos GetNamespacesFromType... _ para escolha do melhor modo de obter Namespaces de Type _ Publicos Auxiliares para Handlers.cs

        //public static string GetNamespaceFromTypeName(this Type type)
        //{
        //    var typeName = type.FullName ?? ""; // type.Name;
        //    //if (typeName == null) return "";
        //    var typeNamespace = typeName.Contains(Type.Delimiter) ?
        //                                string.Join(Type.Delimiter.ToString(), typeName.Split(Type.Delimiter).Reverse().Skip(1).Reverse())
        //                                : ""; // : typeName;
        //    return typeNamespace;
        //}

        public static string GetNamespaceFromTypeNamespace(this Type type)
        {
            if (type == null) return "";
            string typeNamespace;
            try
            {
                typeNamespace = type.Namespace;
            }
            catch (Exception)
            {
                typeNamespace = "";
                //continue;
                //throw;
            }
            return typeNamespace ?? "";
        }

        public static string GetNamespaceFromTypeFullName(this Type type)
        {
            if (type == null) return "";
            var typeFullName = type.FullName ?? "";
            if (typeFullName.Contains(Type.Delimiter))
            {
                var typeNamespace = typeFullName.Split(Type.Delimiter).ToList();
                typeNamespace.Reverse();
                //if (typeNamespace.Count > 1)
                //{
                typeNamespace.RemoveAt(0);
                //}
                typeNamespace.Reverse();
                return string.Join(Type.Delimiter.ToString(), typeNamespace);
            }
            else
            {
                return "";
            }
        }

        public static string GetNamespaceFromTypeAssemblyQualifiedName(this Type type)
        {
            if (type == null) return "";
            var typeAssemblyQualifiedName = type.AssemblyQualifiedName ?? "";
            if (typeAssemblyQualifiedName.Contains(Type.Delimiter))
            {
                var typeNamespace = typeAssemblyQualifiedName.Split(',').ToList().First().Split(Type.Delimiter).ToList(); // First() = ElementAt(0) ?
                typeNamespace.Reverse();
                if (typeNamespace.Count > 1)
                {
                    typeNamespace.RemoveAt(0);
                }
                else
                {
                    return "";
                }
                typeNamespace.Reverse();
                return string.Join(Type.Delimiter.ToString(), typeNamespace);
            }
            else
            {
                return "";
            }
        }

        #endregion

        #region 4 Metodos GetNamespacesFrom... - Type Types Assembly Assemblies _ Publicos Auxiliares para Handlers.cs

        public static string GetNamespaceFromType(this Type type) // metodo escolhido foi o GetNamespaceFromTypeNamespace
        {
            return GetNamespaceFromTypeNamespace(type);
        }

        public static List<string> GetNamespacesFromTypes(this IEnumerable<Type> types, bool distinctNamespacesOnly)
        {
            var namespaces = new List<string>();
            if ((types != null) && (types.Count() > 0))
            {
                foreach (var type in types)
                {
                    if (type == null) continue;
                    var typeNamespace = type.GetNamespaceFromType();
                    if ((!distinctNamespacesOnly) || (!namespaces.Contains(typeNamespace)))
                        namespaces.Add(typeNamespace);
                }
            }
            return namespaces;
        }

        public static List<string> GetNamespacesFromAssembly(this Assembly assem, bool useAssemblyModules,
                                                                bool includeResources, bool distinctNamespacesOnly)
        {
            var namespaces = new List<string>();
            if (assem != null)
            {
                foreach (var typeNamespace in
                    assem.GetTypesFromAssembly(useAssemblyModules, includeResources).GetNamespacesFromTypes(distinctNamespacesOnly))
                {
                    if ((!distinctNamespacesOnly) || (!namespaces.Contains(typeNamespace)))
                    {
                        namespaces.Add(typeNamespace);
                    }
                }
            }
            return namespaces;
        }

        public static List<string> GetNamespacesFromAssemblies(this IEnumerable<Assembly> assemblies, bool useAssemblyModules,
                                                                bool includeResources, bool distinctNamespacesOnly)
        {
            var namespaces = new List<string>();
            foreach (var assem in assemblies)
            {
                foreach (var assemNamespace in assem.GetNamespacesFromAssembly(useAssemblyModules, includeResources, distinctNamespacesOnly))
                {
                    if ((!distinctNamespacesOnly) || (!namespaces.Contains(assemNamespace)))
                    {
                        namespaces.Add(assemNamespace);
                    }
                }
            }
            return namespaces;
        }

        #endregion

        #region 3 Metodos Get...FromAssembly... _ para escolher melhor modo de obter varios dados de Assembly _ Publicos Auxiliares para Handlers.cs

        public static string GetFriendlyNameFromAssembly(this Assembly assem)
        {
            if (assem == null)
            {
                return "";
            }
            return assem.GetName().Name;
        }

        //public static string GetPublicKeyStringFromAssemblyAndFile(this Assembly assem, string fullPathOfKeyFileToCreate)
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

        public static string GetPublicKeyStringFromAssemblyWithToken(this Assembly assem)
        {
            if (assem == null)
            {
                return "";
            }
            if (assem.GetName().GetPublicKeyToken().Length <= 0) // se nao houver token, nao se obtem chave sem atirar SecurityException
            {
                return "";
            }
            return BitConverter.ToString(assem.GetName().GetPublicKey());
        }

        public static string GetPublicKeyStringFromAssembly(this Assembly assem) // metodo escolhido foi o GetPublicKeyStringFromAssemblyWithToken
        {
            return GetPublicKeyStringFromAssemblyWithToken(assem);
        }

        #endregion

        #region 3 Metodos de Dictionary _ usar Dictionary e obter Namespaces e Type(Full)Names de Assemblies _ Publicos Auxiliares para Handlers.cs

        public static Dictionary<string, List<string>> GetDictNamespacesTypeFullNamesFromAssembly(this Assembly assem,
                                                                                                    bool useAssemblyModules, bool includeResources)
        {
            // .Distinct() antes de ToList
            var listTypesOfAssembly = assem.GetTypesFromAssembly(useAssemblyModules, includeResources).OrderBy(t => t.FullName).ToList();
            var dictNamespacesTypeNamesOfAssembly = new Dictionary<string, List<string>>();
            foreach (var type in listTypesOfAssembly)
            {
                var namespaceOfTypeOfAssembly = type.GetNamespaceFromType();
                if (!dictNamespacesTypeNamesOfAssembly.ContainsKey(namespaceOfTypeOfAssembly))
                {
                    dictNamespacesTypeNamesOfAssembly.Add(namespaceOfTypeOfAssembly, new List<string>());
                }
                var nameOfTypeOfAssembly = type.GetFullNameFromType();
                if (nameOfTypeOfAssembly == "")
                {
                    continue;
                }
                List<string> listTypeNamesOfAssembly = dictNamespacesTypeNamesOfAssembly[namespaceOfTypeOfAssembly];
                //if (!dictNamespacesTypeNamesOfAssembly.TryGetValue(namespaceOfTypeOfAssembly, out listTypeNamesOfAssembly))
                //{
                //    continue;
                //}
                if (!listTypeNamesOfAssembly.Contains(nameOfTypeOfAssembly))
                {
                    listTypeNamesOfAssembly.Add(nameOfTypeOfAssembly);
                }
            }
            return dictNamespacesTypeNamesOfAssembly;
        }

        public static void AppendDictWithNoDuplicates(this Dictionary<string,List<string>> dict, Dictionary<string,List<string>> dictToAppend)
        {
            string pairToAppendKey;
            List<string> pairToAppendValue;
            List<string> listStrings;
            foreach (var pairToAppend in dictToAppend)
            {
                pairToAppendKey = pairToAppend.Key;
                pairToAppendValue = pairToAppend.Value;
                if (!dict.ContainsKey(pairToAppendKey))
                {
                    listStrings = new List<string>();
                    foreach (var valueItem in pairToAppendValue)
                    {
                        listStrings.Add(valueItem);
                    }
                    dict.Add(string.Copy(pairToAppendKey),listStrings);
                }
                else
                {
                    listStrings = dict[pairToAppendKey];
                    foreach (var valueItem in pairToAppendValue)
                    {
                        if (!listStrings.Contains(valueItem))
                        {
                            listStrings.Add(valueItem);
                        }
                    }
                }
            }
        }

        public static Dictionary<string, List<string>> GetDictNamespacesTypeFullNamesFromAssemblies(this IEnumerable<Assembly> assemblies,
                                                                                                    bool useAssemblyModules, bool includeResources)
        {
            var dictNamespacesTypeNamesOfAssemblies = new Dictionary<string, List<string>>();
            foreach (var assem in assemblies)
            {
                dictNamespacesTypeNamesOfAssemblies.AppendDictWithNoDuplicates(
                    assem.GetDictNamespacesTypeFullNamesFromAssembly(useAssemblyModules, includeResources));
            }
            return dictNamespacesTypeNamesOfAssemblies;
        }

        #endregion

        #region 2 Metodos IsNameOfNamespace... _ para tratar o caso de Namespace nulo ou vazio _ Publicos Auxiliares para Handlers.cs
        
        public static string IsNameOfNamespaceEmptyThenConvertToSpecial(this string nameOfNamespace)
        {
            //if (nameOfNamespace == null)
            //{
            //    return "";
            //}
            if (nameOfNamespace == "") // caso Vazio para Especial
            {
                return NAMESPACE_SPECIAL;
            }
            return nameOfNamespace;
        }

        public static string IsNameOfNamespaceSpecialThenConvertToEmpty(this string nameOfNamespace)
        {
            //if (nameOfNamespace == null)
            //{
            //    return "";
            //}
            if (nameOfNamespace == NAMESPACE_SPECIAL) // caso Especial para Vazio
            {
                return "";
            }
            return nameOfNamespace;
        }

        #endregion

        #region 2 Metodos de Dictionary _ para usar Dictionary e filtrar apenas os TypeNames dos(e os) Namespaces Parents, sub-Namespaces

        public static Dictionary<string, List<string>> GetDictParentNamespacesTypeFullNamesFromNamespace(this Dictionary<string, List<string>> dict,
                                                                                                            string nameOfNamespace)
        {
            var namespc = string.Copy(nameOfNamespace ?? "");
            if (namespc.EndsWith("."))
            {
                namespc = namespc.Remove(namespc.Length - 1);
            }
            Dictionary<string, List<string>> dictRes = new Dictionary<string, List<string>>();
            foreach (var elemKeyValuePair in dict)
            {
                var elemKey = string.Copy(elemKeyValuePair.Key);
                if (!elemKey.EndsWith("."))
                {
                    elemKey = elemKey + ".";
                }
                if (namespc.StartsWith(elemKey))
                {
                    dictRes.Add(elemKeyValuePair.Key, elemKeyValuePair.Value);
                }
            }
            return dictRes;
        }

        public static Dictionary<string, List<string>> GetDictSubNamespacesTypeFullNamesFromNamespace(this Dictionary<string, List<string>> dict,
                                                                                                        string nameOfNamespace)
        {
            var namespc = string.Copy(nameOfNamespace ?? "");
            if (!namespc.EndsWith("."))
            {
                namespc = namespc + ".";
            }
            Dictionary<string, List<string>> dictRes = new Dictionary<string, List<string>>();
            foreach (var elemKeyValuePair in dict)
            {
                var elemKey = string.Copy(elemKeyValuePair.Key);
                if (elemKey.EndsWith("."))
                {
                    elemKey = elemKey.Remove(elemKey.Length - 1);
                }
                if (elemKey.StartsWith(namespc))
                {
                    dictRes.Add(elemKeyValuePair.Key, elemKeyValuePair.Value);
                }
            }
            return dictRes;
        }

        #endregion
    }
}
