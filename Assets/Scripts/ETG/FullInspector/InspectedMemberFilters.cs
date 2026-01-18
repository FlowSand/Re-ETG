using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FullInspector.Internal;
using FullSerializer.Internal;
using UnityEngine;

#nullable disable
namespace FullInspector
{
    public static class InspectedMemberFilters
    {
        public static IInspectedMemberFilter All = (IInspectedMemberFilter) new InspectedMemberFilters.AllFilter();
        public static IInspectedMemberFilter FullInspectorSerializedProperties = (IInspectedMemberFilter) new InspectedMemberFilters.FullInspectorSerializedPropertiesFilter();
        public static IInspectedMemberFilter InspectableMembers = (IInspectedMemberFilter) new InspectedMemberFilters.InspectableMembersFilter();
        public static IInspectedMemberFilter StaticInspectableMembers = (IInspectedMemberFilter) new InspectedMemberFilters.StaticInspectableMembersFilter();
        public static IInspectedMemberFilter ButtonMembers = (IInspectedMemberFilter) new InspectedMemberFilters.ButtonMembersFilter();

        private static bool ShouldDisplayProperty(InspectedProperty property)
        {
            MemberInfo memberInfo = property.MemberInfo;
            if (memberInfo.IsDefined(typeof (ShowInInspectorAttribute), true))
                return true;
            if (memberInfo.IsDefined(typeof (HideInInspector), true) || memberInfo.IsDefined(typeof (NotSerializedAttribute), true) || ((IEnumerable<System.Type>) fiInstalledSerializerManager.SerializationOptOutAnnotations).Any<System.Type>((Func<System.Type, bool>) (t => memberInfo.IsDefined(t, true))))
                return false;
            if (!property.IsStatic && ((IEnumerable<System.Type>) fiInstalledSerializerManager.SerializationOptInAnnotations).Any<System.Type>((Func<System.Type, bool>) (t => memberInfo.IsDefined(t, true))))
                return true;
            if (property.MemberInfo is PropertyInfo && fiSettings.InspectorRequireShowInInspector)
                return false;
            return typeof (BaseObject).Resolve().IsAssignableFrom(property.StorageType.Resolve()) || InspectedType.IsSerializedByFullInspector(property) || InspectedType.IsSerializedByUnity(property);
        }

        private static bool IsPropertyTypeInspectable(InspectedProperty property)
        {
            if (typeof (Delegate).IsAssignableFrom(property.StorageType))
                return false;
            if (property.MemberInfo is FieldInfo)
            {
                if (property.MemberInfo.IsDefined(typeof (CompilerGeneratedAttribute), false))
                    return false;
            }
            else if (property.MemberInfo is PropertyInfo)
            {
                PropertyInfo memberInfo = (PropertyInfo) property.MemberInfo;
                if (!memberInfo.CanRead)
                    return false;
                string str = memberInfo.DeclaringType.Namespace;
                if (str != null && (str.StartsWith("UnityEngine") || str.StartsWith("UnityEditor")) && !memberInfo.CanWrite || memberInfo.Name.EndsWith("Item") && memberInfo.GetIndexParameters().Length > 0)
                    return false;
            }
            return true;
        }

        private class AllFilter : IInspectedMemberFilter
        {
            public bool IsInterested(InspectedProperty property) => true;

            public bool IsInterested(InspectedMethod method) => true;
        }

        private class FullInspectorSerializedPropertiesFilter : IInspectedMemberFilter
        {
            public bool IsInterested(InspectedProperty property)
            {
                return property.CanWrite && InspectedType.IsSerializedByFullInspector(property) && !InspectedType.IsSerializedByUnity(property);
            }

            public bool IsInterested(InspectedMethod method) => false;
        }

        private class InspectableMembersFilter : IInspectedMemberFilter
        {
            public bool IsInterested(InspectedProperty property)
            {
                return InspectedMemberFilters.IsPropertyTypeInspectable(property) && InspectedMemberFilters.ShouldDisplayProperty(property);
            }

            public bool IsInterested(InspectedMethod method)
            {
                return method.Method.IsDefined(typeof (InspectorButtonAttribute), true);
            }
        }

        private class StaticInspectableMembersFilter : IInspectedMemberFilter
        {
            public bool IsInterested(InspectedProperty property)
            {
                return property.IsStatic && InspectedMemberFilters.IsPropertyTypeInspectable(property);
            }

            public bool IsInterested(InspectedMethod method)
            {
                return method.Method.IsDefined(typeof (InspectorButtonAttribute), true);
            }
        }

        private class ButtonMembersFilter : IInspectedMemberFilter
        {
            public bool IsInterested(InspectedProperty property) => false;

            public bool IsInterested(InspectedMethod method)
            {
                return method.Method.IsDefined(typeof (InspectorButtonAttribute), true);
            }
        }
    }
}
