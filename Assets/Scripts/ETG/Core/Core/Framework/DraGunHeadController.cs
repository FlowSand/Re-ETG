// Decompiled with JetBrains decompiler
// Type: DraGunHeadController
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
    public class DraGunHeadController : BraveBehaviour
    {
      public List<DraGunNeckPieceController> neckPieces;
      public float moveTime = 1f;
      public float maxSpeed = 3f;
      public float overrideMoveTime = 0.5f;
      public float overrideMaxSpeed = 9f;
      private bool m_initialized;
      private Vector2 m_startingHeadPosition;
      private Vector2 m_currentVelocity;

      public float? TargetX { get; set; }

      public Vector2? OverrideDesiredPosition { get; set; }

      public bool ReachedOverridePosition
      {
        get
        {
          return this.OverrideDesiredPosition.HasValue && (double) Vector2.Distance(this.transform.position.XY(), this.OverrideDesiredPosition.Value) < 0.5;
        }
      }

      [DebuggerHidden]
      public IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DraGunHeadController.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public void UpdateHead()
      {
        if (!this.m_initialized)
          return;
        Vector2 current = this.transform.position.XY();
        Vector2 target = new Vector2(current.x, this.m_startingHeadPosition.y);
        if (this.OverrideDesiredPosition.HasValue)
        {
          target = this.OverrideDesiredPosition.Value;
        }
        else
        {
          if (this.TargetX.HasValue)
            target.x = this.TargetX.Value;
          target.y = this.m_startingHeadPosition.y + Mathf.Sin((float) ((double) UnityEngine.Time.timeSinceLevelLoad * 6.2831854820251465 / 1.5)) * 1.5f;
        }
        Vector2 vector2_1 = !this.OverrideDesiredPosition.HasValue ? Vector2.SmoothDamp(current, target, ref this.m_currentVelocity, this.moveTime, this.maxSpeed, BraveTime.DeltaTime) : Vector2.SmoothDamp(current, target, ref this.m_currentVelocity, this.overrideMoveTime, this.overrideMaxSpeed, BraveTime.DeltaTime);
        this.transform.position = (Vector3) vector2_1;
        Vector2 headDelta = vector2_1 - this.m_startingHeadPosition;
        Vector2 vector2_2 = headDelta;
        if (!this.OverrideDesiredPosition.HasValue)
        {
          if ((double) Mathf.Abs(vector2_2.x) > 6.0)
            vector2_2.x = (float) (Math.Sign(vector2_2.x) * 6);
          if ((double) vector2_2.y < -5.0)
            vector2_2.y = -5f;
          if ((double) vector2_2.y > 4.0)
            vector2_2.y = 4f;
        }
        if (vector2_2 != headDelta)
        {
          this.transform.position = (Vector3) (this.m_startingHeadPosition + vector2_2);
          headDelta = vector2_2;
        }
        for (int index = 0; index < this.neckPieces.Count; ++index)
          this.neckPieces[index].UpdateHeadDelta(headDelta);
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void TriggerAnimationEvent(string eventInfo)
      {
        this.aiActor.behaviorSpeculator.TriggerAnimationEvent(eventInfo);
      }
    }

}
