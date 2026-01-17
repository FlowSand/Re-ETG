// Decompiled with JetBrains decompiler
// Type: SerializedPathNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public struct SerializedPathNode
    {
      public IntVector2 position;
      public float delayTime;
      public SerializedPathNode.SerializedNodePlacement placement;
      public bool UsesAlternateTarget;
      public int AlternateTargetPathIndex;
      public int AlternateTargetNodeIndex;

      public SerializedPathNode(IntVector2 pos)
      {
        this.position = pos;
        this.placement = SerializedPathNode.SerializedNodePlacement.SouthWest;
        this.delayTime = 0.0f;
        this.UsesAlternateTarget = false;
        this.AlternateTargetNodeIndex = -1;
        this.AlternateTargetPathIndex = -1;
      }

      public SerializedPathNode(SerializedPathNode sourceNode, IntVector2 positionAdjustment)
      {
        this.position = sourceNode.position + positionAdjustment;
        this.placement = sourceNode.placement;
        this.delayTime = sourceNode.delayTime;
        this.UsesAlternateTarget = sourceNode.UsesAlternateTarget;
        this.AlternateTargetNodeIndex = sourceNode.AlternateTargetNodeIndex;
        this.AlternateTargetPathIndex = sourceNode.AlternateTargetPathIndex;
      }

      public static SerializedPathNode CreateMirror(
        SerializedPathNode source,
        IntVector2 roomDimensions)
      {
        SerializedPathNode mirror = new SerializedPathNode()
        {
          position = source.position
        };
        mirror.position.x = roomDimensions.x - (mirror.position.x + 1);
        mirror.delayTime = source.delayTime;
        mirror.placement = source.placement;
        mirror.UsesAlternateTarget = source.UsesAlternateTarget;
        mirror.AlternateTargetPathIndex = source.AlternateTargetPathIndex;
        mirror.AlternateTargetNodeIndex = source.AlternateTargetNodeIndex;
        return mirror;
      }

      public Vector2 RoomPosition
      {
        get
        {
          IntVector2 vectorFromPlacement = this.GetNormalizedVectorFromPlacement();
          return this.position.ToCenterVector2() + new Vector2(0.5f * (float) vectorFromPlacement.x, 0.5f * (float) vectorFromPlacement.y);
        }
      }

      public IntVector2 GetNormalizedVectorFromPlacement()
      {
        switch (this.placement)
        {
          case SerializedPathNode.SerializedNodePlacement.Center:
            return IntVector2.Zero;
          case SerializedPathNode.SerializedNodePlacement.North:
            return IntVector2.North;
          case SerializedPathNode.SerializedNodePlacement.NorthEast:
            return IntVector2.NorthEast;
          case SerializedPathNode.SerializedNodePlacement.East:
            return IntVector2.East;
          case SerializedPathNode.SerializedNodePlacement.SouthEast:
            return IntVector2.SouthEast;
          case SerializedPathNode.SerializedNodePlacement.South:
            return IntVector2.South;
          case SerializedPathNode.SerializedNodePlacement.SouthWest:
            return IntVector2.SouthWest;
          case SerializedPathNode.SerializedNodePlacement.West:
            return IntVector2.West;
          case SerializedPathNode.SerializedNodePlacement.NorthWest:
            return IntVector2.NorthWest;
          default:
            return IntVector2.Zero;
        }
      }

      public enum SerializedNodePlacement
      {
        Center,
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
      }
    }

}
