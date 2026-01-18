// Decompiled with JetBrains decompiler
// Type: KageBunshinController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class KageBunshinController : BraveBehaviour
  {
    public float Duration = -1f;
    [NonSerialized]
    public PlayerController Owner;
    [NonSerialized]
    public bool UsesRotationInsteadOfInversion;
    [NonSerialized]
    public float RotationAngle = 90f;

    public void InitializeOwner(PlayerController p)
    {
      this.Owner = p;
      this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
      this.GetComponentInChildren<Renderer>().material.SetColor("_FlatColor", new Color(0.25f, 0.25f, 0.25f, 1f));
      this.sprite.usesOverrideMaterial = true;
      this.Owner.PostProcessProjectile += new Action<Projectile, float>(this.HandleProjectile);
      this.Owner.PostProcessBeam += new Action<BeamController>(this.HandleBeam);
      if ((double) this.Duration <= 0.0)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject, this.Duration);
    }

    private void HandleBeam(BeamController obj)
    {
      if (!(bool) (UnityEngine.Object) obj || !(bool) (UnityEngine.Object) obj.projectile)
        return;
      GameObject gameObject = SpawnManager.SpawnProjectile(obj.projectile.gameObject, (Vector3) this.sprite.WorldCenter, Quaternion.identity);
      gameObject.GetComponent<Projectile>().Owner = (GameActor) this.Owner;
      BeamController component = gameObject.GetComponent<BeamController>();
      BasicBeamController basicBeamController = component as BasicBeamController;
      if ((bool) (UnityEngine.Object) basicBeamController)
        basicBeamController.SkipPostProcessing = true;
      component.Owner = (GameActor) this.Owner;
      component.HitsPlayers = false;
      component.HitsEnemies = true;
      Vector3 vector = (Vector3) BraveMathCollege.DegreesToVector(this.Owner.CurrentGun.CurrentAngle);
      component.Direction = (Vector2) vector;
      component.Origin = this.sprite.WorldCenter;
      GameManager.Instance.Dungeon.StartCoroutine(this.HandleFiringBeam(component));
    }

    [DebuggerHidden]
    private IEnumerator HandleFiringBeam(BeamController beam)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new KageBunshinController__HandleFiringBeamc__Iterator0()
      {
        beam = beam,
        _this = this
      };
    }

    private void Disconnect()
    {
      if (!(bool) (UnityEngine.Object) this.Owner)
        return;
      this.Owner.PostProcessProjectile -= new Action<Projectile, float>(this.HandleProjectile);
      this.Owner.PostProcessBeam -= new Action<BeamController>(this.HandleBeam);
    }

    private void HandleProjectile(Projectile sourceProjectile, float arg2)
    {
      Quaternion rotation = sourceProjectile.transform.rotation;
      if ((bool) (UnityEngine.Object) this.Owner && (bool) (UnityEngine.Object) this.Owner.CurrentGun)
      {
        Vector2 vector2 = this.Owner.unadjustedAimPoint.XY();
        float angle = (vector2 - this.Owner.CenterPosition).ToAngle();
        float num = Mathf.DeltaAngle(rotation.eulerAngles.z, angle);
        if (!BraveInput.GetInstanceForPlayer(this.Owner.PlayerIDX).IsKeyboardAndMouse())
          vector2 = this.Owner.CenterPosition + BraveMathCollege.DegreesToVector(this.Owner.CurrentGun.CurrentAngle, 10f);
        rotation = Quaternion.Euler(0.0f, 0.0f, (vector2 - this.specRigidbody.UnitCenter).ToAngle() + num);
      }
      Projectile component = UnityEngine.Object.Instantiate<GameObject>(sourceProjectile.gameObject, (Vector3) this.sprite.WorldCenter, rotation).GetComponent<Projectile>();
      component.specRigidbody.RegisterSpecificCollisionException(this.specRigidbody);
      component.SetOwnerSafe(sourceProjectile.Owner, sourceProjectile.Owner.ActorName);
      component.SetNewShooter(sourceProjectile.Shooter);
    }

    private void LateUpdate()
    {
      if (!(bool) (UnityEngine.Object) this.Owner)
        return;
      if (this.Owner.IsGhost)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      else
      {
        this.sprite.SetSprite(this.Owner.sprite.Collection, this.Owner.sprite.spriteId);
        this.sprite.FlipX = this.Owner.sprite.FlipX;
        this.sprite.transform.localPosition = this.Owner.sprite.transform.localPosition;
        if (this.UsesRotationInsteadOfInversion)
          this.specRigidbody.Velocity = (Quaternion.Euler(0.0f, 0.0f, this.RotationAngle) * (Vector3) this.Owner.specRigidbody.Velocity).XY();
        else
          this.specRigidbody.Velocity = this.Owner.specRigidbody.Velocity * -1f;
      }
    }

    private void AttractEnemies(RoomHandler room)
    {
      List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      if (activeEnemies == null)
        return;
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if ((UnityEngine.Object) activeEnemies[index].OverrideTarget == (UnityEngine.Object) null)
          activeEnemies[index].OverrideTarget = this.specRigidbody;
      }
    }

    protected override void OnDestroy()
    {
      this.Disconnect();
      base.OnDestroy();
    }
  }

