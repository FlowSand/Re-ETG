// Decompiled with JetBrains decompiler
// Type: FullInspector.InspectorCollectionItemAttributesAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using FullSerializer.Internal;
using System;
using System.Linq;
using System.Reflection;

#nullable disable
namespace FullInspector
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class InspectorCollectionItemAttributesAttribute : Attribute
  {
    public MemberInfo AttributeProvider;

    public InspectorCollectionItemAttributesAttribute(Type attributes)
    {
      this.AttributeProvider = typeof (fiICollectionAttributeProvider).Resolve().IsAssignableFrom(attributes.Resolve()) ? fiAttributeProvider.Create(((fiICollectionAttributeProvider) Activator.CreateInstance(attributes)).GetAttributes().ToArray<object>()) : throw new ArgumentException("Must be an instance of FullInspector.fiICollectionAttributeProvider", nameof (attributes));
    }
  }
}
