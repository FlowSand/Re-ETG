// Decompiled with JetBrains decompiler
// Type: MineCartSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class MineCartSwitch : DungeonPlaceableBehaviour
    {
      [DwarfConfigurable]
      public float PrimaryPathIndex;
      [DwarfConfigurable]
      public float TogglePathIndex = 1f;

      private void Start()
      {
        this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
      }

      private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        if (!((Object) rigidbodyCollision.OtherRigidbody.projectile != (Object) null))
          return;
        List<PathMover> componentsInRoom = this.GetAbsoluteParentRoom().GetComponentsInRoom<PathMover>();
        for (int index = 0; index < componentsInRoom.Count; ++index)
          componentsInRoom[index].IsUsingAlternateTargets = !componentsInRoom[index].IsUsingAlternateTargets;
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
