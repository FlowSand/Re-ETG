using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BossFinalGuideDeathController : BraveBehaviour
  {
    public void Start()
    {
      this.healthHaver.ManualDeathHandling = true;
      this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
      this.healthHaver.OverrideKillCamTime = new float?(5f);
    }

    protected override void OnDestroy() => base.OnDestroy();

    private void OnBossDeath(Vector2 dir)
    {
      this.aiAnimator.ChildAnimator.gameObject.SetActive(false);
      this.aiAnimator.PlayUntilCancelled("death", true);
      this.StartCoroutine(this.HandlePostDeathExplosionCR());
      this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnBossDeath);
      GameObject gameObject = GameObject.Find("BossFinalGuide_DrWolf(Clone)");
      if (!(bool) (UnityEngine.Object) gameObject)
        return;
      HealthHaver component = gameObject.GetComponent<HealthHaver>();
      component.healthIsNumberOfHits = false;
      component.ApplyDamage(10000f, Vector2.zero, "Boss Death", damageCategory: DamageCategory.Unstoppable, ignoreInvulnerabilityFrames: true);
    }

    [DebuggerHidden]
    private IEnumerator HandlePostDeathExplosionCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalGuideDeathController__HandlePostDeathExplosionCRc__Iterator0()
      {
        _this = this
      };
    }
  }

