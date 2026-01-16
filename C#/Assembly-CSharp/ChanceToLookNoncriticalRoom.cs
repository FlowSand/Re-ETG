// Decompiled with JetBrains decompiler
// Type: ChanceToLookNoncriticalRoom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class ChanceToLookNoncriticalRoom : MonoBehaviour, IPlaceConfigurable
{
  public float chanceMin = 0.3f;
  public float chanceMax = 0.5f;

  public void ConfigureOnPlacement(RoomHandler room)
  {
    if (room.connectedRooms.Count != 1)
      return;
    room.ShouldAttemptProceduralLock = true;
    room.AttemptProceduralLockChance = Random.Range(this.chanceMin, this.chanceMax);
  }
}
