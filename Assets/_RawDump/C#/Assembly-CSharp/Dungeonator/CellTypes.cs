// Decompiled with JetBrains decompiler
// Type: Dungeonator.CellTypes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Dungeonator;

[Flags]
public enum CellTypes
{
  WALL = 1,
  FLOOR = 2,
  PIT = 4,
}
