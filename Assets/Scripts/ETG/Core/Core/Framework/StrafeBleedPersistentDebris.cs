// Decompiled with JetBrains decompiler
// Type: StrafeBleedPersistentDebris
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class StrafeBleedPersistentDebris : BraveBehaviour
  {
    public ExplosionData explosionData;
    public float CascadeTime = 3f;
    private Gun m_attachedGun;
    private bool m_initialized;
    private float m_elapsed;

    public void InitializeSelf(StrafeBleedBuff source)
    {
      this.m_initialized = true;
      this.explosionData = source.explosionData;
      Projectile component = source.GetComponent<Projectile>();
      if ((UnityEngine.Object) component.PossibleSourceGun != (UnityEngine.Object) null)
      {
        this.m_attachedGun = component.PossibleSourceGun;
        component.PossibleSourceGun.OnFinishAttack += new Action<PlayerController, Gun>(this.HandleCeaseAttack);
      }
      else
      {
        if (!(bool) (UnityEngine.Object) component || !(bool) (UnityEngine.Object) component.Owner || !(bool) (UnityEngine.Object) component.Owner.CurrentGun)
          return;
        this.m_attachedGun = component.Owner.CurrentGun;
        component.Owner.CurrentGun.OnFinishAttack += new Action<PlayerController, Gun>(this.HandleCeaseAttack);
      }
    }

    private void HandleCeaseAttack(PlayerController arg1, Gun arg2)
    {
      this.DoEffect();
      this.Disconnect();
    }

    private void Disconnect()
    {
      this.m_initialized = false;
      if (!(bool) (UnityEngine.Object) this.m_attachedGun)
        return;
      this.m_attachedGun.OnFinishAttack -= new Action<PlayerController, Gun>(this.HandleCeaseAttack);
    }

    private void Update()
    {
      if (!this.m_initialized)
        return;
      this.m_elapsed += BraveTime.DeltaTime;
      if ((double) this.m_elapsed <= (double) this.CascadeTime)
        return;
      this.DoEffect();
      this.Disconnect();
    }

    private void DoEffect()
    {
      this.explosionData.force = 0.0f;
      if ((bool) (UnityEngine.Object) this.sprite)
        Exploder.Explode((Vector3) this.sprite.WorldCenter, this.explosionData, Vector2.zero, ignoreQueues: true);
      else
        Exploder.Explode((Vector3) this.transform.position.XY(), this.explosionData, Vector2.zero, ignoreQueues: true);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    protected override void OnDestroy()
    {
      this.Disconnect();
      base.OnDestroy();
    }
  }

