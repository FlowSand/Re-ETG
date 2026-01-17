// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiLoadedSerializers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Serializers.FullSerializer;
using System;

#nullable disable
namespace FullInspector.Internal;

public class fiLoadedSerializers : fiILoadedSerializers
{
  public Type DefaultSerializerProvider => typeof (FullSerializerMetadata);

  public Type[] AllLoadedSerializerProviders
  {
    get => new Type[1]{ typeof (FullSerializerMetadata) };
  }
}
