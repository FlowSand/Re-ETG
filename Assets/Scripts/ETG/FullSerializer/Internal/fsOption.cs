// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsOption
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace FullSerializer.Internal
{
  public static class fsOption
  {
    public static fsOption<T> Just<T>(T value) => new fsOption<T>(value);
  }
}
