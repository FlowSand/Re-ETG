// Decompiled with JetBrains decompiler
// Type: BossTriggerZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (SpeculativeRigidbody))]
public class BossTriggerZone : BraveBehaviour
{
  public bool HasTriggered { get; set; }

  public RoomHandler ParentRoom { get; set; }

  public void Start()
  {
    this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision);
    this.ParentRoom = GameManager.Instance.Dungeon.GetRoomFromPosition(this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
    if (this.ParentRoom == null)
      return;
    if (this.ParentRoom.bossTriggerZones == null)
      this.ParentRoom.bossTriggerZones = new List<BossTriggerZone>();
    this.ParentRoom.bossTriggerZones.Add(this);
  }

  protected override void OnDestroy() => base.OnDestroy();

  private void OnTriggerCollision(
    SpeculativeRigidbody otherRigidbody,
    SpeculativeRigidbody myRigidbody,
    CollisionData collisionData)
  {
    if (this.HasTriggered || collisionData.OtherPixelCollider.CollisionLayer != CollisionLayer.PlayerCollider && collisionData.OtherPixelCollider.CollisionLayer != CollisionLayer.PlayerHitBox)
      return;
    PlayerController component1 = otherRigidbody.GetComponent<PlayerController>();
    if (!(bool) (Object) component1)
      return;
    List<HealthHaver> allHealthHavers = StaticReferenceManager.AllHealthHavers;
    for (int index = 0; index < allHealthHavers.Count; ++index)
    {
      if (allHealthHavers[index].IsBoss)
      {
        GenericIntroDoer component2 = allHealthHavers[index].GetComponent<GenericIntroDoer>();
        if ((bool) (Object) component2 && component2.triggerType == GenericIntroDoer.TriggerType.BossTriggerZone)
        {
          component2.GetComponent<ObjectVisibilityManager>().ChangeToVisibility(RoomHandler.VisibilityStatus.CURRENT);
          component2.TriggerSequence(component1);
          this.HasTriggered = true;
          break;
        }
      }
    }
  }
}
