// Decompiled with JetBrains decompiler
// Type: EnemyFactorySpawnPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class EnemyFactorySpawnPoint : DungeonPlaceableBehaviour
    {
      public tk2dSpriteAnimator animator;
      public string spawnAnimationOpen = string.Empty;
      public string spawnAnimationClose = string.Empty;
      public float preSpawnDelay = 1f;
      public float postSpawnDelay = 0.5f;
      public GameObject spawnVFX;

      public void OnSpawn(AIActor actorToSpawn, IntVector2 spawnPosition, RoomHandler room)
      {
        this.StartCoroutine(this.HandleSpawnAnimations(actorToSpawn, spawnPosition, room));
      }

      [DebuggerHidden]
      private IEnumerator HandleSpawnAnimations(
        AIActor actorToSpawn,
        IntVector2 spawnPosition,
        RoomHandler room)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EnemyFactorySpawnPoint.<HandleSpawnAnimations>c__Iterator0()
        {
          spawnPosition = spawnPosition,
          actorToSpawn = actorToSpawn,
          room = room,
          _this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
