// Decompiled with JetBrains decompiler
// Type: NightclubCrowdController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class NightclubCrowdController : MonoBehaviour
    {
      public Transform[] Dancers;
      public System.Action OnPanic;
      public tk2dBaseSprite[] DanceFloors;
      private bool m_departed;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new NightclubCrowdController.<Start>c__Iterator0()
        {
          _this = this
        };
      }

      private void NightclubCrowdController_PostProcessBeam(BeamController obj) => this.Panic();

      private void NightclubCrowdController_PostProcessProjectile(
        Projectile obj,
        float effectChanceScalar)
      {
        this.Panic();
      }

      public void Panic()
      {
        if (this.m_departed)
          return;
        if (this.OnPanic != null)
          this.OnPanic();
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          GameManager.Instance.AllPlayers[index].PostProcessProjectile -= new Action<Projectile, float>(this.NightclubCrowdController_PostProcessProjectile);
          GameManager.Instance.AllPlayers[index].PostProcessBeam -= new Action<BeamController>(this.NightclubCrowdController_PostProcessBeam);
        }
        this.m_departed = true;
        this.StartCoroutine(this.HandlePanic());
        this.StartCoroutine(this.HandleDanceFloors());
      }

      [DebuggerHidden]
      private IEnumerator HandleDanceFloors()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new NightclubCrowdController.<HandleDanceFloors>c__Iterator1()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandlePanic()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new NightclubCrowdController.<HandlePanic>c__Iterator2()
        {
          _this = this
        };
      }
    }

}
