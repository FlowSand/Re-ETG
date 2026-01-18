using System.Collections.Generic;
using UnityEngine;

#nullable disable

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

