using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ResourcefulRatDeathController : BraveBehaviour
  {
    public void Start()
    {
      this.healthHaver.ManualDeathHandling = true;
      this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
      this.healthHaver.OverrideKillCamTime = new float?(5f);
      this.healthHaver.TrackDuringDeath = true;
    }

    protected override void OnDestroy() => base.OnDestroy();

    private void OnBossDeath(Vector2 dir)
    {
      this.StartCoroutine(this.BossDeathCR());
      this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnBossDeath);
    }

    [DebuggerHidden]
    private IEnumerator BossDeathCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ResourcefulRatDeathController__BossDeathCRc__Iterator0()
      {
        _this = this
      };
    }
  }

