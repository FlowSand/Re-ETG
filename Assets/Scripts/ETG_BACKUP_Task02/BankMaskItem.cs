// Decompiled with JetBrains decompiler
// Type: BankMaskItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BankMaskItem : PassiveItem, IPaydayItem
{
  public tk2dSpriteAnimation OverrideAnimLib;
  public tk2dSprite OverrideHandSprite;
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

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    base.Pickup(player);
    player.OverrideAnimationLibrary = this.OverrideAnimLib;
    player.OverridePlayerSwitchState = PlayableCharacters.Pilot.ToString();
    player.SetOverrideShader(ShaderCache.Acquire(player.LocalShaderName));
    if (player.characterIdentity == PlayableCharacters.Eevee)
      player.GetComponent<CharacterAnimationRandomizer>().AddOverrideAnimLibrary(this.OverrideAnimLib);
    player.ChangeHandsToCustomType(this.OverrideHandSprite.Collection, this.OverrideHandSprite.spriteId);
    if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
      PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
    if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
      PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
    else
      PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    player.OverrideAnimationLibrary = (tk2dSpriteAnimation) null;
    player.OverridePlayerSwitchState = (string) null;
    player.ClearOverrideShader();
    if (player.characterIdentity == PlayableCharacters.Eevee)
      player.GetComponent<CharacterAnimationRandomizer>().RemoveOverrideAnimLibrary(this.OverrideAnimLib);
    player.RevertHandsToBaseType();
    if (PassiveItem.ActiveFlagItems.ContainsKey(player) && PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
    {
      PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
      if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
        PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
    }
    debrisObject.GetComponent<BankMaskItem>().m_pickedUpThisRun = true;
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (!this.m_pickedUp || !(bool) (UnityEngine.Object) this.m_owner)
      return;
    this.m_owner.RevertHandsToBaseType();
    this.m_owner.OverrideAnimationLibrary = (tk2dSpriteAnimation) null;
    this.m_owner.OverridePlayerSwitchState = (string) null;
    this.m_owner.ClearOverrideShader();
    if (this.m_owner.characterIdentity == PlayableCharacters.Eevee)
      this.m_owner.GetComponent<CharacterAnimationRandomizer>().RemoveOverrideAnimLibrary(this.OverrideAnimLib);
    if (!PassiveItem.ActiveFlagItems.ContainsKey(this.m_owner) || !PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
      return;
    PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
    if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] != 0)
      return;
    PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
  }
}
