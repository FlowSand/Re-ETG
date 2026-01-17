// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectedMethod
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using FullSerializer.Internal;
using System;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace FullInspector
{
  public class InspectedMethod
  {
    public InspectedMethod(MethodInfo method)
    {
      this.Method = method;
      foreach (ParameterInfo parameter in method.GetParameters())
      {
        if (!parameter.IsOptional)
        {
          this.HasArguments = true;
          break;
        }
      }
      this.DisplayLabel = new GUIContent();
      InspectorNameAttribute attribute1 = fsPortableReflection.GetAttribute<InspectorNameAttribute>((MemberInfo) method);
      if (attribute1 != null)
        this.DisplayLabel.text = attribute1.DisplayName;
      if (string.IsNullOrEmpty(this.DisplayLabel.text))
        this.DisplayLabel.text = fiDisplayNameMapper.Map(method.Name);
      InspectorTooltipAttribute attribute2 = fsPortableReflection.GetAttribute<InspectorTooltipAttribute>((MemberInfo) method);
      if (attribute2 == null)
        return;
      this.DisplayLabel.tooltip = attribute2.Tooltip;
    }

    public MethodInfo Method { get; private set; }

    public GUIContent DisplayLabel { get; private set; }

    public bool HasArguments { get; private set; }

    public void Invoke(object instance)
    {
      try
      {
        object[] parameters1 = (object[]) null;
        ParameterInfo[] parameters2 = this.Method.GetParameters();
        if (parameters2.Length != 0)
        {
          parameters1 = new object[parameters2.Length];
          for (int index = 0; index < parameters1.Length; ++index)
            parameters1[index] = parameters2[index].DefaultValue;
        }
        this.Method.Invoke(instance, parameters1);
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }
  }
}
