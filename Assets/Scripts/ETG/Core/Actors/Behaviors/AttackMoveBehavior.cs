// Decompiled with JetBrains decompiler
// Type: AttackMoveBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class AttackMoveBehavior : BasicAttackBehavior
    {
      public AttackMoveBehavior.PositionType positionType = AttackMoveBehavior.PositionType.RelativeToRoomCenter;
      public Vector2[] Positions;
      public AttackMoveBehavior.SelectType selectType = AttackMoveBehavior.SelectType.Random;
      [InspectorShowIf("ShowN")]
      [InspectorIndent]
      public int N;
      [InspectorIndent]
      [InspectorShowIf("ShowDisallowNearest")]
      public bool DisallowNearest;
      public bool SmoothStep = true;
      public float MoveTime = 1f;
      public float MinSpeed;
      public float MaxSpeed;
      [InspectorShowIf("ShowSubsequentMoveSpeed")]
      public float SubsequentMoveSpeed = -1f;
      public bool MirrorIfCloser;
      public bool DisableCollisionDuringMove;
      [InspectorCategory("Attack")]
      public GameObject ShootPoint;
      [InspectorCategory("Attack")]
      public BulletScriptSelector bulletScript;
      [InspectorCategory("Visuals")]
      public string preMoveAnimation;
      [InspectorCategory("Visuals")]
      public string moveAnimation;
      [InspectorCategory("Visuals")]
      public bool disableGoops;
      [InspectorCategory("Visuals")]
      public bool updateFacingDirectionDuringMove = true;
      [InspectorCategory("Visuals")]
      public bool biasFacingRoomCenter;
      [InspectorCategory("Visuals")]
      public bool faceBottomCenter;
      [InspectorCategory("Visuals")]
      public bool enableShadowTrail;
      [InspectorCategory("Visuals")]
      public bool HideGun;
      [InspectorCategory("Visuals")]
      public bool animateShadow;
      [InspectorShowIf("animateShadow")]
      [InspectorCategory("Visuals")]
      public string shadowInAnim;
      [InspectorShowIf("animateShadow")]
      [InspectorCategory("Visuals")]
      public string shadowOutAnim;
      private AttackMoveBehavior.MoveState m_state;
      private Vector2 m_startPoint;
      private Vector2 m_targetPoint;
      private float m_moveTime;
      private float m_timer;
      private int m_sequenceIndex;
      private bool m_mirrorPositions;
      private BulletScriptSource m_bulletSource;
      private GoopDoer[] m_goopDoers;
      private AfterImageTrailController m_shadowTrail;
      private tk2dBaseSprite m_shadowSprite;
      private float m_shadowOutTime;

      private bool ShowN()
      {
        return this.selectType == AttackMoveBehavior.SelectType.RandomClosestN || this.selectType == AttackMoveBehavior.SelectType.RandomFurthestN;
      }

      private bool ShowDisallowNearest()
      {
        return this.selectType == AttackMoveBehavior.SelectType.Random || this.selectType == AttackMoveBehavior.SelectType.RandomClosestN;
      }

      private bool ShowSubsequentMoveSpeed()
      {
        return this.selectType == AttackMoveBehavior.SelectType.InSequence;
      }

      public override void Start()
      {
        base.Start();
        this.m_shadowTrail = this.m_aiActor.GetComponent<AfterImageTrailController>();
        if (this.bulletScript == null || this.bulletScript.IsNull)
          return;
        this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
      }

      public override BehaviorResult Update()
      {
        BehaviorResult behaviorResult = base.Update();
        if (behaviorResult != BehaviorResult.Continue)
          return behaviorResult;
        if ((UnityEngine.Object) this.m_shadowSprite == (UnityEngine.Object) null)
          this.m_shadowSprite = this.m_aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
        if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          return BehaviorResult.Continue;
        this.m_aiActor.ClearPath();
        this.m_aiActor.BehaviorOverridesVelocity = true;
        this.m_aiActor.BehaviorVelocity = Vector2.zero;
        this.m_aiAnimator.LockFacingDirection = true;
        this.m_aiAnimator.FacingDirection = -90f;
        if (this.HideGun && (bool) (UnityEngine.Object) this.m_aiShooter)
          this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (AttackMoveBehavior));
        this.State = string.IsNullOrEmpty(this.preMoveAnimation) ? AttackMoveBehavior.MoveState.Move : AttackMoveBehavior.MoveState.PreMove;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        if (this.State == AttackMoveBehavior.MoveState.PreMove)
        {
          if (!this.m_aiAnimator.IsPlaying(this.preMoveAnimation))
          {
            this.State = AttackMoveBehavior.MoveState.Move;
            return ContinuousBehaviorResult.Continue;
          }
        }
        else if (this.State == AttackMoveBehavior.MoveState.Move)
        {
          Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
          if ((double) this.m_deltaTime <= 0.0)
            return ContinuousBehaviorResult.Continue;
          Vector2 vector2 = !this.SmoothStep ? Vector2.Lerp(this.m_startPoint, this.m_targetPoint, this.m_timer / this.m_moveTime) : Vector2Extensions.SmoothStep(this.m_startPoint, this.m_targetPoint, this.m_timer / this.m_moveTime);
          if (this.animateShadow && (double) this.m_moveTime - (double) this.m_timer <= (double) this.m_shadowOutTime)
          {
            this.m_shadowOutTime = -1f;
            this.m_shadowSprite.spriteAnimator.Play(this.shadowOutAnim);
          }
          if ((double) this.m_timer > (double) this.m_moveTime)
          {
            if (this.selectType == AttackMoveBehavior.SelectType.InSequence && this.m_sequenceIndex < this.Positions.Length)
            {
              this.PlanNextMove();
            }
            else
            {
              this.m_aiActor.BehaviorVelocity = Vector2.zero;
              return ContinuousBehaviorResult.Finished;
            }
          }
          this.m_aiActor.BehaviorVelocity = (vector2 - unitCenter) / this.m_deltaTime;
          if (this.updateFacingDirectionDuringMove)
            this.UpdateFacingDirection(vector2 - unitCenter);
          this.m_timer += this.m_deltaTime;
        }
        return ContinuousBehaviorResult.Continue;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        this.State = AttackMoveBehavior.MoveState.None;
        if (this.HideGun && (bool) (UnityEngine.Object) this.m_aiShooter)
          this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (AttackMoveBehavior));
        if (!string.IsNullOrEmpty(this.preMoveAnimation))
          this.m_aiAnimator.EndAnimationIf(this.preMoveAnimation);
        if (!string.IsNullOrEmpty(this.moveAnimation))
          this.m_aiAnimator.EndAnimationIf(this.moveAnimation);
        this.m_aiAnimator.LockFacingDirection = false;
        this.m_aiActor.BehaviorOverridesVelocity = false;
        this.m_updateEveryFrame = false;
        this.UpdateCooldowns();
      }

      public void AnimationEventTriggered(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frame)
      {
        if (this.m_state == AttackMoveBehavior.MoveState.None || this.bulletScript == null || this.bulletScript.IsNull || !(clip.GetFrame(frame).eventInfo == "fire"))
          return;
        if (!(bool) (UnityEngine.Object) this.m_bulletSource)
          this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
        this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
        this.m_bulletSource.BulletScript = this.bulletScript;
        this.m_bulletSource.Initialize();
      }

      private void UpdateTargetPoint()
      {
        if (this.selectType == AttackMoveBehavior.SelectType.Random)
        {
          if (this.DisallowNearest && this.Positions.Length > 1)
          {
            Vector2 unitCenter = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
            int lastValue = -1;
            float num1 = -1f;
            for (int i = 0; i < this.Positions.Length; ++i)
            {
              Vector2 position = this.GetPosition(i);
              float num2 = Vector2.Distance(unitCenter, position);
              if (i == 0 || (double) num2 < (double) num1)
              {
                lastValue = i;
                num1 = num2;
              }
            }
            this.m_targetPoint = this.GetPosition(BraveUtility.SequentialRandomRange(0, this.Positions.Length, lastValue, excludeLastValue: true));
          }
          else
            this.m_targetPoint = this.GetPosition(UnityEngine.Random.Range(0, this.Positions.Length));
        }
        else if (this.selectType == AttackMoveBehavior.SelectType.Closest)
        {
          Vector2 unitCenter = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          int i1 = -1;
          float num3 = -1f;
          for (int i2 = 0; i2 < this.Positions.Length; ++i2)
          {
            Vector2 position = this.GetPosition(i2);
            float num4 = Vector2.Distance(unitCenter, position);
            if (i2 == 0 || (double) num4 < (double) num3)
            {
              i1 = i2;
              num3 = num4;
            }
          }
          this.m_targetPoint = this.GetPosition(i1);
        }
        else if (this.selectType == AttackMoveBehavior.SelectType.RandomClosestN)
        {
          Vector2 unitCenter = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          List<Tuple<int, float>> source = new List<Tuple<int, float>>();
          for (int i = 0; i < this.Positions.Length; ++i)
            source.Add(Tuple.Create<int, float>(i, Vector2.Distance(unitCenter, this.GetPosition(i))));
          List<Tuple<int, float>> tupleList = new List<Tuple<int, float>>((IEnumerable<Tuple<int, float>>) source.OrderBy<Tuple<int, float>, float>((Func<Tuple<int, float>, float>) (t => t.Second)));
          if (this.DisallowNearest)
            tupleList.RemoveAt(0);
          this.m_targetPoint = this.GetPosition(tupleList[UnityEngine.Random.Range(0, Mathf.Min(this.N + 1, tupleList.Count))].First);
        }
        else if (this.selectType == AttackMoveBehavior.SelectType.Furthest)
        {
          Vector2 unitCenter = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          int i3 = -1;
          float num5 = float.MaxValue;
          for (int i4 = 0; i4 < this.Positions.Length; ++i4)
          {
            Vector2 position = this.GetPosition(i4);
            float num6 = Vector2.Distance(unitCenter, position);
            if (i4 == 0 || (double) num6 > (double) num5)
            {
              i3 = i4;
              num5 = num6;
            }
          }
          this.m_targetPoint = this.GetPosition(i3);
        }
        else if (this.selectType == AttackMoveBehavior.SelectType.RandomFurthestN)
        {
          Vector2 unitCenter = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          List<Tuple<int, float>> source = new List<Tuple<int, float>>();
          for (int i = 0; i < this.Positions.Length; ++i)
            source.Add(Tuple.Create<int, float>(i, Vector2.Distance(unitCenter, this.GetPosition(i))));
          List<Tuple<int, float>> tupleList = new List<Tuple<int, float>>((IEnumerable<Tuple<int, float>>) source.OrderByDescending<Tuple<int, float>, float>((Func<Tuple<int, float>, float>) (t => t.Second)));
          this.m_targetPoint = this.GetPosition(tupleList[UnityEngine.Random.Range(0, Mathf.Min(this.N + 1, tupleList.Count))].First);
        }
        else if (this.selectType == AttackMoveBehavior.SelectType.InSequence)
          this.m_targetPoint = this.GetPosition(this.m_sequenceIndex++);
        else
          Debug.LogError((object) ("Unknown select type: " + (object) this.selectType));
      }

      private Vector2 GetPosition(int i, bool? mirror = null)
      {
        if (!mirror.HasValue)
          mirror = new bool?(this.m_mirrorPositions);
        if (this.positionType == AttackMoveBehavior.PositionType.RelativeToRoomCenter || this.positionType == AttackMoveBehavior.PositionType.RelativeToHelicopterCenter)
        {
          Vector2 center = this.m_aiActor.ParentRoom.area.Center;
          if (this.positionType == AttackMoveBehavior.PositionType.RelativeToHelicopterCenter)
          {
            float a = 0.0f;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
              PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
              if (allPlayer.healthHaver.IsAlive)
                a = Mathf.Max(a, allPlayer.specRigidbody.UnitCenter.y);
            }
            if ((double) a > 0.0)
              center.y = a;
          }
          return mirror.Value ? center + Vector2.Scale(this.Positions[i], new Vector2(-1f, 1f)) : center + this.Positions[i];
        }
        Debug.LogError((object) ("Unknown position type: " + (object) this.positionType));
        return Vector2.zero;
      }

      private void UpdateFacingDirection(Vector2 toTarget)
      {
        if (toTarget == Vector2.zero)
          return;
        toTarget.Normalize();
        if (this.biasFacingRoomCenter)
        {
          Vector2 vector2 = this.m_aiActor.ParentRoom.area.Center - this.m_aiActor.specRigidbody.UnitCenter;
          toTarget = (toTarget + 0.2f * vector2).normalized;
        }
        if (this.faceBottomCenter)
          toTarget = (new Vector2(this.m_aiActor.ParentRoom.area.UnitCenter.x, this.m_aiActor.specRigidbody.UnitCenter.y - 15f) - this.m_aiActor.specRigidbody.UnitCenter).normalized;
        this.m_aiAnimator.FacingDirection = toTarget.ToAngle();
      }

      private AttackMoveBehavior.MoveState State
      {
        get => this.m_state;
        set
        {
          if (this.m_state == value)
            return;
          this.EndState(this.m_state);
          this.m_state = value;
          this.BeginState(this.m_state);
        }
      }

      private void BeginState(AttackMoveBehavior.MoveState state)
      {
        switch (state)
        {
          case AttackMoveBehavior.MoveState.PreMove:
            this.m_aiActor.ClearPath();
            this.m_aiActor.BehaviorOverridesVelocity = true;
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
            this.m_aiAnimator.PlayUntilCancelled(this.preMoveAnimation);
            break;
          case AttackMoveBehavior.MoveState.Move:
            this.m_sequenceIndex = 0;
            if (this.MirrorIfCloser)
            {
              Vector2 position1 = this.GetPosition(0, new bool?(false));
              Vector2 position2 = this.GetPosition(0, new bool?(true));
              Vector2 unitCenter = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
              this.m_mirrorPositions = (double) Vector2.Distance(position2, unitCenter) < (double) Vector2.Distance(position1, unitCenter);
            }
            this.PlanNextMove();
            this.m_aiAnimator.LockFacingDirection = true;
            this.m_aiAnimator.PlayUntilCancelled(this.moveAnimation);
            if (this.DisableCollisionDuringMove)
            {
              this.m_aiActor.specRigidbody.CollideWithOthers = false;
              this.m_aiActor.IsGone = true;
            }
            if (this.disableGoops)
            {
              if (this.m_goopDoers == null)
                this.m_goopDoers = this.m_aiActor.GetComponents<GoopDoer>();
              for (int index = 0; index < this.m_goopDoers.Length; ++index)
                this.m_goopDoers[index].enabled = false;
            }
            if (this.enableShadowTrail)
              this.m_shadowTrail.spawnShadows = true;
            if (!this.animateShadow)
              break;
            this.m_shadowSprite.spriteAnimator.Play(this.shadowInAnim);
            this.m_shadowOutTime = this.m_shadowSprite.spriteAnimator.GetClipByName(this.shadowOutAnim).BaseClipLength;
            break;
        }
      }

      private void PlanNextMove()
      {
        this.m_startPoint = this.m_aiActor.specRigidbody.UnitCenter;
        this.UpdateTargetPoint();
        Vector2 toTarget = this.m_targetPoint - this.m_startPoint;
        float magnitude = toTarget.magnitude;
        if (this.selectType == AttackMoveBehavior.SelectType.InSequence && this.m_sequenceIndex > 1 && (double) this.SubsequentMoveSpeed > 0.0)
        {
          this.m_moveTime = magnitude / this.SubsequentMoveSpeed;
        }
        else
        {
          this.m_moveTime = this.MoveTime;
          if ((double) this.MinSpeed > 0.0)
            this.m_moveTime = Mathf.Min(this.m_moveTime, magnitude / this.MinSpeed);
          if ((double) this.MaxSpeed > 0.0)
            this.m_moveTime = Mathf.Max(this.m_moveTime, magnitude / this.MaxSpeed);
        }
        this.UpdateFacingDirection(toTarget);
        this.m_timer = 0.0f;
      }

      private void EndState(AttackMoveBehavior.MoveState state)
      {
        if (state != AttackMoveBehavior.MoveState.Move)
          return;
        if (this.DisableCollisionDuringMove)
        {
          this.m_aiActor.specRigidbody.CollideWithOthers = true;
          this.m_aiActor.IsGone = false;
        }
        if (this.disableGoops)
        {
          for (int index = 0; index < this.m_goopDoers.Length; ++index)
            this.m_goopDoers[index].enabled = true;
        }
        if (!this.enableShadowTrail)
          return;
        this.m_shadowTrail.spawnShadows = false;
      }

      public enum PositionType
      {
        RelativeToRoomCenter = 20, // 0x00000014
        RelativeToHelicopterCenter = 40, // 0x00000028
      }

      public enum SelectType
      {
        Random = 10, // 0x0000000A
        Closest = 20, // 0x00000014
        RandomClosestN = 30, // 0x0000001E
        Furthest = 40, // 0x00000028
        RandomFurthestN = 50, // 0x00000032
        InSequence = 60, // 0x0000003C
      }

      private enum MoveState
      {
        None,
        PreMove,
        Move,
      }
    }

}
