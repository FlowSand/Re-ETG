// Decompiled with JetBrains decompiler
// Type: BraveDynamicTree.b2TreeNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace BraveDynamicTree
{
  public class b2TreeNode
  {
    public b2AABB fatAabb;
    public b2AABB tightAabb;
    public SpeculativeRigidbody rigidbody;
    public int parent;
    public int next;
    public int child1;
    public int child2;
    public int height;

    public bool IsLeaf() => this.child1 == -1;
  }
}
