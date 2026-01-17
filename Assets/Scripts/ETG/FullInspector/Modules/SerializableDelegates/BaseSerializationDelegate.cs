// Decompiled with JetBrains decompiler
// Type: FullInspector.Modules.SerializableDelegates.BaseSerializationDelegate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer.Internal;
using System;

#nullable disable
namespace FullInspector.Modules.SerializableDelegates
{
  public class BaseSerializationDelegate
  {
    public UnityEngine.Object MethodContainer;
    public string MethodName;

    public BaseSerializationDelegate()
      : this((UnityEngine.Object) null, string.Empty)
    {
    }

    public BaseSerializationDelegate(UnityEngine.Object methodContainer, string methodName)
    {
      this.MethodContainer = methodContainer;
      this.MethodName = methodName;
    }

    public bool CanInvoke
    {
      get
      {
        return this.MethodContainer != (UnityEngine.Object) null && !string.IsNullOrEmpty(this.MethodName) && this.MethodContainer.GetType().GetFlattenedMethod(this.MethodName) != null;
      }
    }

    protected object DoInvoke(params object[] parameters)
    {
      if (this.MethodContainer == (UnityEngine.Object) null)
        throw new InvalidOperationException("Attempt to invoke delegate without a method container");
      return ((!string.IsNullOrEmpty(this.MethodName) ? this.MethodContainer.GetType().GetFlattenedMethod(this.MethodName) : throw new InvalidOperationException("Attempt to invoke delegate without a selected method")) ?? throw new InvalidOperationException($"Unable to locate method {this.MethodName} in container {(object) this.MethodContainer}")).Invoke((object) this.MethodContainer, parameters);
    }
  }
}
