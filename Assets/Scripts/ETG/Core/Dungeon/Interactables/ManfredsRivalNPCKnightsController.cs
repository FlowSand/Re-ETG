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

