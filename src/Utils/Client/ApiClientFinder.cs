using Floai.ApiClients;
using Floai.ApiClients.abs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Floai.Utils.Client;

public class ApiClientFinder
{
    private readonly string _namespace;

    public ApiClientFinder(string namespaceName)
    {
        _namespace = namespaceName;
    }

    public Type GetApiClientClass(string className)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type apiClientType = assembly.GetTypes()
            .SingleOrDefault(t => t.Namespace == _namespace && t.IsClass && t.Name == className + "ApiClient" && t.BaseType == typeof(BaseApiClient));
        return apiClientType;
    }

    public Type GetApiClientOptionsClass(string className)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type optionsType = assembly.GetTypes()
            .SingleOrDefault(t => t.Namespace == _namespace && t.IsClass && t.Name == className + "ApiClientOptions");
        //foreach(var type in assembly.GetTypes())
        //{
        //    if (type.FullName == "Floai.ApiClients.OpenAiApiClientOptions")
        //    {
        //        Debug.WriteLine("e");
        //    }
        //}
        return optionsType;
    }
}
