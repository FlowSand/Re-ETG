// Decompiled with JetBrains decompiler
// Type: FullInspector.NullSerializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

#nullable disable
namespace FullInspector;

[Obsolete("Please use [fiInspectorOnly]")]
public class NullSerializer : BaseSerializer
{
  public override string Serialize(
    MemberInfo storageType,
    object value,
    ISerializationOperator serializationOperator)
  {
    return (string) null;
  }

  public override object Deserialize(
    MemberInfo storageType,
    string serializedState,
    ISerializationOperator serializationOperator)
  {
    return (object) null;
  }
}
