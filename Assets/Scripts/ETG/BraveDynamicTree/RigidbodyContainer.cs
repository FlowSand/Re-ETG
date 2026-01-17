// Decompiled with JetBrains decompiler
// Type: BraveDynamicTree.RigidbodyContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace BraveDynamicTree
{
  public interface RigidbodyContainer
  {
    void RayCast(
      b2RayCastInput input,
      Func<b2RayCastInput, SpeculativeRigidbody, float> callback);

    void Query(b2AABB aabb, Func<SpeculativeRigidbody, bool> callback);

    int CreateProxy(b2AABB aabb, SpeculativeRigidbody rigidbody);

    bool MoveProxy(int proxyId, b2AABB aabb, Vector2 displacement);

    void DestroyProxy(int proxyId);
  }
}
