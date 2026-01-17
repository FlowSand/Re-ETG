// Decompiled with JetBrains decompiler
// Type: CigaretteItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class CigaretteItem : MonoBehaviour
    {
      public GameObject inAirVFX;
      private bool m_inAir = true;
      public GameObject smokeSystem;
      public GameObject sparkVFX;
      public bool DestroyOnGrounded;

      private void Start()
      {
        DebrisObject component = this.GetComponent<DebrisObject>();
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_cigarette_throw_01", this.gameObject);
        component.killTranslationOnBounce = false;
        if ((bool) (UnityEngine.Object) component)
        {
          component.OnBounced += new Action<DebrisObject>(this.OnBounced);
          component.OnGrounded += new Action<DebrisObject>(this.OnHitGround);
        }
        if (!((UnityEngine.Object) this.inAirVFX != (UnityEngine.Object) null))
          return;
        this.StartCoroutine(this.SpawnVFX());
      }

      [DebuggerHidden]
      private IEnumerator SpawnVFX()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CigaretteItem.<SpawnVFX>c__Iterator0()
        {
          _this = this
        };
      }

      private void OnBounced(DebrisObject obj)
      {
        DeadlyDeadlyGoopManager.IgniteGoopsCircle(this.transform.position.XY(), 1f);
      }

      private void OnHitGround(DebrisObject obj)
      {
        this.OnBounced(obj);
        if ((bool) (UnityEngine.Object) this.smokeSystem)
          BraveUtility.EnableEmission(this.smokeSystem.GetComponent<ParticleSystem>(), false);
        this.GetComponent<tk2dSpriteAnimator>().Stop();
        if (!this.DestroyOnGrounded)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
    }

}
