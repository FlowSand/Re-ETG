// Decompiled with JetBrains decompiler
// Type: FragileGunItemPiece
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class FragileGunItemPiece : PickupObject
{
  [NonSerialized]
  public int AssignedGunId = -1;
  private bool m_pickedUp;

  public void Start()
  {
    this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.TriggerWasEntered);
    this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTrigger);
    this.IgnoredByRat = true;
    SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
  }

  public void AssignGun(Gun sourceGun)
  {
    this.AssignedGunId = sourceGun.PickupObjectId;
    if (!(bool) (UnityEngine.Object) sourceGun.sprite)
      return;
    this.sprite.SetSprite(sourceGun.sprite.Collection, sourceGun.sprite.spriteId);
  }

  private void TriggerWasEntered(
    SpeculativeRigidbody otherRigidbody,
    SpeculativeRigidbody selfRigidbody,
    CollisionData collisionData)
  {
    if (this.m_pickedUp)
      return;
    if ((UnityEngine.Object) otherRigidbody.GetComponent<PlayerController>() != (UnityEngine.Object) null)
    {
      this.PrePickupLogic(otherRigidbody, selfRigidbody);
    }
    else
    {
      if (!((UnityEngine.Object) otherRigidbody.GetComponent<PickupObject>() != (UnityEngine.Object) null) || !(bool) (UnityEngine.Object) this.debris)
        return;
      this.debris.ApplyVelocity((selfRigidbody.UnitCenter - otherRigidbody.UnitCenter).normalized);
      selfRigidbody.RegisterGhostCollisionException(otherRigidbody);
    }
  }

  public void OnTrigger(
    SpeculativeRigidbody otherRigidbody,
    SpeculativeRigidbody selfRigidbody,
    CollisionData collisionData)
  {
    if (this.m_pickedUp || !((UnityEngine.Object) otherRigidbody.GetComponent<PlayerController>() != (UnityEngine.Object) null))
      return;
    this.PrePickupLogic(otherRigidbody, selfRigidbody);
  }

  private void PrePickupLogic(
    SpeculativeRigidbody otherRigidbody,
    SpeculativeRigidbody selfRigidbody)
  {
    PlayerController component = otherRigidbody.GetComponent<PlayerController>();
    if (component.IsGhost || !this.CheckPlayerForItem(component))
      return;
    this.Pickup(component);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private bool CheckPlayerForItem(PlayerController player)
  {
    if ((bool) (UnityEngine.Object) player)
    {
      for (int index = 0; index < player.passiveItems.Count; ++index)
      {
        if (player.passiveItems[index] is FragileGunItem)
          return true;
      }
    }
    return false;
  }

  public override void Pickup(PlayerController player)
  {
    if (player.IsGhost)
      return;
    this.m_pickedUp = true;
    FragileGunItem fragileGunItem = (FragileGunItem) null;
    for (int index = 0; index < player.passiveItems.Count; ++index)
    {
      if (player.passiveItems[index] is FragileGunItem)
      {
        fragileGunItem = player.passiveItems[index] as FragileGunItem;
        break;
      }
    }
    if (!(bool) (UnityEngine.Object) fragileGunItem)
      return;
    fragileGunItem.AcquirePiece(this);
  }
}
