// Decompiled with JetBrains decompiler
// Type: PathMover
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class PathMover : BraveBehaviour
    {
      public bool Paused;
      [NonSerialized]
      protected float m_pathSpeed = 1f;
      [FormerlySerializedAs("PathSpeed")]
      public float OriginalPathSpeed = 1f;
      public float AdditionalNodeDelay;
      [NonSerialized]
      public Vector2 nodeOffset;
      [NonSerialized]
      public SerializedPath Path;
      [NonSerialized]
      public RoomHandler RoomHandler;
      [NonSerialized]
      public int PathStartNode;
      [NonSerialized]
      public bool IsUsingAlternateTargets;
      [NonSerialized]
      public bool ForceCornerDelayHack;
      public Action<Vector2, Vector2, bool> OnNodeReached;
      private Vector2 prevMotionVec = Vector2.zero;
      private int m_currentIndex;
      private int m_currentIndexDelta = 1;

      public float AbsPathSpeed => Mathf.Abs(this.PathSpeed);

      public float PathSpeed
      {
        get => this.m_pathSpeed;
        set
        {
          bool flag = false;
          if ((double) Mathf.Sign(value) != (double) Mathf.Sign(this.m_pathSpeed))
            flag = true;
          this.m_pathSpeed = value;
          if (!flag)
            return;
          this.HandleDirectionFlip();
        }
      }

      public int PreviousIndex
      {
        get
        {
          return (this.m_currentIndex + this.m_currentIndexDelta * -1 + this.Path.nodes.Count) % this.Path.nodes.Count;
        }
      }

      public int CurrentIndex => this.m_currentIndex;

      public static SerializedPath GetRoomPath(RoomHandler room, int pathIndex)
      {
        return room.area.runtimePrototypeData.paths[pathIndex];
      }

      private void Awake() => this.m_pathSpeed = this.OriginalPathSpeed;

      public void Start()
      {
        if (this.Path == null)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this);
        }
        else
        {
          if (this.Path != null && (double) this.Path.overrideSpeed != -1.0)
            this.PathSpeed = this.Path.overrideSpeed;
          this.specRigidbody.OnPathTargetReached += new System.Action(this.PathTargetReached);
          this.m_currentIndex = this.PathStartNode;
          if ((bool) (UnityEngine.Object) this.talkDoer)
          {
            this.Paused = true;
            this.PathToNextNode();
          }
          else
          {
            this.transform.position = (Vector3) (this.RoomHandler.area.basePosition.ToVector2() + this.Path.nodes[this.PathStartNode].RoomPosition + this.nodeOffset);
            this.specRigidbody.Reinitialize();
            this.PathTargetReached();
          }
        }
      }

      public void Update()
      {
        this.specRigidbody.PathSpeed = !this.Paused ? Mathf.Abs(this.PathSpeed) : 0.0f;
      }

      protected override void OnDestroy() => base.OnDestroy();

      protected float GetTotalPathLength()
      {
        float totalPathLength = 0.0f;
        for (int index = 0; index < this.Path.nodes.Count - 1; ++index)
          totalPathLength += (this.Path.nodes[index + 1].RoomPosition - this.Path.nodes[index].RoomPosition).magnitude;
        if (this.Path.wrapMode == SerializedPath.SerializedPathWrapMode.Loop)
          totalPathLength += (this.Path.nodes[0].RoomPosition - this.Path.nodes[this.Path.nodes.Count - 1].RoomPosition).magnitude;
        return totalPathLength;
      }

      public Vector2 GetPositionOfNode(int nodeIndex)
      {
        return this.Path.nodes[nodeIndex].RoomPosition + this.RoomHandler.area.basePosition.ToVector2() + this.nodeOffset;
      }

      public float GetParametrizedPathPosition()
      {
        float totalPathLength = this.GetTotalPathLength();
        float num1 = 0.0f;
        float num2;
        if ((double) this.PathSpeed >= 0.0)
        {
          int num3 = this.m_currentIndex != 0 ? this.m_currentIndex : this.Path.nodes.Count;
          for (int index = 0; index < num3 - 1; ++index)
            num1 += (this.Path.nodes[index + 1].RoomPosition - this.Path.nodes[index].RoomPosition).magnitude;
          int index1 = (this.m_currentIndex + this.Path.nodes.Count - 1) % this.Path.nodes.Count;
          num2 = num1 + Vector2.Distance(this.Path.nodes[index1].RoomPosition + this.RoomHandler.area.basePosition.ToVector2() + this.nodeOffset, this.specRigidbody.Position.UnitPosition);
        }
        else
        {
          for (int index = 0; index < this.m_currentIndex; ++index)
            num1 += (this.Path.nodes[index + 1].RoomPosition - this.Path.nodes[index].RoomPosition).magnitude;
          num2 = num1 + Vector2.Distance(this.Path.nodes[this.m_currentIndex].RoomPosition + this.RoomHandler.area.basePosition.ToVector2() + this.nodeOffset, this.specRigidbody.Position.UnitPosition);
        }
        return num2 / totalPathLength;
      }

      private void PathTargetReached()
      {
        if (this.ForceCornerDelayHack && (double) Vector2.Distance(this.Path.nodes[this.m_currentIndex].RoomPosition + this.RoomHandler.area.basePosition.ToVector2() + this.nodeOffset, this.specRigidbody.Position.UnitPosition) > 0.10000000149011612)
        {
          this.specRigidbody.Velocity = Vector2.zero;
        }
        else
        {
          SerializedPathNode node = this.Path.nodes[this.m_currentIndex];
          Vector2 vector2_1 = this.Path.nodes[(this.m_currentIndex - 1 + this.Path.nodes.Count) % this.Path.nodes.Count].position.ToVector2();
          Vector2 vector2_2 = this.Path.nodes[this.m_currentIndex].position.ToVector2();
          bool flag1 = this.UpdatePathIndex();
          if (this.OnNodeReached != null)
          {
            bool flag2 = flag1;
            Vector2 vector2_3 = Vector2.zero;
            if (flag2)
              vector2_3 = this.Path.nodes[this.m_currentIndex].position.ToVector2();
            if (this.prevMotionVec == Vector2.zero)
              this.prevMotionVec = vector2_2 - vector2_1;
            Vector2 vector2_4 = vector2_3 - vector2_2;
            this.OnNodeReached(this.prevMotionVec, vector2_4, flag2);
            if (vector2_4 != Vector2.zero)
              this.prevMotionVec = vector2_3 - vector2_2;
          }
          if (!flag1)
          {
            this.specRigidbody.PathMode = false;
            this.specRigidbody.Velocity = Vector2.zero;
          }
          else if ((double) node.delayTime == 0.0 && (double) this.AdditionalNodeDelay == 0.0)
          {
            this.PathToNextNode();
          }
          else
          {
            this.specRigidbody.PathMode = false;
            this.specRigidbody.Velocity = Vector2.zero;
            this.Invoke("PathToNextNode", node.delayTime + this.AdditionalNodeDelay);
          }
        }
      }

      public void WarpToStart()
      {
        this.m_currentIndex = this.PathStartNode;
        this.transform.position = (Vector3) (this.RoomHandler.area.basePosition.ToVector2() + this.Path.nodes[this.PathStartNode].RoomPosition + this.nodeOffset);
        this.specRigidbody.Reinitialize();
        this.PathTargetReached();
      }

      public void WarpToNearestPoint(Vector2 targetPoint)
      {
        Vector2 vector = Vector2.zero;
        float num1 = float.MaxValue;
        int num2 = -1;
        for (int index = 0; index < this.Path.nodes.Count - 1; ++index)
        {
          Vector2 vector2_1 = this.Path.nodes[index].RoomPosition + this.nodeOffset + this.RoomHandler.area.basePosition.ToVector2();
          Vector2 vector2_2 = this.Path.nodes[index + 1].RoomPosition + this.nodeOffset + this.RoomHandler.area.basePosition.ToVector2();
          Vector2 vector2_3 = BraveMathCollege.ClosestPointOnLineSegment(targetPoint, vector2_1, vector2_2);
          float num3 = Vector2.Distance(vector2_3, targetPoint);
          if ((double) num3 < 1.0)
          {
            Vector2 vector2_4 = ((double) Vector2.Distance(vector2_1, vector2_3) >= (double) Vector2.Distance(vector2_2, vector2_3) ? vector2_1 : vector2_2) - vector2_3;
            if ((double) vector2_4.magnitude > 1.0)
              vector2_3 += vector2_4.normalized;
          }
          if ((double) num3 < (double) num1)
          {
            num1 = num3;
            num2 = index + 1;
            vector = vector2_3;
          }
        }
        if (this.Path.wrapMode == SerializedPath.SerializedPathWrapMode.Loop)
        {
          Vector2 vector2_5 = this.Path.nodes[this.Path.nodes.Count - 1].RoomPosition + this.nodeOffset + this.RoomHandler.area.basePosition.ToVector2();
          Vector2 vector2_6 = this.Path.nodes[0].RoomPosition + this.nodeOffset + this.RoomHandler.area.basePosition.ToVector2();
          Vector2 vector2_7 = BraveMathCollege.ClosestPointOnLineSegment(targetPoint, vector2_5, vector2_6);
          float num4 = Vector2.Distance(vector2_7, targetPoint);
          if ((double) num4 < 1.0)
          {
            Vector2 vector2_8 = ((double) Vector2.Distance(vector2_5, vector2_7) >= (double) Vector2.Distance(vector2_6, vector2_7) ? vector2_5 : vector2_6) - vector2_7;
            if ((double) vector2_8.magnitude > 1.0)
              vector2_7 += vector2_8.normalized;
          }
          if ((double) num4 < (double) num1)
          {
            num2 = 0;
            vector = vector2_7;
          }
        }
        this.m_currentIndex = num2;
        this.transform.position = vector.ToVector3ZUp();
        this.specRigidbody.Reinitialize();
        this.PathToNextNode();
      }

      public void ForcePathToNextNode() => this.PathToNextNode();

      protected void HandleDirectionFlip()
      {
        this.m_currentIndexDelta *= -1;
        switch (this.Path.wrapMode)
        {
          case SerializedPath.SerializedPathWrapMode.PingPong:
            if (this.m_currentIndex + this.m_currentIndexDelta < 0 || this.m_currentIndex + this.m_currentIndexDelta >= this.Path.nodes.Count)
              this.m_currentIndexDelta *= -1;
            this.m_currentIndex += this.m_currentIndexDelta;
            break;
          case SerializedPath.SerializedPathWrapMode.Loop:
            this.m_currentIndex = (this.m_currentIndex + this.Path.nodes.Count + this.m_currentIndexDelta) % this.Path.nodes.Count;
            break;
          case SerializedPath.SerializedPathWrapMode.Once:
            this.m_currentIndex = (this.m_currentIndex + this.Path.nodes.Count + this.m_currentIndexDelta) % this.Path.nodes.Count;
            break;
          default:
            this.m_currentIndex = (this.m_currentIndex + this.Path.nodes.Count + this.m_currentIndexDelta) % this.Path.nodes.Count;
            break;
        }
        this.PathToNextNode();
      }

      private void PathToNextNode()
      {
        SerializedPathNode node = this.Path.nodes[this.m_currentIndex];
        this.specRigidbody.PathMode = true;
        this.specRigidbody.PathTarget = PhysicsEngine.UnitToPixel(this.RoomHandler.area.basePosition.ToVector2() + this.nodeOffset + node.RoomPosition);
        this.specRigidbody.PathSpeed = !this.Paused ? Mathf.Abs(this.PathSpeed) : 0.0f;
      }

      public Vector2 GetCurrentTargetPosition() => this.GetPositionOfNode(this.m_currentIndex);

      public Vector2 GetPreviousSourcePosition()
      {
        return this.GetPositionOfNode((this.m_currentIndex + this.m_currentIndexDelta * -2 + this.Path.nodes.Count) % this.Path.nodes.Count);
      }

      public Vector2 GetPreviousTargetPosition()
      {
        return this.GetPositionOfNode((this.m_currentIndex + this.m_currentIndexDelta * -1 + this.Path.nodes.Count) % this.Path.nodes.Count);
      }

      public Vector2 GetNextTargetPosition()
      {
        return this.GetNextTargetRoomPosition() + this.nodeOffset + this.RoomHandler.area.basePosition.ToVector2();
      }

      private Vector2 GetNextTargetRoomPosition()
      {
        SerializedPath path = this.Path;
        int currentIndex = this.m_currentIndex;
        if (this.IsUsingAlternateTargets && this.Path.nodes[this.m_currentIndex].UsesAlternateTarget)
        {
          SerializedPathNode node = this.Path.nodes[this.m_currentIndex];
          return PathMover.GetRoomPath(this.RoomHandler, node.AlternateTargetPathIndex).nodes[node.AlternateTargetNodeIndex].RoomPosition;
        }
        int num;
        if (this.Path.wrapMode == SerializedPath.SerializedPathWrapMode.Once)
        {
          num = currentIndex + this.m_currentIndexDelta;
          if (num >= this.Path.nodes.Count)
          {
            if (this.m_currentIndex >= this.Path.nodes.Count)
              this.m_currentIndex = 0;
            return this.Path.nodes[this.m_currentIndex].RoomPosition;
          }
        }
        else if (this.Path.wrapMode == SerializedPath.SerializedPathWrapMode.Loop)
          num = (currentIndex + this.Path.nodes.Count + this.m_currentIndexDelta) % this.Path.nodes.Count;
        else if (this.Path.wrapMode == SerializedPath.SerializedPathWrapMode.PingPong)
        {
          num = currentIndex + this.m_currentIndexDelta;
          if (num < 0 || num >= this.Path.nodes.Count)
          {
            this.m_currentIndexDelta *= -1;
            num += this.m_currentIndexDelta * 2;
          }
        }
        else
        {
          Debug.LogError((object) ("Unsupported path type " + (object) this.Path.wrapMode));
          return this.Path.nodes[this.m_currentIndex].RoomPosition;
        }
        while (num < 0)
          num += this.Path.nodes.Count;
        return this.Path.nodes[num % this.Path.nodes.Count].RoomPosition;
      }

      private bool UpdatePathIndex()
      {
        if (this.IsUsingAlternateTargets && this.Path.nodes[this.m_currentIndex].UsesAlternateTarget)
        {
          SerializedPathNode node = this.Path.nodes[this.m_currentIndex];
          this.Path = PathMover.GetRoomPath(this.RoomHandler, node.AlternateTargetPathIndex);
          this.m_currentIndex = node.AlternateTargetNodeIndex;
          return true;
        }
        if (this.Path.wrapMode == SerializedPath.SerializedPathWrapMode.Once)
        {
          this.m_currentIndex += this.m_currentIndexDelta;
          if (this.m_currentIndex >= this.Path.nodes.Count || this.m_currentIndex < 0)
            return false;
        }
        else if (this.Path.wrapMode == SerializedPath.SerializedPathWrapMode.Loop)
          this.m_currentIndex = (this.m_currentIndex + this.m_currentIndexDelta + this.Path.nodes.Count) % this.Path.nodes.Count;
        else if (this.Path.wrapMode == SerializedPath.SerializedPathWrapMode.PingPong)
        {
          if (this.m_currentIndex == 0)
          {
            this.m_currentIndex = 1;
            this.m_currentIndexDelta = 1;
          }
          else if (this.m_currentIndex == this.Path.nodes.Count - 1)
          {
            this.m_currentIndex = this.Path.nodes.Count - 2;
            this.m_currentIndexDelta = -1;
          }
          else
          {
            this.m_currentIndex += this.m_currentIndexDelta;
            if (this.m_currentIndex < 0 || this.m_currentIndex >= this.Path.nodes.Count)
            {
              this.m_currentIndexDelta *= -1;
              this.m_currentIndex += this.m_currentIndexDelta * 2;
            }
          }
        }
        else
        {
          Debug.LogError((object) ("Unsupported path type " + (object) this.Path.wrapMode));
          return false;
        }
        return true;
      }
    }

}
