using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace FullInspector.Internal
{
    public static class TypeCache
    {
        private static Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();
        private static Dictionary<string, Assembly> _assembliesByName = new Dictionary<string, Assembly>();
        private static List<Assembly> _assembliesByIndex = new List<Assembly>();

        static TypeCache()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                TypeCache._assembliesByName[assembly.FullName] = assembly;
                TypeCache._assembliesByIndex.Add(assembly);
            }
            TypeCache._cachedTypes = new Dictionary<string, Type>();
            AppDomain currentDomain = AppDomain.CurrentDomain;
            // ISSUE: reference to a compiler-generated field
            if (TypeCache._f__mg_cache0 == null)
            {
                // ISSUE: reference to a compiler-generated field
                TypeCache._f__mg_cache0 = new AssemblyLoadEventHandler(TypeCache.OnAssemblyLoaded);
            }
            // ISSUE: reference to a compiler-generated field
            AssemblyLoadEventHandler fMgCache0 = TypeCache._f__mg_cache0;
            currentDomain.AssemblyLoad += fMgCache0;
        }

        private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            TypeCache._assembliesByName[args.LoadedAssembly.FullName] = args.LoadedAssembly;
            TypeCache._assembliesByIndex.Add(args.LoadedAssembly);
            TypeCache._cachedTypes = new Dictionary<string, Type>();
        }

        private static bool TryDirectTypeLookup(string assemblyName, string typeName, out Type type)
        {
            Assembly assembly;
            if (assemblyName != null && TypeCache._assembliesByName.TryGetValue(assemblyName, out assembly))
            {
                type = assembly.GetType(typeName, false);
                return type != null;
            }
            type = (Type) null;
            return false;
        }

        private static bool TryIndirectTypeLookup(string typeName, out Type type)
        {
            for (int index = 0; index < TypeCache._assembliesByIndex.Count; ++index)
            {
                Assembly assembly = TypeCache._assembliesByIndex[index];
                type = assembly.GetType(typeName);
                if (type != null)
                    return true;
                foreach (Type type1 in assembly.GetTypes())
                {
                    if (type1.FullName == typeName)
                    {
                        type = type1;
                        return true;
                    }
                }
            }
            type = (Type) null;
            return false;
        }

        public static void Reset() => TypeCache._cachedTypes = new Dictionary<string, Type>();

        public static Type FindType(string name) => TypeCache.FindType(name, (string) null);

        public static Type FindType(string name, string assemblyHint)
        {
            if (string.IsNullOrEmpty(name))
                return (Type) null;
            Type type;
            if (!TypeCache._cachedTypes.TryGetValue(name, out type))
            {
                if (TypeCache.TryDirectTypeLookup(assemblyHint, name, out type) || TypeCache.TryIndirectTypeLookup(name, out type))
                    ;
                TypeCache._cachedTypes[name] = type;
            }
            return type;
        }
    }
}
