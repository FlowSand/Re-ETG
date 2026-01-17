// Decompiled with JetBrains decompiler
// Type: FullInspector.NotSupportedSerializationOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector
{
  public class NotSupportedSerializationOperator : ISerializationOperator
  {
    public UnityEngine.Object RetrieveObjectReference(int storageId)
    {
      throw new NotSupportedException("UnityEngine.Object references are not supported with this serialization operator");
    }

    public int StoreObjectReference(UnityEngine.Object obj)
    {
      throw new NotSupportedException($"UnityEngine.Object references are not supported with this serialization operator (obj={(object) obj})");
    }
  }
}
