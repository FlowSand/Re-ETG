// Decompiled with JetBrains decompiler
// Type: KnockbackDoer
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
    public class KnockbackDoer : BraveBehaviour
    {
      private const float MAX_ENEMY_KNOCKBACK_MAGNITUDE = 30f;
      private const float DEFAULT_KNOCKBACK_TIME = 0.5f;
      public float weight = 10f;
      public float deathMultiplier = 5f;
      public bool knockbackWhileReflecting;
      public bool shouldBounce = true;
      public float collisionDecay = 0.5f;
      [NonSerialized]
      public float knockbackMultiplier = 1f;
      [NonSerialized]
      public float timeScalar = 1f;
      private SuperReaperController m_reaper;
      private PlayerController m_player;
      private List<ActiveKnockbackData> m_activeKnockbacks;
      private List<Vector2> m_activeContinuousKnockbacks;
      private OverridableBool m_isImmobile = new OverridableBool(false);

      private void Awake()
      {
        this.m_player = this.GetComponent<PlayerController>();
        this.m_reaper = this.GetComponent<SuperReaperController>();
        this.m_activeKnockbacks = new List<ActiveKnockbackData>();
        this.m_activeContinuousKnockbacks = new List<Vector2>();
      }

      private void Start()
      {
        if (!(bool) (UnityEngine.Object) this.specRigidbody)
          return;
        this.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
      }

      private void Update()
      {
        Vector2 zero1 = Vector2.zero;
        Vector2 zero2 = Vector2.zero;
        for (int index = this.m_activeKnockbacks.Count - 1; index >= 0; --index)
        {
          ActiveKnockbackData activeKnockback = this.m_activeKnockbacks[index];
          if (activeKnockback.curveFalloff != null)
          {
            activeKnockback.elapsedTime += BraveTime.DeltaTime * this.timeScalar;
            float time = activeKnockback.elapsedTime / activeKnockback.curveTime;
            float num = activeKnockback.curveFalloff.Evaluate(time);
            if ((double) time >= 1.0)
              this.m_activeKnockbacks.RemoveAt(index);
            else if (activeKnockback.immutable)
              zero2 += activeKnockback.initialKnockback * num;
            else
              zero1 += activeKnockback.initialKnockback * num;
          }
          else
          {
            activeKnockback.elapsedTime += BraveTime.DeltaTime * this.timeScalar;
            float num1 = activeKnockback.elapsedTime / activeKnockback.curveTime;
            float num2 = (float) (1.0 - (double) num1 * (double) num1);
            activeKnockback.knockback = activeKnockback.initialKnockback * num2;
            if (activeKnockback.immutable)
              zero2 += activeKnockback.knockback;
            else
              zero1 += activeKnockback.knockback;
            if ((double) activeKnockback.elapsedTime >= (double) activeKnockback.curveTime)
              this.m_activeKnockbacks.RemoveAt(index);
          }
        }
        bool flag = true;
        for (int index = 0; index < this.m_activeContinuousKnockbacks.Count; ++index)
        {
          if (this.m_activeContinuousKnockbacks[index] != Vector2.zero)
          {
            zero1 += this.m_activeContinuousKnockbacks[index];
            flag = false;
          }
        }
        if (flag && this.m_activeContinuousKnockbacks.Count > 0)
          this.m_activeContinuousKnockbacks.Clear();
        Vector2 vector2_1 = zero1 * this.knockbackMultiplier;
        if (this.m_isImmobile.Value)
          vector2_1 = Vector2.zero;
        if ((UnityEngine.Object) this.m_reaper != (UnityEngine.Object) null)
          this.m_reaper.knockbackComponent = vector2_1 + zero2;
        if ((UnityEngine.Object) this.m_player != (UnityEngine.Object) null)
        {
          this.m_player.knockbackComponent = vector2_1;
          this.m_player.immutableKnockbackComponent = zero2;
        }
        if (!((UnityEngine.Object) this.aiActor != (UnityEngine.Object) null))
          return;
        Vector2 vector2_2 = vector2_1 + zero2;
        float magnitude = vector2_2.magnitude;
        this.aiActor.KnockbackVelocity = vector2_2.normalized * Mathf.Min(magnitude, 30f);
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void SetImmobile(bool value, string reason = "")
      {
        if (string.IsNullOrEmpty(reason))
        {
          this.m_isImmobile.BaseValue = value;
          if (!value)
            this.m_isImmobile.ClearOverrides();
        }
        else
          this.m_isImmobile.SetOverride(reason, value);
        if (!value || !(bool) (UnityEngine.Object) this.specRigidbody)
          return;
        this.specRigidbody.Velocity = Vector2.zero;
      }

      public void TriggerTemporaryKnockbackInvulnerability(float duration)
      {
        this.StartCoroutine(this.HandleKnockbackInvulnerabilityPeriod(duration));
      }

      [DebuggerHidden]
      private IEnumerator HandleKnockbackInvulnerabilityPeriod(float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new KnockbackDoer.<HandleKnockbackInvulnerabilityPeriod>c__Iterator0()
        {
          duration = duration,
          _this = this
        };
      }

      public ActiveKnockbackData ApplyKnockback(Vector2 direction, float force, bool immutable = false)
      {
        return this.m_isImmobile.Value ? (ActiveKnockbackData) null : this.ApplyKnockback(direction, force, 0.5f, immutable);
      }

      public ActiveKnockbackData ApplyKnockback(
        Vector2 direction,
        float force,
        float time,
        bool immutable = false)
      {
        if (this.m_isImmobile.Value)
          return (ActiveKnockbackData) null;
        ActiveKnockbackData activeKnockbackData = new ActiveKnockbackData(direction.normalized * (force / (this.weight / 10f)), time, immutable);
        this.m_activeKnockbacks.Add(activeKnockbackData);
        return activeKnockbackData;
      }

      public ActiveKnockbackData ApplyKnockback(
        Vector2 direction,
        float force,
        AnimationCurve customFalloff,
        float time,
        bool immutable = false)
      {
        if (this.m_isImmobile.Value)
          return (ActiveKnockbackData) null;
        ActiveKnockbackData activeKnockbackData = new ActiveKnockbackData(direction.normalized * (force / (this.weight / 10f)), customFalloff, time, immutable);
        this.m_activeKnockbacks.Add(activeKnockbackData);
        return activeKnockbackData;
      }

      public ActiveKnockbackData ApplySourcedKnockback(
        Vector2 direction,
        float force,
        GameObject source,
        bool immutable = false)
      {
        return this.m_isImmobile.Value ? (ActiveKnockbackData) null : this.ApplySourcedKnockback(direction, force, 0.5f, source, immutable);
      }

      public ActiveKnockbackData ApplySourcedKnockback(
        Vector2 direction,
        float force,
        float time,
        GameObject source,
        bool immutable = false)
      {
        if (this.m_isImmobile.Value)
          return (ActiveKnockbackData) null;
        if (this.CheckSourceInKnockbacks(source))
          return (ActiveKnockbackData) null;
        ActiveKnockbackData activeKnockbackData = this.ApplyKnockback(direction, force, time, immutable);
        activeKnockbackData.sourceObject = source;
        return activeKnockbackData;
      }

      public ActiveKnockbackData ApplySourcedKnockback(
        Vector2 direction,
        float force,
        AnimationCurve customFalloff,
        float time,
        GameObject source,
        bool immutable = false)
      {
        if (this.m_isImmobile.Value)
          return (ActiveKnockbackData) null;
        if (this.CheckSourceInKnockbacks(source))
          return (ActiveKnockbackData) null;
        ActiveKnockbackData activeKnockbackData = this.ApplyKnockback(direction, force, customFalloff, time, immutable);
        activeKnockbackData.sourceObject = source;
        return activeKnockbackData;
      }

      public int ApplyContinuousKnockback(Vector2 direction, float force)
      {
        this.m_activeContinuousKnockbacks.Add(direction.normalized * (force / (this.weight / 10f)));
        return this.m_activeContinuousKnockbacks.Count - 1;
      }

      public void UpdateContinuousKnockback(Vector2 direction, float force, int id)
      {
        if (this.m_activeContinuousKnockbacks.Count <= id)
          return;
        this.m_activeContinuousKnockbacks[id] = direction.normalized * (force / (this.weight / 10f));
      }

      public void EndContinuousKnockback(int id)
      {
        if (id < 0 || id >= this.m_activeContinuousKnockbacks.Count)
          return;
        this.m_activeContinuousKnockbacks[id] = Vector2.zero;
      }

      public void ClearContinuousKnockbacks()
      {
        if (this.m_activeContinuousKnockbacks == null)
          return;
        for (int id = 0; id < this.m_activeContinuousKnockbacks.Count; ++id)
          this.EndContinuousKnockback(id);
      }

      protected virtual void OnCollision(CollisionData collision)
      {
        if (collision.collisionType == CollisionData.CollisionType.Rigidbody && collision.OtherRigidbody.Velocity != Vector2.zero || !((UnityEngine.Object) this.healthHaver != (UnityEngine.Object) null) || !this.healthHaver.IsDead)
          return;
        for (int index = 0; index < this.m_activeKnockbacks.Count; ++index)
        {
          if ((double) Mathf.Sign(collision.Normal.x) != (double) Mathf.Sign(this.m_activeKnockbacks[index].initialKnockback.x) && (double) collision.Normal.x != 0.0)
          {
            if (this.shouldBounce)
              this.m_activeKnockbacks[index].initialKnockback = Vector2.Scale(this.m_activeKnockbacks[index].initialKnockback, new Vector2(-1f, 1f));
            this.m_activeKnockbacks[index].initialKnockback = Vector2.Scale(this.m_activeKnockbacks[index].initialKnockback, new Vector2(1f - this.collisionDecay, 1f));
          }
          if ((double) Mathf.Sign(collision.Normal.y) != (double) Mathf.Sign(this.m_activeKnockbacks[index].initialKnockback.y) && (double) collision.Normal.y != 0.0)
          {
            if (this.shouldBounce)
              this.m_activeKnockbacks[index].initialKnockback = Vector2.Scale(this.m_activeKnockbacks[index].initialKnockback, new Vector2(1f, -1f));
            this.m_activeKnockbacks[index].initialKnockback = Vector2.Scale(this.m_activeKnockbacks[index].initialKnockback, new Vector2(1f, 1f - this.collisionDecay));
          }
        }
      }

      private bool CheckSourceInKnockbacks(GameObject source)
      {
        if ((UnityEngine.Object) source == (UnityEngine.Object) null)
          return false;
        for (int index = 0; index < this.m_activeKnockbacks.Count; ++index)
        {
          if ((UnityEngine.Object) this.m_activeKnockbacks[index].sourceObject == (UnityEngine.Object) source)
            return true;
        }
        return false;
      }
    }

}
