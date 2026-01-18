using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BashelliskDeathController : BraveBehaviour
  {
    public VFXPool HeadVfx;

    public void Start()
    {
      this.healthHaver.ManualDeathHandling = true;
      this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
    }

    protected override void OnDestroy() => base.OnDestroy();

    private void OnBossDeath(Vector2 dir) => this.StartCoroutine(this.OnDeathCR());

    [DebuggerHidden]
    private IEnumerator OnDeathCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BashelliskDeathController__OnDeathCRc__Iterator0()
      {
        _this = this
      };
    }
  }

