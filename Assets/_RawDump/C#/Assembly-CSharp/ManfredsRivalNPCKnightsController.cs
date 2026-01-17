// Decompiled with JetBrains decompiler
// Type: ManfredsRivalNPCKnightsController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ManfredsRivalNPCKnightsController : BraveBehaviour
{
  private List<AIActor> m_knights = new List<AIActor>();

  protected override void OnDestroy() => base.OnDestroy();

  public void ManfredKnightsSpawned()
  {
    List<AIActor> activeEnemies = this.talkDoer.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    for (int index = 0; index < activeEnemies.Count; ++index)
    {
      if (!((Object) activeEnemies[index] == (Object) this.aiActor))
      {
        activeEnemies[index].behaviorSpeculator.enabled = false;
        this.m_knights.Add(activeEnemies[index]);
      }
    }
  }
}
