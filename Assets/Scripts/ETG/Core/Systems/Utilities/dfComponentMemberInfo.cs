using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

#nullable disable

[Serializable]
public class dfComponentMemberInfo
    {
        public Component Component;
        public string MemberName;

        public bool IsValid
        {
            get
            {
                return (UnityEngine.Object) this.Component != (UnityEngine.Object) null && !string.IsNullOrEmpty(this.MemberName) && ((IEnumerable<MemberInfo>) this.Component.GetType().GetMember(this.MemberName)).FirstOrDefault<MemberInfo>() != null;
            }
        }

        public System.Type GetMemberType()
        {
            System.Type type = this.Component.GetType();
            MemberInfo member = ((IEnumerable<MemberInfo>) type.GetMember(this.MemberName)).FirstOrDefault<MemberInfo>();
            switch (member)
            {
                case null:
                    throw new MissingMemberException($"Member not found: {type.Name}.{this.MemberName}");
                case FieldInfo _:
                    return ((FieldInfo) member).FieldType;
                case PropertyInfo _:
                    return ((PropertyInfo) member).PropertyType;
                case MethodInfo _:
                    return ((MethodInfo) member).ReturnType;
                case EventInfo _:
                    return ((EventInfo) member).EventHandlerType;
                default:
                    throw new InvalidCastException("Invalid member type: " + (object) member.GetMemberType());
            }
        }

        public MethodInfo GetMethod()
        {
            return ((IEnumerable<MemberInfo>) this.Component.GetType().GetMember(this.MemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).FirstOrDefault<MemberInfo>() as MethodInfo;
        }

        public dfObservableProperty GetProperty()
        {
            System.Type type = this.Component.GetType();
            MemberInfo member = ((IEnumerable<MemberInfo>) this.Component.GetType().GetMember(this.MemberName)).FirstOrDefault<MemberInfo>();
            switch (member)
            {
                case null:
                    throw new MissingMemberException($"Member not found: {type.Name}.{this.MemberName}");
                case FieldInfo _:
                case PropertyInfo _:
                    return new dfObservableProperty((object) this.Component, member);
                default:
                    throw new InvalidCastException($"Member {this.MemberName} is not an observable field or property");
            }
        }

        public override string ToString()
        {
            return $"{(!((UnityEngine.Object) this.Component != (UnityEngine.Object) null) ? (object) "[Missing ComponentType]" : (object) this.Component.GetType().Name)}.{(string.IsNullOrEmpty(this.MemberName) ? (object) "[Missing MemberName]" : (object) this.MemberName)}";
        }
    }

