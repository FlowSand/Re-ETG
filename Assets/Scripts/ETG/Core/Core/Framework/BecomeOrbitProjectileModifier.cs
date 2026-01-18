// Decompiled with JetBrains decompiler
// Type: BecomeOrbitProjectileModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class BecomeOrbitProjectileModifier : BraveBehaviour
  {
    public float MinRadius = 2f;
    public float MaxRadius = 5f;
    public int OrbitGroup = -1;
    public float SpawnVFXElapsedTimer = -1f;
    public VFXPool RespawnVFX;
    public bool TriggerOnBounce = true;

    public void Start()
    {
      Projectile projectile = this.projectile;
      if (this.TriggerOnBounce)
      {
        BounceProjModifier orAddComponent = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
        orAddComponent.numberOfBounces = Mathf.Max(orAddComponent.numberOfBounces, 1);
        orAddComponent.onlyBounceOffTiles = true;
        orAddComponent.OnBounceContext += new Action<BounceProjModifier, SpeculativeRigidbody>(this.HandleStartOrbit);
      }
      else
        this.StartOrbit();
    }

    private void StartOrbit()
    {
      this.projectile.OverrideMotionModule = (ProjectileMotionModule) new OrbitProjectileMotionModule()
      {
        MinRadius = this.MinRadius,
        MaxRadius = this.MaxRadius,
        OrbitGroup = this.OrbitGroup
      };
    }

    private void HandleStartOrbit(BounceProjModifier bouncer, SpeculativeRigidbody srb)
    {
      bouncer.projectile.specRigidbody.CollideWithTileMap = false;
      bouncer.projectile.OverrideMotionModule = (ProjectileMotionModule) new OrbitProjectileMotionModule()
      {
        MinRadius = this.MinRadius,
        MaxRadius = this.MaxRadius,
        OrbitGroup = this.OrbitGroup,
        HasSpawnVFX = true,
        SpawnVFX = this.RespawnVFX.effects[0].effects[0].effect,
        CustomSpawnVFXElapsed = this.SpawnVFXElapsedTimer
      };
    }
  }

