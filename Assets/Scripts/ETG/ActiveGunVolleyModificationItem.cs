// Decompiled with JetBrains decompiler
// Type: ActiveGunVolleyModificationItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ActiveGunVolleyModificationItem : PlayerItem
{
  public float duration = 5f;
  [Header("Gun Mod Settings")]
  public bool AddsModule;
  [ShowInInspectorIf("AddsModule", false)]
  public ProjectileModule ModuleToAdd;
  public int DuplicatesOfBaseModule;
  public float DuplicateAngleOffset = 10f;
  [LongNumericEnum]
  public CustomSynergyType[] SynergiesToIncrementDuplicates;
  private PlayerController m_cachedUser;

  protected override void DoEffect(PlayerController user)
  {
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_power_up_01", this.gameObject);
    this.m_cachedUser = user;
    this.StartCoroutine(this.HandleDuration(user));
  }

  [DebuggerHidden]
  private IEnumerator HandleDuration(PlayerController user)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ActiveGunVolleyModificationItem.\u003CHandleDuration\u003Ec__Iterator0()
    {
      user = user,
      \u0024this = this
    };
  }

  public void ModifyVolley(ProjectileVolleyData volleyToModify)
  {
    if (this.AddsModule)
    {
      this.ModuleToAdd.isExternalAddedModule = true;
      volleyToModify.projectiles.Add(this.ModuleToAdd);
    }
    int duplicatesOfBaseModule = this.DuplicatesOfBaseModule;
    for (int index = 0; index < this.SynergiesToIncrementDuplicates.Length; ++index)
    {
      if ((bool) (Object) this.LastOwner && this.LastOwner.HasActiveBonusSynergy(this.SynergiesToIncrementDuplicates[index]))
        ++duplicatesOfBaseModule;
    }
    if (duplicatesOfBaseModule <= 0)
      return;
    int count = volleyToModify.projectiles.Count;
    for (int index1 = 0; index1 < count; ++index1)
    {
      ProjectileModule projectile = volleyToModify.projectiles[index1];
      float num1 = (float) ((double) duplicatesOfBaseModule * (double) this.DuplicateAngleOffset * -1.0 / 2.0);
      for (int index2 = 0; index2 < duplicatesOfBaseModule; ++index2)
      {
        int sourceIndex = index1;
        if (projectile.CloneSourceIndex >= 0)
          sourceIndex = projectile.CloneSourceIndex;
        ProjectileModule clone = ProjectileModule.CreateClone(projectile, false, sourceIndex);
        float num2 = num1 + this.DuplicateAngleOffset * (float) index2;
        clone.angleFromAim = num2;
        clone.ignoredForReloadPurposes = true;
        clone.ammoCost = 0;
        volleyToModify.projectiles.Add(clone);
      }
    }
  }

  public override void OnItemSwitched(PlayerController user)
  {
    base.OnItemSwitched(user);
    this.IsCurrentlyActive = false;
    if (!((Object) this.m_cachedUser != (Object) null))
      return;
    this.m_cachedUser.stats.RecalculateStats(this.m_cachedUser);
  }

  protected override void OnDestroy()
  {
    this.IsCurrentlyActive = false;
    if ((bool) (Object) this.m_cachedUser)
      this.m_cachedUser.stats.RecalculateStats(this.m_cachedUser);
    base.OnDestroy();
  }
}
