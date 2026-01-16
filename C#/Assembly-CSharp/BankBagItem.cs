// Decompiled with JetBrains decompiler
// Type: BankBagItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BankBagItem : PassiveItem, IPaydayItem
{
  public static float cachedCoinLifespan = 6f;
  public float CoinLifespan = 6f;
  public float MinPercentToDrop = 0.5f;
  public float MaxPercentToDrop = 1f;
  public int MaxCoinsToDrop = -1;
  public GameObject DropVFX;
  public GameObject AttachmentObject;
  private GameObject instanceAttachment;
  private tk2dSprite instanceAttachmentSprite;
  [NonSerialized]
  public bool HasSetOrder;
  [NonSerialized]
  public string ID01;
  [NonSerialized]
  public string ID02;
  [NonSerialized]
  public string ID03;

  public void StoreData(string id1, string id2, string id3)
  {
    this.ID01 = id1;
    this.ID02 = id2;
    this.ID03 = id3;
    this.HasSetOrder = true;
  }

  public bool HasCachedData() => this.HasSetOrder;

  public string GetID(int placement)
  {
    if (placement == 0)
      return this.ID01;
    return placement == 1 ? this.ID02 : this.ID03;
  }

  public override void MidGameSerialize(List<object> data)
  {
    base.MidGameSerialize(data);
    data.Add((object) this.HasSetOrder);
    data.Add((object) this.ID01);
    data.Add((object) this.ID02);
    data.Add((object) this.ID03);
  }

  public override void MidGameDeserialize(List<object> data)
  {
    base.MidGameDeserialize(data);
    if (data.Count != 4)
      return;
    this.HasSetOrder = (bool) data[0];
    this.ID01 = (string) data[1];
    this.ID02 = (string) data[2];
    this.ID03 = (string) data[3];
  }

  public void Awake() => BankBagItem.cachedCoinLifespan = this.CoinLifespan;

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    base.Pickup(player);
    if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
      PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
    player.OnReceivedDamage += new Action<PlayerController>(this.HandlePlayerDamaged);
    this.instanceAttachment = player.RegisterAttachedObject(this.AttachmentObject, "center");
    this.instanceAttachment.transform.parent = player.sprite.transform;
    this.instanceAttachmentSprite = this.instanceAttachment.GetComponentInChildren<tk2dSprite>();
    if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
      PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
    else
      PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
  }

  private void HandlePlayerDamaged(PlayerController p)
  {
    if (p.carriedConsumables.Currency <= 0)
      return;
    int a = UnityEngine.Random.Range(Mathf.FloorToInt((float) p.carriedConsumables.Currency * this.MinPercentToDrop), Mathf.CeilToInt((float) p.carriedConsumables.Currency * this.MaxPercentToDrop) + 1);
    if (this.MaxCoinsToDrop > 0)
      a = Mathf.Clamp(a, 0, this.MaxCoinsToDrop);
    int amountToDrop = Mathf.Min(a, p.carriedConsumables.Currency);
    if ((bool) (UnityEngine.Object) this.DropVFX)
      p.PlayEffectOnActor(this.DropVFX, Vector3.zero, false, true);
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_coin_spill_01", this.gameObject);
    p.carriedConsumables.Currency -= amountToDrop;
    LootEngine.SpawnCurrencyManual(p.CenterPosition, amountToDrop);
  }

  private void LateUpdate()
  {
    if (!(bool) (UnityEngine.Object) this.instanceAttachment || !this.m_pickedUp || !(bool) (UnityEngine.Object) this.m_owner)
      return;
    this.instanceAttachment.transform.position = (Vector3) (this.m_owner.sprite.WorldCenter + new Vector2(0.0f, -0.125f));
    this.instanceAttachmentSprite.FlipX = this.m_owner.sprite.FlipX;
    this.instanceAttachmentSprite.transform.localPosition = new Vector3(!this.instanceAttachmentSprite.FlipX ? -0.5f : 0.5f, -0.5f, 0.0f);
    this.instanceAttachmentSprite.renderer.enabled = this.m_owner.IsVisible && this.m_owner.sprite.renderer.enabled && !this.m_owner.IsFalling && !this.m_owner.IsDodgeRolling;
    this.instanceAttachmentSprite.UpdateZDepth();
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    if ((bool) (UnityEngine.Object) player)
    {
      player.OnReceivedDamage -= new Action<PlayerController>(this.HandlePlayerDamaged);
      player.DeregisterAttachedObject(this.instanceAttachment);
      this.instanceAttachment = (GameObject) null;
      this.instanceAttachmentSprite = (tk2dSprite) null;
    }
    if (PassiveItem.ActiveFlagItems.ContainsKey(player) && PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
    {
      PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
      if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
        PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
    }
    debrisObject.GetComponent<BankBagItem>().m_pickedUpThisRun = true;
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (!this.m_pickedUp || !(bool) (UnityEngine.Object) this.m_owner)
      return;
    this.m_owner.OnReceivedDamage -= new Action<PlayerController>(this.HandlePlayerDamaged);
    this.m_owner.DeregisterAttachedObject(this.instanceAttachment);
    this.instanceAttachment = (GameObject) null;
    this.instanceAttachmentSprite = (tk2dSprite) null;
    if (!PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
      return;
    PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
    if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] != 0)
      return;
    PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
  }
}
