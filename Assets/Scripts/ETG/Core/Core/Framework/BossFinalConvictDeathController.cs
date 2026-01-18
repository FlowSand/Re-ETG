using System;
using UnityEngine;

#nullable disable

public class BossFinalConvictDeathController : BraveBehaviour
  {
    public void Start() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);

    protected override void OnDestroy() => base.OnDestroy();

    private void OnBossDeath(Vector2 dir)
    {
      UnityEngine.Object.FindObjectOfType<ConvictPastController>().OnBossKilled(this.transform);
    }
  }

