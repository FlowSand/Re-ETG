// Decompiled with JetBrains decompiler
// Type: RadialSlowItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
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
        return (IEnumerator) new RadialSlowItem.<HandleStealEffect>c__Iterator0()
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
        return (IEnumerator) new RadialSlowItem.<HandleActive>c__Iterator1()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ProcessSlow(AIActor target)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RadialSlowItem.<ProcessSlow>c__Iterator2()
        {
          target = target,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ProcessHammerSlow(ForgeHammerController target)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RadialSlowItem.<ProcessHammerSlow>c__Iterator3()
        {
          target = target,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ProcessTrapSlow(ProjectileTrapController target)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RadialSlowItem.<ProcessTrapSlow>c__Iterator4()
        {
          target = target,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ProcessShopSlow(BaseShopController target, AIAnimator shopkeep)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RadialSlowItem.<ProcessShopSlow>c__Iterator5()
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
        return (IEnumerator) new RadialSlowItem.<ProcessBehaviorSpeculatorSlow>c__Iterator6()
        {
          target = target,
          _this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
