// Decompiled with JetBrains decompiler
// Type: BossFinalRobotDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class BossFinalRobotDeathController : BraveBehaviour
    {
      public void Start()
      {
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
        this.healthHaver.OverrideKillCamTime = new float?(1f);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnBossDeath(Vector2 dir)
      {
        UnityEngine.Object.FindObjectOfType<RobotPastController>().OnBossKilled(this.transform);
      }
    }

}
