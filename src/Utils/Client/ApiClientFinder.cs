using Floai.ApiClients.abs;
using System;
using System.Linq;
using System.Reflection;

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
            .SingleOrDefault(t => t.Namespace == _namespace 
            && t.IsClass 
            && t.Name == className + "ApiClient" 
            && t.BaseType == typeof(BaseApiClient))
            ?? throw new NullReferenceException();
        return apiClientType;
    }

    public Type GetApiClientOptionsClass(string className)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type optionsType = assembly.GetTypes()
            .SingleOrDefault(t => t.Namespace == _namespace 
            && t.IsClass 
            && t.Name == className + "ApiClientOptions" 
            && t.BaseType == typeof(BaseApiClientOptions))
            ?? throw new NullReferenceException();
        return optionsType;
    }
}
