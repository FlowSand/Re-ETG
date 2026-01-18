// Decompiled with JetBrains decompiler
// Type: BabyDragunController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class BabyDragunController : MonoBehaviour
  {
    public List<Transform> Segments;
    private List<tk2dSprite> SegmentSprites = new List<tk2dSprite>();
    public AIAnimator HeadAnimator;
    private SpeculativeRigidbody m_srb;
    private Vector2 m_lastBasePosition;
    private Vector2 m_lastVelocityAvg;
    private List<BabyDragunSegment> m_segmentData = new List<BabyDragunSegment>();
    public float SinWave1Multiplier = 1f;
    public float SinTimeMultiplier = 1f;
    public float SinAmplitude = 0.5f;
    [Header("Enemy Stats")]
    public float EnemyBaseSpeed = 4.5f;
    public float EnemyFastSpeed = 6f;
    private float m_concernTimer;
    private PooledLinkedList<Vector2> m_path;
    private int m_lastChangedFacingFrame = -100;
    private float m_pathSegmentLength;
    private AIBulletBank m_bulletBank;
    private BehaviorSpeculator m_behaviorSpeculator;

    public bool IsEnemy { get; set; }

    public Vector2 EnemyTargetPos { get; set; }

    public float EnemySpeed { get; set; }

    public DraGunController Parent { get; set; }

    public AutoAimTarget ParentHeart { get; set; }

    private void Start()
    {
      this.m_srb = this.GetComponent<SpeculativeRigidbody>();
      this.m_srb.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.OnPostRigidbodyMovement);
      this.m_lastBasePosition = this.transform.position.XY();
      this.m_path = new PooledLinkedList<Vector2>();
      this.m_path.AddLast(this.m_lastBasePosition);
      this.m_path.AddLast(this.m_lastBasePosition);
      float num = 0.0f;
      for (int index = 0; index < this.Segments.Count; ++index)
      {
        BabyDragunSegment babyDragunSegment = new BabyDragunSegment()
        {
          lastPosition = this.Segments[index].position.XY()
        };
        babyDragunSegment.initialStartingDistance = index != 0 ? (this.m_segmentData[this.m_segmentData.Count - 1].lastPosition - babyDragunSegment.lastPosition).magnitude : (this.m_lastBasePosition - babyDragunSegment.lastPosition).magnitude;
        babyDragunSegment.pathDist = num;
        num += babyDragunSegment.initialStartingDistance;
        this.m_segmentData.Add(babyDragunSegment);
        this.SegmentSprites.Add(this.Segments[index].GetComponent<tk2dSprite>());
      }
    }

    public void Update()
    {
      if (!this.IsEnemy || (double) BraveTime.DeltaTime <= 0.0)
        return;
      Vector2 enemyTargetPos = this.EnemyTargetPos;
      if ((bool) (UnityEngine.Object) this.Parent.head && (double) this.Parent.head.transform.position.x + 5.0 > (double) enemyTargetPos.x)
        enemyTargetPos.x = this.Parent.head.transform.position.x + 5f;
      Vector2 vector2_1 = enemyTargetPos + new Vector2(1f, 0.0f).Rotate((float) ((double) UnityEngine.Time.realtimeSinceStartup / 3.0 * 360.0)).Scale(3f, 1.5f);
      if (this.ParentHeart.enabled)
      {
        if ((double) this.m_concernTimer <= 0.0)
        {
          this.EnemySpeed = this.EnemyFastSpeed;
          this.EnemyTargetPos += new Vector2(-5.5f, -3.4f);
          this.m_behaviorSpeculator.InterruptAndDisable();
        }
        this.m_concernTimer += BraveTime.DeltaTime;
        vector2_1 = this.EnemyTargetPos + new Vector2(1.5f, 0.0f).Rotate((float) ((double) UnityEngine.Time.realtimeSinceStartup / 2.0 * -360.0));
      }
      Vector2 vector2_2 = vector2_1 - this.transform.position.XY();
      if ((double) vector2_2.magnitude < (double) this.EnemySpeed * (double) BraveTime.DeltaTime)
        this.m_srb.Velocity = vector2_2 / BraveTime.DeltaTime;
      else
        this.m_srb.Velocity = vector2_2.normalized * this.EnemySpeed;
    }

    private void UpdatePath(Vector2 newPosition)
    {
      float num = Vector2.Distance(newPosition, this.m_path.First.Value);
      for (int index = 0; index < this.m_segmentData.Count; ++index)
      {
        BabyDragunSegment babyDragunSegment = this.m_segmentData[index];
        babyDragunSegment.pathDist += num;
        this.m_segmentData[index] = babyDragunSegment;
      }
      if ((double) this.m_pathSegmentLength > 0.05000000074505806)
        this.m_path.AddFirst(newPosition);
      else
        this.m_path.First.Value = newPosition;
      this.m_pathSegmentLength = Vector2.Distance(this.m_path.First.Value, this.m_path.First.Next.Value);
    }

    private float GetPerp(float totalPathDist, int segmentIndex)
    {
      return Mathf.Lerp(0.0f, Mathf.Sin((float) ((double) totalPathDist * (double) this.SinWave1Multiplier + (double) UnityEngine.Time.time * (double) this.SinTimeMultiplier)) * this.SinAmplitude, (float) (1.0 - (double) segmentIndex / (1.0 * (double) this.Segments.Count)));
    }

    private void UpdatePathSegment(
      LinkedListNode<Vector2> pathNode,
      float totalPathDist,
      float thisNodeDist,
      int segmentIndex)
    {
      BabyDragunSegment babyDragunSegment = this.m_segmentData[segmentIndex];
      float num1 = segmentIndex > 0 ? this.m_segmentData[segmentIndex - 1].pathDist : 0.0f;
      Transform segment = this.Segments[segmentIndex];
      float num2 = babyDragunSegment.pathDist - num1;
      float num3 = babyDragunSegment.initialStartingDistance;
      bool flag = false;
      if ((double) num2 < (double) num3)
        num3 = num2;
      float num4 = -thisNodeDist;
      for (; pathNode.Next != null; pathNode = pathNode.Next)
      {
        float num5 = Vector2.Distance(pathNode.Next.Value, pathNode.Value);
        if ((double) num4 + (double) num5 >= (double) num3)
        {
          float thisNodeDist1 = num3 - num4;
          if (!flag)
          {
            Vector2 vector2 = Vector2.Lerp(pathNode.Value, pathNode.Next.Value, thisNodeDist1 / num5);
            segment.position = (Vector3) vector2;
            this.SegmentSprites[segmentIndex].UpdateZDepth();
          }
          babyDragunSegment.pathDist = totalPathDist + num3;
          if (segmentIndex + 1 < this.m_segmentData.Count)
          {
            this.UpdatePathSegment(pathNode, totalPathDist + num3, thisNodeDist1, segmentIndex + 1);
          }
          else
          {
            while (this.m_path.Last != pathNode.Next)
              this.m_path.RemoveLast();
          }
          this.m_segmentData[segmentIndex] = babyDragunSegment;
          return;
        }
        num4 += num5;
      }
      this.m_segmentData[segmentIndex] = babyDragunSegment;
    }

    public void OnPostRigidbodyMovement(SpeculativeRigidbody s, Vector2 v, IntVector2 iv)
    {
      if (!this.enabled)
        return;
      Vector2 newPosition = this.transform.position.XY();
      this.UpdatePath(newPosition);
      this.UpdatePathSegment(this.m_path.First, 0.0f, 0.0f, 0);
      this.m_lastBasePosition = newPosition;
      this.m_lastVelocityAvg = BraveMathCollege.MovingAverage(this.m_lastVelocityAvg, this.m_srb.Velocity, 8);
      if (float.IsNaN(this.m_lastVelocityAvg.x) || float.IsNaN(this.m_lastVelocityAvg.y))
        this.m_lastVelocityAvg = Vector2.zero;
      this.HeadAnimator.LockFacingDirection = true;
      if (UnityEngine.Time.frameCount - this.m_lastChangedFacingFrame < 3)
        return;
      this.HeadAnimator.FacingDirection = this.m_lastVelocityAvg.ToAngle();
      this.m_lastChangedFacingFrame = UnityEngine.Time.frameCount;
    }

    public void BecomeEnemy(DraGunController draGunController)
    {
      if (this.IsEnemy)
        return;
      this.GetComponent<PlayerOrbital>().DecoupleBabyDragun();
      this.m_srb.CollideWithOthers = false;
      this.IsEnemy = true;
      this.EnemyTargetPos = (Vector2) (draGunController.transform.position + new Vector3(10f, 6f));
      this.EnemySpeed = this.EnemyBaseSpeed;
      this.Parent = UnityEngine.Object.FindObjectOfType<DraGunController>();
      this.ParentHeart = this.Parent.GetComponentsInChildren<AutoAimTarget>(true)[0];
      foreach (tk2dBaseSprite componentsInChild in this.GetComponentsInChildren<tk2dBaseSprite>())
      {
        if (!componentsInChild.IsOutlineSprite)
          ++componentsInChild.HeightOffGround;
      }
      this.m_bulletBank = this.GetComponent<AIBulletBank>();
      this.m_bulletBank.ActorName = this.Parent.aiActor.GetActorName();
      this.m_bulletBank.enabled = true;
      this.m_behaviorSpeculator = this.GetComponent<BehaviorSpeculator>();
      this.m_behaviorSpeculator.enabled = true;
      this.m_behaviorSpeculator.AttackCooldown = 5f;
    }
  }

