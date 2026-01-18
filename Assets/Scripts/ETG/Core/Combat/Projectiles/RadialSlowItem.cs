using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class RadialSlowItem : AffectEnemiesInRadiusItem
  {
    public float InTime;
    public float HoldTime = 5f;
    public float OutTime = 3f;
    public float MaxTimeModifier = 0.25f;
    public bool AllowStealing;

    protected override void DoEffect(PlayerController user)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_time_bell_01", this.gameObject);
      base.DoEffect(user);
      if (!this.AllowStealing)
        return;
      user.StartCoroutine(this.HandleStealEffect(user));
    }

    [DebuggerHidden]
    private IEnumerator HandleStealEffect(PlayerController user)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RadialSlowItem__HandleStealEffectc__Iterator0()
      {
        user = user,
        _this = this
      };
    }

    protected override void AffectEnemy(AIActor target)
    {
      if (!this.IsCurrentlyActive)
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleActive());
      target.StartCoroutine(this.ProcessSlow(target));
    }

    protected override void AffectForgeHammer(ForgeHammerController target)
    {
      if (!this.IsCurrentlyActive)
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleActive());
      target.StartCoroutine(this.ProcessHammerSlow(target));
    }

    protected override void AffectProjectileTrap(ProjectileTrapController target)
    {
      if (!this.IsCurrentlyActive)
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleActive());
      target.StartCoroutine(this.ProcessTrapSlow(target));
    }

    protected override void AffectShop(BaseShopController target)
    {
      if (!this.AllowStealing || !(bool) (Object) target || !(bool) (Object) target.shopkeepFSM)
        return;
      AIAnimator component = target.shopkeepFSM.GetComponent<AIAnimator>();
      if (!this.IsCurrentlyActive)
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleActive());
      target.StartCoroutine(this.ProcessShopSlow(target, component));
    }

    protected override void AffectMajorBreakable(MajorBreakable target)
    {
      if (!(bool) (Object) target.behaviorSpeculator)
        return;
      target.StartCoroutine(this.ProcessBehaviorSpeculatorSlow(target.behaviorSpeculator));
    }

    [DebuggerHidden]
    private IEnumerator HandleActive()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RadialSlowItem__HandleActivec__Iterator1()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator ProcessSlow(AIActor target)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RadialSlowItem__ProcessSlowc__Iterator2()
      {
        target = target,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator ProcessHammerSlow(ForgeHammerController target)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RadialSlowItem__ProcessHammerSlowc__Iterator3()
      {
        target = target,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator ProcessTrapSlow(ProjectileTrapController target)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RadialSlowItem__ProcessTrapSlowc__Iterator4()
      {
        target = target,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator ProcessShopSlow(BaseShopController target, AIAnimator shopkeep)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RadialSlowItem__ProcessShopSlowc__Iterator5()
      {
        target = target,
        shopkeep = shopkeep,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator ProcessBehaviorSpeculatorSlow(BehaviorSpeculator target)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RadialSlowItem__ProcessBehaviorSpeculatorSlowc__Iterator6()
      {
        target = target,
        _this = this
      };
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

