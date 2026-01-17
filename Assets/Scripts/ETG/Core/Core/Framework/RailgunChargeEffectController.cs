// Decompiled with JetBrains decompiler
// Type: RailgunChargeEffectController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class RailgunChargeEffectController : BraveBehaviour
    {
      public RailgunChargeEffectController.LineChargeMode lineMode;
      public GameObject childLinePrefab;
      public float Width = 1f;
      public float NewLineFrequency = 0.5f;
      public float LineTraversalTime = 0.5f;
      public float StopCreatingLinesTime = 3f;
      public bool SequentialLinesReduceTraversalTime;
      [ShowInInspectorIf("lineMode", 0, false)]
      public float DistanceStart = 1f;
      [ShowInInspectorIf("lineMode", 1, false)]
      public float AngleStart = 90f;
      [ShowInInspectorIf("lineMode", 2, false)]
      public float SolidAngleStart = 60f;
      [ShowInInspectorIf("lineMode", 2, false)]
      public float SolidRotationSpeed = 180f;
      [ShowInInspectorIf("lineMode", 4, false)]
      public float ScalingDistanceDepth = 0.25f;
      [ShowInInspectorIf("lineMode", 4, false)]
      public float ScalingDistanceStart = 1f;
      [ShowInInspectorIf("lineMode", 4, false)]
      public float ScalingPower = 3f;
      public bool SmoothLerpIn;
      public bool SmoothLerpOut;
      public bool UseRaycast;
      public bool DestroyedOnCompletion;
      public float TargetHeightOffGround = -0.5f;
      public Gradient ColorGradient;
      public ParticleSystem ImpactParticles;
      private Gun m_ownerGun;
      private tk2dTiledSprite m_sprite;
      private List<tk2dTiledSprite> m_childLines;
      private float m_lineTimer;
      private float m_totalTimer;
      private float m_modTraversalTime;
      private bool m_hasConverged;
      public float? overrideBeamLength;
      [NonSerialized]
      public bool IsManuallyControlled;
      [NonSerialized]
      public float ManualCompletionPercentage;
      private Transform m_cachedParentTransform;
      private Dictionary<tk2dTiledSprite, float> CompletionMap;

      private void Start()
      {
        this.m_sprite = this.GetComponent<tk2dTiledSprite>();
        this.m_modTraversalTime = this.LineTraversalTime;
        this.m_sprite.color = this.ColorGradient.Evaluate(1f);
        this.m_ownerGun = this.gameObject.GetComponentInParent<Gun>();
        this.m_childLines = new List<tk2dTiledSprite>();
        this.UpdateAngleAndLength();
        if (this.lineMode != RailgunChargeEffectController.LineChargeMode.VERTICAL_CONVERGE)
          return;
        int num = (int) AkSoundEngine.PostEvent("Play_WPN_dawnhammer_charge_01", this.gameObject);
      }

      private void OnEnable() => this.m_cachedParentTransform = this.transform.parent;

      private tk2dTiledSprite CreateDuplicate(bool forceVisible = false)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.childLinePrefab);
        gameObject.transform.parent = this.transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.GetComponent<Renderer>().enabled = forceVisible || this.renderer.enabled;
        return gameObject.GetComponent<tk2dTiledSprite>();
      }

      private void UpdateAngleAndLength()
      {
        if ((UnityEngine.Object) this.m_cachedParentTransform != (UnityEngine.Object) this.transform.parent)
        {
          this.m_ownerGun = this.gameObject.GetComponentInParent<Gun>();
          this.m_cachedParentTransform = this.transform.parent;
        }
        if (this.lineMode == RailgunChargeEffectController.LineChargeMode.VERTICAL_CONVERGE)
        {
          this.m_sprite.dimensions = new Vector2(270f, this.Width);
          if ((bool) (UnityEngine.Object) this.ImpactParticles && this.ImpactParticles.isPlaying)
            this.ImpactParticles.Stop();
        }
        else
        {
          if (this.lineMode == RailgunChargeEffectController.LineChargeMode.SCALING_PARALLEL && (bool) (UnityEngine.Object) this.m_ownerGun && !this.m_ownerGun.IsFiring)
          {
            SpawnManager.Despawn(this.gameObject);
            return;
          }
          if ((bool) (UnityEngine.Object) this.m_ownerGun)
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.m_ownerGun.CurrentAngle);
          int mask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle);
          bool flag = false;
          RaycastResult result;
          if (this.overrideBeamLength.HasValue)
          {
            result = (RaycastResult) null;
          }
          else
          {
            flag = PhysicsEngine.Instance.Raycast(this.transform.position.XY(), (Vector2) this.transform.right, 30f, out result, collideWithRigidbodies: false, rayMask: mask);
            if (this.UseRaycast)
            {
              RaycastResult.Pool.Free(ref result);
              flag |= PhysicsEngine.Instance.Raycast(this.transform.position.XY(), (Vector2) this.transform.right, 30f, out result, false, rayMask: mask);
            }
          }
          if (flag && result != null)
          {
            if ((bool) (UnityEngine.Object) this.m_sprite)
              this.m_sprite.dimensions = new Vector2(result.Distance / (1f / 16f), this.Width);
            if ((bool) (UnityEngine.Object) this.ImpactParticles)
            {
              this.ImpactParticles.transform.position = result.Contact.ToVector3ZUp(result.Contact.y - this.TargetHeightOffGround);
              if (this.m_hasConverged)
              {
                if (!this.ImpactParticles.isPlaying)
                  this.ImpactParticles.Play();
              }
              else if (this.ImpactParticles.isPlaying)
                this.ImpactParticles.Stop();
            }
          }
          else if (this.overrideBeamLength.HasValue)
          {
            if ((bool) (UnityEngine.Object) this.m_sprite)
              this.m_sprite.dimensions = new Vector2(this.overrideBeamLength.Value * 16f, this.Width);
            if ((bool) (UnityEngine.Object) this.ImpactParticles && (bool) (UnityEngine.Object) this.m_sprite)
            {
              this.ImpactParticles.transform.position = this.m_sprite.transform.position + new Vector3(0.0f, -this.overrideBeamLength.Value, -this.overrideBeamLength.Value);
              if (!this.ImpactParticles.isPlaying)
                this.ImpactParticles.Play();
            }
          }
          else
          {
            if ((bool) (UnityEngine.Object) this.m_sprite)
              this.m_sprite.dimensions = new Vector2(480f, this.Width);
            if ((bool) (UnityEngine.Object) this.ImpactParticles && this.ImpactParticles.isPlaying)
              this.ImpactParticles.Stop();
          }
          RaycastResult.Pool.Free(ref result);
        }
        if (!(bool) (UnityEngine.Object) this.m_sprite)
          return;
        this.m_sprite.IsPerpendicular = false;
        this.m_sprite.HeightOffGround = this.TargetHeightOffGround;
        this.m_sprite.UpdateZDepth();
        for (int index = 0; index < this.m_childLines.Count; ++index)
        {
          this.m_childLines[index].dimensions = this.m_sprite.dimensions;
          if (this.lineMode == RailgunChargeEffectController.LineChargeMode.SCALING_PARALLEL && this.CompletionMap != null && this.CompletionMap.ContainsKey(this.m_childLines[index]))
          {
            float t = Mathf.Pow(this.CompletionMap[this.m_childLines[index]], this.ScalingPower);
            this.m_childLines[index].dimensions = Vector2.Lerp(new Vector2(this.Width * 2f, this.Width * 2f), this.m_childLines[index].dimensions, t);
          }
          this.m_childLines[index].IsPerpendicular = false;
          this.m_childLines[index].HeightOffGround = this.TargetHeightOffGround;
          this.m_childLines[index].UpdateZDepth();
        }
      }

      public void OnSpawned()
      {
        this.m_totalTimer = 0.0f;
        this.m_modTraversalTime = this.LineTraversalTime;
        this.m_lineTimer = 0.0f;
        this.UpdateAngleAndLength();
        this.m_hasConverged = false;
      }

      public void OnDespawned()
      {
        this.StopAllCoroutines();
        for (int index = 0; index < this.m_childLines.Count; ++index)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_childLines[index].gameObject);
        this.m_childLines.Clear();
      }

      [DebuggerHidden]
      private IEnumerator HandleLine_ScalingParallel(float modTraversalTime)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RailgunChargeEffectController.<HandleLine_ScalingParallel>c__Iterator0()
        {
          modTraversalTime = modTraversalTime,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleLine_SequentialParallel(float modTraversalTime)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RailgunChargeEffectController.<HandleLine_SequentialParallel>c__Iterator1()
        {
          modTraversalTime = modTraversalTime,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleLine_PyramidalConverge(float modTraversalTime)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RailgunChargeEffectController.<HandleLine_PyramidalConverge>c__Iterator2()
        {
          modTraversalTime = modTraversalTime,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleLine_VerticalConverge(float modTraversalTime)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RailgunChargeEffectController.<HandleLine_VerticalConverge>c__Iterator3()
        {
          modTraversalTime = modTraversalTime,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleLine_TriangularConverge(float modTraversalTime)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RailgunChargeEffectController.<HandleLine_TriangularConverge>c__Iterator4()
        {
          modTraversalTime = modTraversalTime,
          $this = this
        };
      }

      private void Update()
      {
        this.m_lineTimer -= BraveTime.DeltaTime;
        this.m_totalTimer += BraveTime.DeltaTime;
        if ((double) this.m_totalTimer < (double) this.StopCreatingLinesTime)
        {
          if (this.lineMode == RailgunChargeEffectController.LineChargeMode.SEQUENTIAL_PARALLEL && (double) this.m_lineTimer <= 0.0)
          {
            this.StartCoroutine(this.HandleLine_SequentialParallel(!this.SequentialLinesReduceTraversalTime ? this.LineTraversalTime : this.m_modTraversalTime));
            this.m_lineTimer += this.NewLineFrequency;
            this.m_modTraversalTime -= this.NewLineFrequency;
          }
          else if (this.lineMode == RailgunChargeEffectController.LineChargeMode.TRIANGULAR_CONVERGE && (double) this.m_lineTimer <= 0.0)
          {
            this.StartCoroutine(this.HandleLine_TriangularConverge(!this.SequentialLinesReduceTraversalTime ? this.LineTraversalTime : this.m_modTraversalTime));
            this.m_lineTimer += this.NewLineFrequency;
            this.m_modTraversalTime -= this.NewLineFrequency;
          }
          else if (this.lineMode == RailgunChargeEffectController.LineChargeMode.PYRAMIDAL_CONVERGE && (double) this.m_lineTimer <= 0.0)
          {
            this.StartCoroutine(this.HandleLine_PyramidalConverge(!this.SequentialLinesReduceTraversalTime ? this.LineTraversalTime : this.m_modTraversalTime));
            this.m_lineTimer += this.NewLineFrequency;
            this.m_modTraversalTime -= this.NewLineFrequency;
          }
          else if (this.lineMode == RailgunChargeEffectController.LineChargeMode.VERTICAL_CONVERGE && (double) this.m_lineTimer <= 0.0)
          {
            this.StartCoroutine(this.HandleLine_VerticalConverge(!this.SequentialLinesReduceTraversalTime ? this.LineTraversalTime : this.m_modTraversalTime));
            this.m_lineTimer += this.NewLineFrequency;
            this.m_modTraversalTime -= this.NewLineFrequency;
          }
          else
          {
            if (this.lineMode != RailgunChargeEffectController.LineChargeMode.SCALING_PARALLEL || (double) this.m_lineTimer > 0.0)
              return;
            this.StartCoroutine(this.HandleLine_ScalingParallel(!this.SequentialLinesReduceTraversalTime ? this.LineTraversalTime : this.m_modTraversalTime));
            this.m_lineTimer += this.NewLineFrequency;
            this.m_modTraversalTime -= this.NewLineFrequency;
          }
        }
        else
        {
          this.m_hasConverged = true;
          if (!this.DestroyedOnCompletion)
            return;
          SpawnManager.Despawn(this.gameObject);
        }
      }

      private void LateUpdate() => this.UpdateAngleAndLength();

      protected override void OnDestroy() => base.OnDestroy();

      public enum LineChargeMode
      {
        SEQUENTIAL_PARALLEL,
        TRIANGULAR_CONVERGE,
        PYRAMIDAL_CONVERGE,
        VERTICAL_CONVERGE,
        SCALING_PARALLEL,
      }
    }

}
