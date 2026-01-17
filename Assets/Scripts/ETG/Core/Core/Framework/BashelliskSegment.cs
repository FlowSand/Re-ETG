// Decompiled with JetBrains decompiler
// Type: BashelliskSegment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public abstract class BashelliskSegment : BraveBehaviour
    {
      public Transform center;
      public float attachRadius;
      [NonSerialized]
      public BashelliskSegment next;
      [NonSerialized]
      public BashelliskSegment previous;
      [NonSerialized]
      public float PathDist;

      public virtual void UpdatePosition(
        PooledLinkedList<Vector2> path,
        LinkedListNode<Vector2> pathNode,
        float totalPathDist,
        float thisNodeDist)
      {
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
