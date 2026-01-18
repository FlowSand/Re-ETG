using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FullSerializer;
using FullSerializer.Internal;

#nullable disable
namespace FullInspector.Internal
{
    public class fiRuntimeReflectionUtility
    {
        private static List<Assembly> _cachedRuntimeAssemblies;
        private static List<Assembly> _cachedUserDefinedEditorAssemblies;
        private static List<Assembly> _cachedAllEditorAssembles;

        public static object InvokeStaticMethod(System.Type type, string methodName, object[] parameters)
        {
            try
            {
                return type.GetFlattenedMethod(methodName).Invoke((object) null, parameters);
            }
            catch
            {
            }
            return (object) null;
        }

        public static object InvokeStaticMethod(string typeName, string methodName, object[] parameters)
        {
            return fiRuntimeReflectionUtility.InvokeStaticMethod(TypeCache.FindType(typeName), methodName, parameters);
        }

        public static void InvokeMethod(
            System.Type type,
            string methodName,
            object thisInstance,
            object[] parameters)
        {
            try
            {
                type.GetFlattenedMethod(methodName).Invoke(thisInstance, parameters);
            }
            catch
            {
            }
        }

        public static T ReadField<TContext, T>(TContext context, string fieldName)
        {
            MemberInfo[] flattenedMember = typeof (TContext).GetFlattenedMember(fieldName);
            if (flattenedMember == null || flattenedMember.Length == 0)
                throw new ArgumentException($"{typeof (TContext).CSharpName()} does not contain a field named \"{fieldName}\"");
            if (flattenedMember.Length > 1)
                throw new ArgumentException($"{typeof (TContext).CSharpName()} has more than one field named \"{fieldName}\"");
            if (!(flattenedMember[0] is FieldInfo fieldInfo))
                throw new ArgumentException($"{typeof (TContext).CSharpName()}.{fieldName} is not a field");
            return fieldInfo.FieldType == typeof (T) ? (T) fieldInfo.GetValue((object) context) : throw new ArgumentException($"{typeof (TContext).CSharpName()}.{fieldName} type is not compatable with {typeof (T).CSharpName()}");
        }

        public static T ReadFields<TContext, T>(TContext context, params string[] fieldNames)
        {
            for (int index = 0; index < fieldNames.Length; ++index)
            {
                MemberInfo[] flattenedMember = typeof (TContext).GetFlattenedMember(fieldNames[index]);
                if (flattenedMember != null && flattenedMember.Length != 0 && flattenedMember.Length <= 1 && flattenedMember[0] is FieldInfo fieldInfo && fieldInfo.FieldType == typeof (T))
                    return (T) fieldInfo.GetValue((object) context);
            }
            throw new ArgumentException($"Unable to read any of the following fields {string.Join(", ", fieldNames)} on {(object) context}");
        }

        public static IEnumerable<TInterface> GetAssemblyInstances<TInterface>()
        {
            // ISSUE: object of a compiler-generated type is created
            return fiRuntimeReflectionUtility.GetUserDefinedEditorAssemblies().SelectMany<Assembly, System.Type, __AnonType0<Assembly, System.Type>>((Func<Assembly, IEnumerable<System.Type>>) (assembly => (IEnumerable<System.Type>) assembly.GetTypes()), (Func<Assembly, System.Type, __AnonType0<Assembly, System.Type>>) ((assembly, type) => new __AnonType0<Assembly, System.Type>(assembly, type))).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => !_param0.type.Resolve().IsGenericTypeDefinition)).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => !_param0.type.Resolve().IsAbstract)).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => !_param0.type.Resolve().IsInterface)).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => typeof (TInterface).IsAssignableFrom(_param0.type))).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => _param0.type.GetDeclaredConstructor(fsPortableReflection.EmptyTypes) != null)).Select<__AnonType0<Assembly, System.Type>, TInterface>((Func<__AnonType0<Assembly, System.Type>, TInterface>) (_param0 => (TInterface) Activator.CreateInstance(_param0.type)));
        }

        public static IEnumerable<System.Type> GetUnityObjectTypes()
        {
            // ISSUE: object of a compiler-generated type is created
            return fiRuntimeReflectionUtility.GetRuntimeAssemblies().SelectMany<Assembly, System.Type, __AnonType0<Assembly, System.Type>>((Func<Assembly, IEnumerable<System.Type>>) (assembly => (IEnumerable<System.Type>) assembly.GetTypes()), (Func<Assembly, System.Type, __AnonType0<Assembly, System.Type>>) ((assembly, type) => new __AnonType0<Assembly, System.Type>(assembly, type))).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => _param0.type.Resolve().IsVisible)).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => !_param0.type.Resolve().IsGenericTypeDefinition)).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => typeof (UnityEngine.Object).IsAssignableFrom(_param0.type))).Select<__AnonType0<Assembly, System.Type>, System.Type>((Func<__AnonType0<Assembly, System.Type>, System.Type>) (_param0 => _param0.type));
        }

        private static string GetName(Assembly assembly)
        {
            int length = assembly.FullName.IndexOf(",");
            return length >= 0 ? assembly.FullName.Substring(0, length) : assembly.FullName;
        }

        public static IEnumerable<Assembly> GetRuntimeAssemblies()
        {
            if (fiRuntimeReflectionUtility._cachedRuntimeAssemblies == null)
            {
                fiRuntimeReflectionUtility._cachedRuntimeAssemblies = ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Func<Assembly, bool>) (assembly => !fiRuntimeReflectionUtility.IsBannedAssembly(assembly))).Where<Assembly>((Func<Assembly, bool>) (assembly => !fiRuntimeReflectionUtility.IsUnityEditorAssembly(assembly))).Where<Assembly>((Func<Assembly, bool>) (assembly => !fiRuntimeReflectionUtility.GetName(assembly).Contains("-Editor"))).ToList<Assembly>();
                fiLog.Blank();
                foreach (Assembly cachedRuntimeAssembly in fiRuntimeReflectionUtility._cachedRuntimeAssemblies)
                    fiLog.Log((object) typeof (fiRuntimeReflectionUtility), "GetRuntimeAssemblies - " + fiRuntimeReflectionUtility.GetName(cachedRuntimeAssembly));
                fiLog.Blank();
            }
            return (IEnumerable<Assembly>) fiRuntimeReflectionUtility._cachedRuntimeAssemblies;
        }

        public static IEnumerable<Assembly> GetUserDefinedEditorAssemblies()
        {
            if (fiRuntimeReflectionUtility._cachedUserDefinedEditorAssemblies == null)
            {
                fiRuntimeReflectionUtility._cachedUserDefinedEditorAssemblies = ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Func<Assembly, bool>) (assembly => !fiRuntimeReflectionUtility.IsBannedAssembly(assembly))).Where<Assembly>((Func<Assembly, bool>) (assembly => !fiRuntimeReflectionUtility.IsUnityEditorAssembly(assembly))).ToList<Assembly>();
                fiLog.Blank();
                foreach (Assembly definedEditorAssembly in fiRuntimeReflectionUtility._cachedUserDefinedEditorAssemblies)
                    fiLog.Log((object) typeof (fiRuntimeReflectionUtility), "GetUserDefinedEditorAssemblies - " + fiRuntimeReflectionUtility.GetName(definedEditorAssembly));
                fiLog.Blank();
            }
            return (IEnumerable<Assembly>) fiRuntimeReflectionUtility._cachedUserDefinedEditorAssemblies;
        }

        public static IEnumerable<Assembly> GetAllEditorAssemblies()
        {
            if (fiRuntimeReflectionUtility._cachedAllEditorAssembles == null)
            {
                fiRuntimeReflectionUtility._cachedAllEditorAssembles = ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Func<Assembly, bool>) (assembly => !fiRuntimeReflectionUtility.IsBannedAssembly(assembly))).ToList<Assembly>();
                fiLog.Blank();
                foreach (Assembly allEditorAssemble in fiRuntimeReflectionUtility._cachedAllEditorAssembles)
                    fiLog.Log((object) typeof (fiRuntimeReflectionUtility), "GetAllEditorAssemblies - " + fiRuntimeReflectionUtility.GetName(allEditorAssemble));
                fiLog.Blank();
            }
            return (IEnumerable<Assembly>) fiRuntimeReflectionUtility._cachedAllEditorAssembles;
        }

        private static bool IsUnityEditorAssembly(Assembly assembly)
        {
            return ((IEnumerable<string>) new string[2]
            {
                "UnityEditor",
                "UnityEditor.UI"
            }).Contains<string>(fiRuntimeReflectionUtility.GetName(assembly));
        }

        private static bool IsBannedAssembly(Assembly assembly)
        {
            return ((IEnumerable<string>) new string[46]
            {
                "AssetStoreTools",
                "AssetStoreToolsExtra",
                "UnityScript",
                "UnityScript.Lang",
                "Boo.Lang.Parser",
                "Boo.Lang",
                "Boo.Lang.Compiler",
                "mscorlib",
                "System.ComponentModel.DataAnnotations",
                "System.Xml.Linq",
                "ICSharpCode.NRefactory",
                "Mono.Cecil",
                "Mono.Cecil.Mdb",
                "Unity.DataContract",
                "Unity.IvyParser",
                "Unity.Locator",
                "Unity.PackageManager",
                "Unity.SerializationLogic",
                "UnityEngine.UI",
                "UnityEditor.Android.Extensions",
                "UnityEditor.BB10.Extensions",
                "UnityEditor.Metro.Extensions",
                "UnityEditor.WP8.Extensions",
                "UnityEditor.iOS.Extensions",
                "UnityEditor.iOS.Extensions.Xcode",
                "UnityEditor.WindowsStandalone.Extensions",
                "UnityEditor.LinuxStandalone.Extensions",
                "UnityEditor.OSXStandalone.Extensions",
                "UnityEditor.WebGL.Extensions",
                "UnityEditor.Graphs",
                "protobuf-net",
                "Newtonsoft.Json",
                "System",
                "System.Configuration",
                "System.Xml",
                "System.Core",
                "Mono.Security",
                "I18N",
                "I18N.West",
                "nunit.core",
                "nunit.core.interfaces",
                "nunit.framework",
                "NSubstitute",
                "UnityVS.VersionSpecific",
                "SyntaxTree.VisualStudio.Unity.Bridge",
                "SyntaxTree.VisualStudio.Unity.Messaging"
            }).Contains<string>(fiRuntimeReflectionUtility.GetName(assembly));
        }

        public static IEnumerable<System.Type> AllSimpleTypesDerivingFrom(System.Type baseType)
        {
            // ISSUE: object of a compiler-generated type is created
            return fiRuntimeReflectionUtility.GetRuntimeAssemblies().SelectMany<Assembly, System.Type, __AnonType0<Assembly, System.Type>>((Func<Assembly, IEnumerable<System.Type>>) (assembly => (IEnumerable<System.Type>) assembly.GetTypes()), (Func<Assembly, System.Type, __AnonType0<Assembly, System.Type>>) ((assembly, type) => new __AnonType0<Assembly, System.Type>(assembly, type))).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param1 => baseType.IsAssignableFrom(_param1.type))).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => _param0.type.Resolve().IsClass)).Where<__AnonType0<Assembly, System.Type>>((Func<__AnonType0<Assembly, System.Type>, bool>) (_param0 => !_param0.type.Resolve().IsGenericTypeDefinition)).Select<__AnonType0<Assembly, System.Type>, System.Type>((Func<__AnonType0<Assembly, System.Type>, System.Type>) (_param0 => _param0.type));
        }

        public static IEnumerable<System.Type> AllSimpleCreatableTypesDerivingFrom(System.Type baseType)
        {
            return fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(baseType).Where<System.Type>((Func<System.Type, bool>) (type => !type.Resolve().IsAbstract)).Where<System.Type>((Func<System.Type, bool>) (type => !type.Resolve().IsGenericType)).Where<System.Type>((Func<System.Type, bool>) (type => type.GetDeclaredConstructor(fsPortableReflection.EmptyTypes) != null));
        }
    }
}
