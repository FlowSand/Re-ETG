// Decompiled with JetBrains decompiler
// Type: JetpackItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class JetpackItem : PlayerItem
  {
    public GameObject prefabToAttachToPlayer;
    private GameObject instanceJetpack;
    private tk2dSprite instanceJetpackSprite;

    protected override void DoEffect(PlayerController user)
    {
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
        return;
      this.PreventCooldownBar = true;
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_jetpack_start_01", this.gameObject);
      this.IsCurrentlyActive = true;
      user.SetIsFlying(true, "jetpack");
      this.instanceJetpack = user.RegisterAttachedObject(this.prefabToAttachToPlayer, "jetpack");
      this.instanceJetpackSprite = this.instanceJetpack.GetComponent<tk2dSprite>();
    }

    protected override void DoActiveEffect(PlayerController user)
    {
      int num = (int) AkSoundEngine.PostEvent("Stop_OBJ_jetpack_loop_01", this.gameObject);
      this.IsCurrentlyActive = false;
      user.SetIsFlying(false, "jetpack");
      user.DeregisterAttachedObject(this.instanceJetpack);
      this.instanceJetpackSprite = (tk2dSprite) null;
      user.stats.RecalculateStats(user);
    }

    public override void Update()
    {
      base.Update();
      if (!this.IsCurrentlyActive)
        return;
      DeadlyDeadlyGoopManager.IgniteGoopsCircle(this.instanceJetpackSprite.WorldBottomCenter, 0.5f);
      if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.END_TIMES)
        return;
      this.DoActiveEffect(this.LastOwner);
    }

    protected override void OnPreDrop(PlayerController user)
    {
      if (!this.IsCurrentlyActive)
        return;
      this.DoActiveEffect(user);
    }

    public override void OnItemSwitched(PlayerController user)
    {
      if (!this.IsCurrentlyActive)
        return;
      this.DoActiveEffect(user);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

