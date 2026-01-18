// Decompiled with JetBrains decompiler
// Type: dfReflectionExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable disable

  public static class dfReflectionExtensions
  {
    public static System.Type[] EmptyTypes = new System.Type[0];

    public static MemberTypes GetMemberType(this MemberInfo member) => member.MemberType;

    public static System.Type GetBaseType(this System.Type type) => type.BaseType;

    public static Assembly GetAssembly(this System.Type type) => type.Assembly;

[HideInInspector]
    internal static bool SignalHierarchy(
      this GameObject target,
      string messageName,
      params object[] args)
    {
      for (; (UnityEngine.Object) target != (UnityEngine.Object) null; target = target.transform.parent.gameObject)
      {
        if (target.Signal(messageName, args))
          return true;
        if ((UnityEngine.Object) target.transform.parent == (UnityEngine.Object) null)
          break;
      }
      return false;
    }

[HideInInspector]
    internal static bool Signal(this GameObject target, string messageName, params object[] args)
    {
      Component[] components = target.GetComponents(typeof (MonoBehaviour));
      System.Type[] paramTypes = new System.Type[args.Length];
      for (int index = 0; index < paramTypes.Length; ++index)
        paramTypes[index] = args[index] != null ? args[index].GetType() : typeof (object);
      bool flag = false;
      for (int index = 0; index < components.Length; ++index)
      {
        Component component = components[index];
        if (!((UnityEngine.Object) component == (UnityEngine.Object) null) && component.GetType() != null && (!(component is MonoBehaviour) || ((Behaviour) component).enabled))
        {
          MethodInfo method1 = dfReflectionExtensions.getMethod(component.GetType(), messageName, paramTypes);
          if (method1 != null)
          {
            if (method1.Invoke((object) component, args) is IEnumerator routine)
              ((MonoBehaviour) component).StartCoroutine(routine);
            flag = true;
          }
          else if (args.Length != 0)
          {
            MethodInfo method2 = dfReflectionExtensions.getMethod(component.GetType(), messageName, dfReflectionExtensions.EmptyTypes);
            if (method2 != null)
            {
              if (method2.Invoke((object) component, (object[]) null) is IEnumerator routine)
                ((MonoBehaviour) component).StartCoroutine(routine);
              flag = true;
            }
          }
        }
      }
      return flag;
    }

    private static MethodInfo getMethod(System.Type type, string name, System.Type[] paramTypes)
    {
      return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, paramTypes, (ParameterModifier[]) null);
    }

    private static bool matchesParameterTypes(MethodInfo method, System.Type[] types)
    {
      ParameterInfo[] parameters = method.GetParameters();
      if (parameters.Length != types.Length)
        return false;
      for (int index = 0; index < types.Length; ++index)
      {
        if (!parameters[index].ParameterType.IsAssignableFrom(types[index]))
          return false;
      }
      return true;
    }

    internal static FieldInfo[] GetAllFields(this System.Type type)
    {
      if (type == null)
        return new FieldInfo[0];
      BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      return ((IEnumerable<FieldInfo>) type.GetFields(bindingAttr)).Concat<FieldInfo>((IEnumerable<FieldInfo>) type.GetBaseType().GetAllFields()).Where<FieldInfo>((Func<FieldInfo, bool>) (f => !f.IsDefined(typeof (HideInInspector), true))).ToArray<FieldInfo>();
    }
  }

