// Decompiled with JetBrains decompiler
// Type: ActiveBasicStatItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ActiveBasicStatItem : PlayerItem
  {
    public float duration = 5f;
    [BetterList]
    public List<StatModifier> modifiers;
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
      return (IEnumerator) new ActiveBasicStatItem__HandleDurationc__Iterator0()
      {
        user = user,
        _this = this
      };
    }

    protected override void OnPreDrop(PlayerController user)
    {
      if (this.IsCurrentlyActive && (bool) (Object) this.m_cachedUser)
        this.m_cachedUser.stats.RecalculateStats(this.m_cachedUser);
      this.m_cachedUser = (PlayerController) null;
      base.OnPreDrop(user);
    }

    public override void OnItemSwitched(PlayerController user)
    {
      base.OnItemSwitched(user);
      this.IsCurrentlyActive = false;
      if (!(bool) (Object) this.m_cachedUser)
        return;
      this.m_cachedUser.stats.RecalculateStats(this.m_cachedUser);
    }

    protected override void OnDestroy()
    {
      this.IsCurrentlyActive = false;
      if ((bool) (Object) this.m_cachedUser)
        this.m_cachedUser.stats.RecalculateStats(this.m_cachedUser);
      this.m_cachedUser = (PlayerController) null;
      base.OnDestroy();
    }
  }

