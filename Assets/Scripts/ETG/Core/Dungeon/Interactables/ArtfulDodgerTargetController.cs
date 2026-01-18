// Decompiled with JetBrains decompiler
// Type: ArtfulDodgerTargetController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ArtfulDodgerTargetController : DungeonPlaceableBehaviour
  {
    public GameObject HitVFX;
    public Renderer ShadowRenderer;
    [NonSerialized]
    public bool IsBroken;
    public GameObject Sparkles;
    private ArtfulDodgerRoomController m_artfulDodger;

    private void Start()
    {
      this.m_artfulDodger = this.GetAbsoluteParentRoom().GetComponentsAbsoluteInRoom<ArtfulDodgerRoomController>()[0];
      this.m_artfulDodger.RegisterTarget(this);
      this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
      this.specRigidbody.enabled = false;
      this.sprite.renderer.enabled = false;
      this.ShadowRenderer.enabled = false;
      this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
    }

    public void Activate() => this.StartCoroutine(this.HandleActivation());

    [DebuggerHidden]
    private IEnumerator HandleActivation()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ArtfulDodgerTargetController__HandleActivationc__Iterator0()
      {
        _this = this
      };
    }

    public void ExplodeJoyously()
    {
      if (this.IsBroken)
        return;
      if ((bool) (UnityEngine.Object) this.HitVFX)
        SpawnManager.SpawnVFX(this.HitVFX, (Vector3) this.sprite.WorldCenter, Quaternion.identity);
      this.IsBroken = true;
      this.specRigidbody.enabled = false;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      this.sprite.renderer.enabled = false;
      this.ShadowRenderer.enabled = false;
      this.Sparkles.SetActive(false);
    }

    public void DisappearSadly()
    {
      if (this.IsBroken)
        return;
      LootEngine.DoDefaultItemPoof(this.sprite.WorldCenter);
      this.IsBroken = true;
      this.specRigidbody.enabled = false;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      this.sprite.renderer.enabled = false;
      this.ShadowRenderer.enabled = false;
      this.Sparkles.SetActive(false);
    }

    private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
    {
      if (this.IsBroken || !((UnityEngine.Object) rigidbodyCollision.OtherRigidbody.projectile != (UnityEngine.Object) null))
        return;
      Projectile projectile = rigidbodyCollision.OtherRigidbody.projectile;
      if (!projectile.name.StartsWith("ArtfulDodger") && (!(bool) (UnityEngine.Object) projectile.PossibleSourceGun || !projectile.PossibleSourceGun.name.StartsWith("ArtfulDodger")))
        return;
      ArtfulDodgerProjectileController component = projectile.GetComponent<ArtfulDodgerProjectileController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.hitTarget = true;
      this.ExplodeJoyously();
      if (!((UnityEngine.Object) projectile.GetComponent<PierceProjModifier>() == (UnityEngine.Object) null))
        return;
      projectile.DieInAir();
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

