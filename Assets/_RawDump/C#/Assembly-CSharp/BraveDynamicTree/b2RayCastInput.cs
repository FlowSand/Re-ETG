// Decompiled with JetBrains decompiler
// Type: BraveDynamicTree.b2RayCastInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace BraveDynamicTree;

public struct b2RayCastInput(Vector2 p1, Vector2 p2)
{
  public Vector2 p1 = p1;
  public Vector2 p2 = p2;
  public float maxFraction = 1f;
}
