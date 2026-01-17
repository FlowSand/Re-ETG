// Decompiled with JetBrains decompiler
// Type: SpikedArmorItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class SpikedArmorItem : BasicStatPickup
{
  public bool HasIgniteSynergy;
  [LongNumericEnum]
  public CustomSynergyType RequiredSynergy;
  public GameActorFireEffect IgniteEffect;

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
      PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
    if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
      PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
    else
      PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
    if (this.HasIgniteSynergy)
      player.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
    base.Pickup(player);
  }

  private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
  {
    if (!this.HasIgniteSynergy || !(bool) (UnityEngine.Object) this.m_owner || !this.m_owner.HasActiveBonusSynergy(this.RequiredSynergy) || !(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody || !(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.aiActor)
      return;
    AIActor aiActor = rigidbodyCollision.OtherRigidbody.aiActor;
    if (!aiActor.IsNormalEnemy || aiActor.IsHarmlessEnemy)
      return;
    aiActor.ApplyEffect((GameActorEffect) this.IgniteEffect);
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    if (PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
    {
      PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
      if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
        PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
    }
    if ((bool) (UnityEngine.Object) player && (bool) (UnityEngine.Object) player.specRigidbody)
      player.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
    debrisObject.GetComponent<SpikedArmorItem>().m_pickedUpThisRun = true;
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    BraveTime.ClearMultiplier(this.gameObject);
    if (this.m_pickedUp && PassiveItem.ActiveFlagItems.ContainsKey(this.m_owner) && PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
    {
      PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
      if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] == 0)
        PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
    }
    if ((bool) (UnityEngine.Object) this.m_owner && (bool) (UnityEngine.Object) this.m_owner.specRigidbody)
      this.m_owner.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
    base.OnDestroy();
  }
}
