// Decompiled with JetBrains decompiler
// Type: FireplaceController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class FireplaceController : BraveBehaviour, IPlayerInteractable
    {
      public GoopDefinition oilGoop;
      public GameObject FireObject;
      public SpeculativeRigidbody GrateRigidbody;
      public Vector2 goopMinOffset;
      public Vector2 goopDimensions;
      public Transform InteractPoint;
      private bool m_flipped;
      private System.Action OnInteract;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FireplaceController.<Start>c__Iterator0()
        {
          _this = this
        };
      }

      private void HandleGooped(CellData obj)
      {
        if (obj == null)
          return;
        IntVector2 intVector2 = (obj.position.ToCenterVector2() / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
        if (!DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(intVector2) || DeadlyDeadlyGoopManager.allGoopPositionMap[intVector2].goopDefinition.CanBeIgnited)
          return;
        this.OnFireExtinguished();
      }

      private void HandleBeamCollision(BasicBeamController obj)
      {
        GoopModifier component = obj.GetComponent<GoopModifier>();
        if (!(bool) (UnityEngine.Object) component || !((UnityEngine.Object) component.goopDefinition != (UnityEngine.Object) null) || component.goopDefinition.CanBeIgnited)
          return;
        this.OnFireExtinguished();
      }

      private void HandleCollision(CollisionData rigidbodyCollision)
      {
        Projectile projectile = rigidbodyCollision.OtherRigidbody.projectile;
        if (!(bool) (UnityEngine.Object) projectile || !(bool) (UnityEngine.Object) projectile.GetComponent<GoopModifier>())
          return;
        GoopModifier component = projectile.GetComponent<GoopModifier>();
        if (!((UnityEngine.Object) component.goopDefinition != (UnityEngine.Object) null) || component.goopDefinition.CanBeIgnited)
          return;
        this.OnFireExtinguished();
      }

      private void OnFireExtinguished()
      {
        IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
        GameManager.Instance.Dungeon.data[intVector2 + new IntVector2(3, 2)].OnCellGooped -= new Action<CellData>(this.HandleGooped);
        GameManager.Instance.Dungeon.data[intVector2 + new IntVector2(4, 2)].OnCellGooped -= new Action<CellData>(this.HandleGooped);
        this.GrateRigidbody.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleCollision);
        this.GrateRigidbody.specRigidbody.OnHitByBeam -= new Action<BasicBeamController>(this.HandleBeamCollision);
        this.GrateRigidbody.enabled = false;
        this.GrateRigidbody.spriteAnimator.Play();
        tk2dBaseSprite component = this.FireObject.GetComponent<tk2dBaseSprite>();
        GlobalSparksDoer.DoRandomParticleBurst(25, (Vector3) component.WorldBottomLeft, (Vector3) component.WorldTopRight, Vector3.up, 70f, 0.5f, startLifetime: new float?(0.75f), startColor: new Color?(new Color(4f, 0.3f, 0.0f)), systemType: GlobalSparksDoer.SparksType.SOLID_SPARKLES);
        GlobalSparksDoer.DoRandomParticleBurst(25, (Vector3) component.WorldBottomLeft, (Vector3) component.WorldTopRight, Vector3.up, 70f, 0.5f, startLifetime: new float?(1.5f), startColor: new Color?(new Color(4f, 0.3f, 0.0f)), systemType: GlobalSparksDoer.SparksType.SOLID_SPARKLES);
        GlobalSparksDoer.DoRandomParticleBurst(25, (Vector3) component.WorldBottomLeft, (Vector3) component.WorldTopRight, Vector3.up, 70f, 0.5f, startLifetime: new float?(2.25f), startColor: new Color?(new Color(4f, 0.3f, 0.0f)), systemType: GlobalSparksDoer.SparksType.SOLID_SPARKLES);
        GlobalSparksDoer.DoRandomParticleBurst(25, (Vector3) component.WorldBottomLeft, (Vector3) component.WorldTopRight, Vector3.up, 70f, 0.5f, startLifetime: new float?(3f), startColor: new Color?(new Color(4f, 0.3f, 0.0f)), systemType: GlobalSparksDoer.SparksType.SOLID_SPARKLES);
        this.FireObject.SetActive(false);
      }

      private void Update()
      {
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        return Vector2.Distance(point, this.InteractPoint.position.XY());
      }

      public float GetOverrideMaxDistance() => -1f;

      public void Interact(PlayerController interactor)
      {
        if (this.m_flipped)
          return;
        this.m_flipped = true;
        int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_secret_switch_01", this.gameObject);
        int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_wall_reveal_01", this.gameObject);
        GameManager.Instance.MainCameraController.DoScreenShake(new ScreenShakeSettings(0.2f, 7f, 1f, 0.0f, Vector2.left), new Vector2?());
        if (this.OnInteract == null)
          return;
        this.OnInteract();
      }

      public void OnEnteredRange(PlayerController interactor) => UnityEngine.Debug.Log((object) "ENTERED RANGE");

      public void OnExitRange(PlayerController interactor) => UnityEngine.Debug.Log((object) "EXITED RANGE");
    }

}
