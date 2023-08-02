using Floai.ApiClients.abs;
using System;
using System.Collections.Generic;
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

    public List<Type> GetApiClientClasses()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        List<Type> apiClientTypes = assembly.GetTypes()
            .Where(t => t.Namespace == _namespace
            && t.IsClass
            && t.Name != nameof(BaseApiClient) 
            && t.Name.EndsWith("ApiClient")
            && t.BaseType == typeof(BaseApiClient))
            .ToList();
        return apiClientTypes;
    }

    public List<Type> GetApiClientOptionsClasses()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        List<Type> optionsTypes = assembly.GetTypes()
            .Where(t => t.Namespace == _namespace
            && t.IsClass
            && t.Name != nameof(BaseApiClientOptions)
            && t.Name.EndsWith("ApiClientOptions")
            && t.BaseType == typeof(BaseApiClientOptions))
            .ToList();
        return optionsTypes;
    }

    public Type GetTargetApiClientClass(string className)
    {
        Type apiClientType = GetApiClientClasses()
            .SingleOrDefault(t => t.Name == className + "ApiClient")
            ?? throw new NullReferenceException();
        return apiClientType;
    }

    public Type GetTargetApiClientOptionsClass(string className)
    {
        Type optionsType = GetApiClientOptionsClasses()
            .SingleOrDefault(t => t.Name == className + "ApiClientOptions")
            ?? throw new NullReferenceException();
        return optionsType;
    }
}
