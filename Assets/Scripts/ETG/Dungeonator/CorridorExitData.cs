// Decompiled with JetBrains decompiler
// Type: Dungeonator.CorridorExitData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace Dungeonator
{
  public class CorridorExitData
  {
    public List<CellData> cells;
    public RoomHandler linkedRoom;

    public CorridorExitData(List<CellData> c, RoomHandler rh)
    {
      this.cells = c;
      this.linkedRoom = rh;
    }
  }
}
