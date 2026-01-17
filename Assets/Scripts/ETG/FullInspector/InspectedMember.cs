// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectedMember
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

#nullable disable
namespace FullInspector
{
  public struct InspectedMember
  {
    private InspectedProperty _property;
    private InspectedMethod _method;

    public InspectedMember(InspectedProperty property)
    {
      this._property = property;
      this._method = (InspectedMethod) null;
    }

    public InspectedMember(InspectedMethod method)
    {
      this._property = (InspectedProperty) null;
      this._method = method;
    }

    public InspectedProperty Property
    {
      get
      {
        if (!this.IsProperty)
          throw new InvalidOperationException("Member is not a property");
        return this._property;
      }
    }

    public InspectedMethod Method
    {
      get
      {
        if (!this.IsMethod)
          throw new InvalidOperationException("Member is not a method");
        return this._method;
      }
    }

    public bool IsMethod => this._method != null;

    public bool IsProperty => this._property != null;

    public string Name => this.MemberInfo.Name;

    public MemberInfo MemberInfo
    {
      get => this.IsMethod ? (MemberInfo) this._method.Method : this._property.MemberInfo;
    }
  }
}
