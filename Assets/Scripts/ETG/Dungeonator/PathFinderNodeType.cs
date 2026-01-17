// Decompiled with JetBrains decompiler
// Type: Dungeonator.PathFinderNodeType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace Dungeonator
{
  public enum PathFinderNodeType
  {
    Start = 1,
    End = 2,
    Open = 4,
    Close = 8,
    Current = 16, // 0x00000010
    Path = 32, // 0x00000020
  }
}
