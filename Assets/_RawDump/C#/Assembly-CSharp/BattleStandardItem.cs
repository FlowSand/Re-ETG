// Decompiled with JetBrains decompiler
// Type: BattleStandardItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BattleStandardItem : PassiveItem
{
  public static float BattleStandardCharmDurationMultiplier = 2f;
  public static float BattleStandardCompanionDamageMultiplier = 2f;
  public float CharmDurationMultiplier = 2f;
  public float CompanionDamageMultiplier = 2f;
  public GameObject OverheadVFXSprite;
  private GameObject m_instanceOverhead;
  private tk2dSprite m_instanceOverheadSprite;
  private bool m_hiddenForFall;
  private bool m_isBackfacing;

  protected override void Update()
  {
    if (!((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null) || !this.m_pickedUp)
      return;
    if ((bool) (UnityEngine.Object) this.m_instanceOverheadSprite)
    {
      if (UnityEngine.Time.frameCount % 10 == 0 && (bool) (UnityEngine.Object) this.m_instanceOverhead && (UnityEngine.Object) this.m_instanceOverhead.transform.parent == (UnityEngine.Object) null)
      {
        this.DisengageEffect(this.m_owner);
        this.EngageEffect(this.m_owner);
      }
      if (this.m_owner.IsFalling)
      {
        this.m_hiddenForFall = true;
        this.m_instanceOverheadSprite.renderer.enabled = false;
      }
      else
      {
        if (this.m_hiddenForFall)
        {
          this.m_hiddenForFall = false;
          this.m_instanceOverheadSprite.renderer.enabled = true;
        }
        if (this.m_isBackfacing != this.m_owner.IsBackfacing())
        {
          this.m_isBackfacing = !this.m_isBackfacing;
          if (this.m_isBackfacing)
          {
            this.m_instanceOverheadSprite.transform.localPosition = this.m_instanceOverheadSprite.transform.localPosition.WithY(this.m_instanceOverheadSprite.transform.localPosition.y - 0.25f);
            this.m_instanceOverheadSprite.SetSprite("battle_standard_back_001");
          }
          else
          {
            this.m_instanceOverheadSprite.transform.localPosition = this.m_instanceOverheadSprite.transform.localPosition.WithY(this.m_instanceOverheadSprite.transform.localPosition.y + 0.25f);
            this.m_instanceOverheadSprite.SetSprite("battle_standard_001");
          }
        }
        if (this.m_instanceOverheadSprite.FlipX != this.m_owner.sprite.FlipX)
        {
          this.m_instanceOverheadSprite.FlipX = this.m_owner.sprite.FlipX;
          this.m_instanceOverheadSprite.transform.localPosition = this.m_instanceOverheadSprite.transform.localPosition.WithX(this.m_instanceOverheadSprite.transform.localPosition.x * -1f);
        }
        if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.END_TIMES)
          return;
        this.DisengageEffect(this.m_owner);
      }
    }
    else
    {
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
        return;
      this.EngageEffect(this.m_owner);
    }
  }

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    BattleStandardItem.BattleStandardCharmDurationMultiplier = this.CharmDurationMultiplier;
    BattleStandardItem.BattleStandardCompanionDamageMultiplier = this.CompanionDamageMultiplier;
    if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
      PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
    if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
      PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
    else
      PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
    this.EngageEffect(player);
    base.Pickup(player);
  }

  protected void EngageEffect(PlayerController user)
  {
    if (!(bool) (UnityEngine.Object) this.m_instanceOverhead)
      this.m_instanceOverhead = user.RegisterAttachedObject(this.OverheadVFXSprite, "jetpack", 0.1f);
    this.m_instanceOverheadSprite = this.m_instanceOverhead.GetComponentInChildren<tk2dSprite>();
  }

  protected void DisengageEffect(PlayerController user)
  {
    if (!(bool) (UnityEngine.Object) this.m_instanceOverhead)
      return;
    user.DeregisterAttachedObject(this.m_instanceOverhead);
    this.m_instanceOverhead = (GameObject) null;
    this.m_instanceOverheadSprite = (tk2dSprite) null;
  }

  public override DebrisObject Drop(PlayerController player)
  {
    this.DisengageEffect(player);
    DebrisObject debrisObject = base.Drop(player);
    if (PassiveItem.ActiveFlagItems.ContainsKey(player) && PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
    {
      PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
      if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
        PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
    }
    debrisObject.GetComponent<BattleStandardItem>().m_pickedUpThisRun = true;
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.m_owner)
      this.DisengageEffect(this.m_owner);
    BraveTime.ClearMultiplier(this.gameObject);
    if (this.m_pickedUp && PassiveItem.ActiveFlagItems.ContainsKey(this.m_owner) && PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
    {
      PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
      if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] == 0)
        PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
    }
    base.OnDestroy();
  }
}
