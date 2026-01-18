// Decompiled with JetBrains decompiler
// Type: StickyGrenadePersistentDebris
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class StickyGrenadePersistentDebris : BraveBehaviour
  {
    public ExplosionData explosionData;
    private PlayerController m_player;
    private Gun m_attachedGun;

    public void InitializeSelf(StickyGrenadeBuff source)
    {
      this.explosionData = source.explosionData;
      Projectile component = source.GetComponent<Projectile>();
      if ((UnityEngine.Object) component.PossibleSourceGun != (UnityEngine.Object) null)
      {
        this.m_attachedGun = component.PossibleSourceGun;
        this.m_player = component.PossibleSourceGun.CurrentOwner as PlayerController;
        component.PossibleSourceGun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.ExplodeOnReload);
        if (!(bool) (UnityEngine.Object) this.m_player)
          return;
        this.m_player.GunChanged += new Action<Gun, Gun, bool>(this.GunChanged);
      }
      else
      {
        if (!(bool) (UnityEngine.Object) component || !(bool) (UnityEngine.Object) component.Owner || !(bool) (UnityEngine.Object) component.Owner.CurrentGun)
          return;
        this.m_attachedGun = component.Owner.CurrentGun;
        this.m_player = component.Owner as PlayerController;
        component.Owner.CurrentGun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.ExplodeOnReload);
        if (!(bool) (UnityEngine.Object) this.m_player)
          return;
        this.m_player.GunChanged += new Action<Gun, Gun, bool>(this.GunChanged);
      }
    }

    private void Disconnect()
    {
      if ((bool) (UnityEngine.Object) this.m_player)
        this.m_player.GunChanged -= new Action<Gun, Gun, bool>(this.GunChanged);
      if (!(bool) (UnityEngine.Object) this.m_attachedGun)
        return;
      this.m_attachedGun.OnReloadPressed -= new Action<PlayerController, Gun, bool>(this.ExplodeOnReload);
    }

    private void GunChanged(Gun arg1, Gun arg2, bool newGun)
    {
      this.Disconnect();
      this.DoEffect();
    }

    private void ExplodeOnReload(PlayerController arg1, Gun arg2, bool actual)
    {
      this.Disconnect();
      this.DoEffect();
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

