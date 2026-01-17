// Decompiled with JetBrains decompiler
// Type: BraveDynamicTree.b2DynamicTree
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace BraveDynamicTree
{
  public class b2DynamicTree : RigidbodyContainer
  {
    private int m_root;
    private b2TreeNode[] m_nodes;
    private int m_nodeCount;
    private int m_nodeCapacity;
    private int m_freeList;
    private Stack<int> m_stack = new Stack<int>(256 /*0x0100*/);
    public const int b2_nullNode = -1;
    private const float b2_aabbExtension = 0.1f;
    private const float b2_aabbMultiplier = 2f;

    public b2DynamicTree()
    {
      this.m_root = -1;
      this.m_nodeCapacity = 16 /*0x10*/;
      this.m_nodeCount = 0;
      this.m_nodes = new b2TreeNode[this.m_nodeCapacity];
      for (int index = 0; index < this.m_nodeCapacity - 1; ++index)
      {
        this.m_nodes[index] = new b2TreeNode();
        this.m_nodes[index].next = index + 1;
        this.m_nodes[index].height = -1;
      }
      this.m_nodes[this.m_nodeCapacity - 1] = new b2TreeNode();
      this.m_nodes[this.m_nodeCapacity - 1].next = -1;
      this.m_nodes[this.m_nodeCapacity - 1].height = -1;
      this.m_freeList = 0;
    }

    [Conditional("BRAVE_INTERNAL")]
    public static void b2Assert(bool mustBeTrue)
    {
      if (!mustBeTrue)
        throw new Exception("Failed assert in b2DynamicTree!");
    }

    public int CreateProxy(b2AABB aabb, SpeculativeRigidbody rigidbody)
    {
      int leaf = this.AllocateNode();
      Vector2 vector2 = new Vector2(0.1f, 0.1f);
      this.m_nodes[leaf].fatAabb.lowerBound = aabb.lowerBound - vector2;
      this.m_nodes[leaf].fatAabb.upperBound = aabb.upperBound + vector2;
      this.m_nodes[leaf].tightAabb = this.m_nodes[leaf].fatAabb;
      this.m_nodes[leaf].rigidbody = rigidbody;
      this.m_nodes[leaf].height = 0;
      this.InsertLeaf(leaf);
      return leaf;
    }

    public void DestroyProxy(int proxyId)
    {
      this.RemoveLeaf(proxyId);
      this.FreeNode(proxyId);
    }

    public bool MoveProxy(int proxyId, b2AABB aabb, Vector2 displacement)
    {
      float lowX = aabb.lowerBound.x - 0.1f;
      float lowY = aabb.lowerBound.y - 0.1f;
      float upperX = aabb.upperBound.x + 0.1f;
      float upperY = aabb.upperBound.y + 0.1f;
      this.m_nodes[proxyId].tightAabb = new b2AABB(lowX, lowY, upperX, upperY);
      if (this.m_nodes[proxyId].fatAabb.Contains(aabb))
        return false;
      this.RemoveLeaf(proxyId);
      Vector2 vector2 = 2f * displacement;
      if ((double) vector2.x < 0.0)
        lowX += vector2.x;
      else
        upperX += vector2.x;
      if ((double) vector2.y < 0.0)
        lowY += vector2.y;
      else
        upperY += vector2.y;
      this.m_nodes[proxyId].fatAabb = new b2AABB(lowX, lowY, upperX, upperY);
      this.InsertLeaf(proxyId);
      return true;
    }

    public SpeculativeRigidbody GetSpeculativeRigidbody(int proxyId)
    {
      return this.m_nodes[proxyId].rigidbody;
    }

    public b2AABB GetFatAABB(int proxyId) => this.m_nodes[proxyId].fatAabb;

    public void Query(b2AABB aabb, Func<SpeculativeRigidbody, bool> callback)
    {
      this.m_stack.Clear();
      this.m_stack.Push(this.m_root);
      while (this.m_stack.Count > 0)
      {
        int index = this.m_stack.Pop();
        if (index != -1)
        {
          b2TreeNode node = this.m_nodes[index];
          b2AABB fatAabb = node.fatAabb;
          if ((double) aabb.lowerBound.x <= (double) fatAabb.upperBound.x && (double) fatAabb.lowerBound.x <= (double) aabb.upperBound.x && (double) aabb.lowerBound.y <= (double) fatAabb.upperBound.y && (double) fatAabb.lowerBound.y <= (double) aabb.upperBound.y)
          {
            if (node.child1 == -1)
            {
              b2AABB tightAabb = node.tightAabb;
              if ((double) aabb.lowerBound.x <= (double) tightAabb.upperBound.x && (double) tightAabb.lowerBound.x <= (double) aabb.upperBound.x && (double) aabb.lowerBound.y <= (double) tightAabb.upperBound.y && (double) tightAabb.lowerBound.y <= (double) aabb.upperBound.y && !callback(node.rigidbody))
                break;
            }
            else
            {
              this.m_stack.Push(node.child1);
              this.m_stack.Push(node.child2);
            }
          }
        }
      }
    }

    public void RayCast(
      b2RayCastInput input,
      Func<b2RayCastInput, SpeculativeRigidbody, float> callback)
    {
      Vector2 p1 = input.p1;
      Vector2 p2 = input.p2;
      Vector2 a = p2 - p1;
      if ((double) a.sqrMagnitude <= 0.0)
        return;
      a.Normalize();
      Vector2 vector2 = Vector2Extensions.Cross(1f, a);
      Vector2 lhs = vector2.Abs();
      float num1 = input.maxFraction;
      Vector2 rhs1 = p1 + num1 * (p2 - p1);
      b2AABB b;
      b.lowerBound = Vector2.Min(p1, rhs1);
      b.upperBound = Vector2.Max(p1, rhs1);
      this.m_stack.Clear();
      this.m_stack.Push(this.m_root);
      while (this.m_stack.Count > 0)
      {
        int index = this.m_stack.Pop();
        if (index != -1)
        {
          b2TreeNode node = this.m_nodes[index];
          if (b2AABB.b2TestOverlap(ref node.fatAabb, ref b))
          {
            Vector2 center = node.fatAabb.GetCenter();
            Vector2 extents = node.fatAabb.GetExtents();
            if ((double) (Mathf.Abs(Vector2.Dot(vector2, p1 - center)) - Vector2.Dot(lhs, extents)) <= 0.0)
            {
              if (node.IsLeaf())
              {
                b2RayCastInput b2RayCastInput;
                b2RayCastInput.p1 = input.p1;
                b2RayCastInput.p2 = input.p2;
                b2RayCastInput.maxFraction = num1;
                float num2 = callback(b2RayCastInput, node.rigidbody);
                if ((double) num2 == 0.0)
                  break;
                if ((double) num2 > 0.0)
                {
                  num1 = num2;
                  Vector2 rhs2 = p1 + num1 * (p2 - p1);
                  b.lowerBound = Vector2.Min(p1, rhs2);
                  b.upperBound = Vector2.Max(p1, rhs2);
                }
              }
              else
              {
                this.m_stack.Push(node.child1);
                this.m_stack.Push(node.child2);
              }
            }
          }
        }
      }
    }

    public void Validate()
    {
      this.ValidateStructure(this.m_root);
      this.ValidateMetrics(this.m_root);
      int num = 0;
      int index = this.m_freeList;
      while (index != -1)
      {
        index = this.m_nodes[index].next;
        ++num;
      }
    }

    public int GetHeight() => this.m_root == -1 ? 0 : this.m_nodes[this.m_root].height;

    public int GetMaxBalance()
    {
      int a = 0;
      for (int index = 0; index < this.m_nodeCapacity; ++index)
      {
        b2TreeNode node = this.m_nodes[index];
        if (node.height > 1)
        {
          int child1 = node.child1;
          int b = Mathf.Abs(this.m_nodes[node.child2].height - this.m_nodes[child1].height);
          a = Mathf.Max(a, b);
        }
      }
      return a;
    }

    public float GetAreaRatio()
    {
      if (this.m_root == -1)
        return 0.0f;
      float perimeter = this.m_nodes[this.m_root].fatAabb.GetPerimeter();
      float num = 0.0f;
      for (int index = 0; index < this.m_nodeCapacity; ++index)
      {
        b2TreeNode node = this.m_nodes[index];
        if (node.height >= 0)
          num += node.fatAabb.GetPerimeter();
      }
      return num / perimeter;
    }

    public void RebuildBottomUp()
    {
      int[] numArray = new int[this.m_nodeCount];
      int index1 = 0;
      for (int nodeId = 0; nodeId < this.m_nodeCapacity; ++nodeId)
      {
        if (this.m_nodes[nodeId].height >= 0)
        {
          if (this.m_nodes[nodeId].IsLeaf())
          {
            this.m_nodes[nodeId].parent = -1;
            numArray[index1] = nodeId;
            ++index1;
          }
          else
            this.FreeNode(nodeId);
        }
      }
      for (; index1 > 1; --index1)
      {
        float num = float.MaxValue;
        int index2 = -1;
        int index3 = -1;
        for (int index4 = 0; index4 < index1; ++index4)
        {
          b2AABB fatAabb1 = this.m_nodes[numArray[index4]].fatAabb;
          for (int index5 = index4 + 1; index5 < index1; ++index5)
          {
            b2AABB fatAabb2 = this.m_nodes[numArray[index5]].fatAabb;
            b2AABB b2Aabb = new b2AABB();
            b2Aabb.Combine(fatAabb1, fatAabb2);
            float perimeter = b2Aabb.GetPerimeter();
            if ((double) perimeter < (double) num)
            {
              index2 = index4;
              index3 = index5;
              num = perimeter;
            }
          }
        }
        int index6 = numArray[index2];
        int index7 = numArray[index3];
        b2TreeNode node1 = this.m_nodes[index6];
        b2TreeNode node2 = this.m_nodes[index7];
        int index8 = this.AllocateNode();
        b2TreeNode node3 = this.m_nodes[index8];
        node3.child1 = index6;
        node3.child2 = index7;
        node3.height = 1 + Mathf.Max(node1.height, node2.height);
        node3.fatAabb.Combine(node1.fatAabb, node2.fatAabb);
        node3.parent = -1;
        node1.parent = index8;
        node2.parent = index8;
        numArray[index3] = numArray[index1 - 1];
        numArray[index2] = index8;
      }
      this.m_root = numArray[0];
      this.Validate();
    }

    public void ShiftOrigin(Vector2 newOrigin)
    {
      for (int index = 0; index < this.m_nodeCapacity; ++index)
      {
        this.m_nodes[index].fatAabb.lowerBound -= newOrigin;
        this.m_nodes[index].fatAabb.upperBound -= newOrigin;
      }
    }

    private int AllocateNode()
    {
      if (this.m_freeList == -1)
      {
        this.m_nodeCapacity *= 2;
        Array.Resize<b2TreeNode>(ref this.m_nodes, this.m_nodeCapacity);
        for (int nodeCount = this.m_nodeCount; nodeCount < this.m_nodeCapacity - 1; ++nodeCount)
        {
          this.m_nodes[nodeCount] = new b2TreeNode();
          this.m_nodes[nodeCount].next = nodeCount + 1;
          this.m_nodes[nodeCount].height = -1;
        }
        this.m_nodes[this.m_nodeCapacity - 1] = new b2TreeNode();
        this.m_nodes[this.m_nodeCapacity - 1].next = -1;
        this.m_nodes[this.m_nodeCapacity - 1].height = -1;
        this.m_freeList = this.m_nodeCount;
      }
      int freeList = this.m_freeList;
      this.m_freeList = this.m_nodes[freeList].next;
      this.m_nodes[freeList].parent = -1;
      this.m_nodes[freeList].child1 = -1;
      this.m_nodes[freeList].child2 = -1;
      this.m_nodes[freeList].height = 0;
      this.m_nodes[freeList].rigidbody = (SpeculativeRigidbody) null;
      ++this.m_nodeCount;
      return freeList;
    }

    private void FreeNode(int nodeId)
    {
      this.m_nodes[nodeId].next = this.m_freeList;
      this.m_nodes[nodeId].height = -1;
      this.m_freeList = nodeId;
      --this.m_nodeCount;
    }

    private void InsertLeaf(int leaf)
    {
      if (this.m_root == -1)
      {
        this.m_root = leaf;
        this.m_nodes[this.m_root].parent = -1;
      }
      else
      {
        b2AABB fatAabb = this.m_nodes[leaf].fatAabb;
        int index1;
        int child1_1;
        int child2_1;
        float num1;
        float num2;
        for (index1 = this.m_root; !this.m_nodes[index1].IsLeaf(); index1 = (double) num1 >= (double) num2 ? child2_1 : child1_1)
        {
          child1_1 = this.m_nodes[index1].child1;
          child2_1 = this.m_nodes[index1].child2;
          float perimeter1 = this.m_nodes[index1].fatAabb.GetPerimeter();
          b2AABB b2Aabb1 = new b2AABB();
          b2Aabb1.Combine(this.m_nodes[index1].fatAabb, fatAabb);
          float perimeter2 = b2Aabb1.GetPerimeter();
          float num3 = 2f * perimeter2;
          float num4 = (float) (2.0 * ((double) perimeter2 - (double) perimeter1));
          if (this.m_nodes[child1_1].IsLeaf())
          {
            b2AABB b2Aabb2 = new b2AABB();
            b2Aabb2.Combine(fatAabb, this.m_nodes[child1_1].fatAabb);
            num1 = b2Aabb2.GetPerimeter() + num4;
          }
          else
          {
            b2AABB b2Aabb3 = new b2AABB();
            b2Aabb3.Combine(fatAabb, this.m_nodes[child1_1].fatAabb);
            float perimeter3 = this.m_nodes[child1_1].fatAabb.GetPerimeter();
            num1 = b2Aabb3.GetPerimeter() - perimeter3 + num4;
          }
          if (this.m_nodes[child2_1].IsLeaf())
          {
            b2AABB b2Aabb4 = new b2AABB();
            b2Aabb4.Combine(fatAabb, this.m_nodes[child2_1].fatAabb);
            num2 = b2Aabb4.GetPerimeter() + num4;
          }
          else
          {
            b2AABB b2Aabb5 = new b2AABB();
            b2Aabb5.Combine(fatAabb, this.m_nodes[child2_1].fatAabb);
            float perimeter4 = this.m_nodes[child2_1].fatAabb.GetPerimeter();
            num2 = b2Aabb5.GetPerimeter() - perimeter4 + num4;
          }
          if ((double) num3 < (double) num1 && (double) num3 < (double) num2)
            break;
        }
        int index2 = index1;
        int parent1 = this.m_nodes[index2].parent;
        int index3 = this.AllocateNode();
        this.m_nodes[index3].parent = parent1;
        this.m_nodes[index3].rigidbody = (SpeculativeRigidbody) null;
        this.m_nodes[index3].fatAabb.Combine(fatAabb, this.m_nodes[index2].fatAabb);
        this.m_nodes[index3].height = this.m_nodes[index2].height + 1;
        if (parent1 != -1)
        {
          if (this.m_nodes[parent1].child1 == index2)
            this.m_nodes[parent1].child1 = index3;
          else
            this.m_nodes[parent1].child2 = index3;
          this.m_nodes[index3].child1 = index2;
          this.m_nodes[index3].child2 = leaf;
          this.m_nodes[index2].parent = index3;
          this.m_nodes[leaf].parent = index3;
        }
        else
        {
          this.m_nodes[index3].child1 = index2;
          this.m_nodes[index3].child2 = leaf;
          this.m_nodes[index2].parent = index3;
          this.m_nodes[leaf].parent = index3;
          this.m_root = index3;
        }
        int index4;
        for (int parent2 = this.m_nodes[leaf].parent; parent2 != -1; parent2 = this.m_nodes[index4].parent)
        {
          index4 = this.Balance(parent2);
          int child1_2 = this.m_nodes[index4].child1;
          int child2_2 = this.m_nodes[index4].child2;
          this.m_nodes[index4].height = 1 + Mathf.Max(this.m_nodes[child1_2].height, this.m_nodes[child2_2].height);
          this.m_nodes[index4].fatAabb.Combine(this.m_nodes[child1_2].fatAabb, this.m_nodes[child2_2].fatAabb);
        }
      }
    }

    private void RemoveLeaf(int leaf)
    {
      if (leaf == this.m_root)
      {
        this.m_root = -1;
      }
      else
      {
        int parent1 = this.m_nodes[leaf].parent;
        int parent2 = this.m_nodes[parent1].parent;
        int index1 = this.m_nodes[parent1].child1 != leaf ? this.m_nodes[parent1].child1 : this.m_nodes[parent1].child2;
        if (parent2 != -1)
        {
          if (this.m_nodes[parent2].child1 == parent1)
            this.m_nodes[parent2].child1 = index1;
          else
            this.m_nodes[parent2].child2 = index1;
          this.m_nodes[index1].parent = parent2;
          this.FreeNode(parent1);
          int index2;
          for (int iA = parent2; iA != -1; iA = this.m_nodes[index2].parent)
          {
            index2 = this.Balance(iA);
            int child1 = this.m_nodes[index2].child1;
            int child2 = this.m_nodes[index2].child2;
            this.m_nodes[index2].fatAabb.Combine(this.m_nodes[child1].fatAabb, this.m_nodes[child2].fatAabb);
            this.m_nodes[index2].height = 1 + Mathf.Max(this.m_nodes[child1].height, this.m_nodes[child2].height);
          }
        }
        else
        {
          this.m_root = index1;
          this.m_nodes[index1].parent = -1;
          this.FreeNode(parent1);
        }
      }
    }

    private int Balance(int iA)
    {
      b2TreeNode node1 = this.m_nodes[iA];
      if (node1.IsLeaf() || node1.height < 2)
        return iA;
      int child1_1 = node1.child1;
      int child2_1 = node1.child2;
      b2TreeNode node2 = this.m_nodes[child1_1];
      b2TreeNode node3 = this.m_nodes[child2_1];
      int num = node3.height - node2.height;
      if (num > 1)
      {
        int child1_2 = node3.child1;
        int child2_2 = node3.child2;
        b2TreeNode node4 = this.m_nodes[child1_2];
        b2TreeNode node5 = this.m_nodes[child2_2];
        node3.child1 = iA;
        node3.parent = node1.parent;
        node1.parent = child2_1;
        if (node3.parent != -1)
        {
          if (this.m_nodes[node3.parent].child1 == iA)
            this.m_nodes[node3.parent].child1 = child2_1;
          else
            this.m_nodes[node3.parent].child2 = child2_1;
        }
        else
          this.m_root = child2_1;
        if (node4.height > node5.height)
        {
          node3.child2 = child1_2;
          node1.child2 = child2_2;
          node5.parent = iA;
          node1.fatAabb.Combine(node2.fatAabb, node5.fatAabb);
          node3.fatAabb.Combine(node1.fatAabb, node4.fatAabb);
          node1.height = 1 + Mathf.Max(node2.height, node5.height);
          node3.height = 1 + Mathf.Max(node1.height, node4.height);
        }
        else
        {
          node3.child2 = child2_2;
          node1.child2 = child1_2;
          node4.parent = iA;
          node1.fatAabb.Combine(node2.fatAabb, node4.fatAabb);
          node3.fatAabb.Combine(node1.fatAabb, node5.fatAabb);
          node1.height = 1 + Mathf.Max(node2.height, node4.height);
          node3.height = 1 + Mathf.Max(node1.height, node5.height);
        }
        return child2_1;
      }
      if (num >= -1)
        return iA;
      int child1_3 = node2.child1;
      int child2_3 = node2.child2;
      b2TreeNode node6 = this.m_nodes[child1_3];
      b2TreeNode node7 = this.m_nodes[child2_3];
      node2.child1 = iA;
      node2.parent = node1.parent;
      node1.parent = child1_1;
      if (node2.parent != -1)
      {
        if (this.m_nodes[node2.parent].child1 == iA)
          this.m_nodes[node2.parent].child1 = child1_1;
        else
          this.m_nodes[node2.parent].child2 = child1_1;
      }
      else
        this.m_root = child1_1;
      if (node6.height > node7.height)
      {
        node2.child2 = child1_3;
        node1.child1 = child2_3;
        node7.parent = iA;
        node1.fatAabb.Combine(node3.fatAabb, node7.fatAabb);
        node2.fatAabb.Combine(node1.fatAabb, node6.fatAabb);
        node1.height = 1 + Mathf.Max(node3.height, node7.height);
        node2.height = 1 + Mathf.Max(node1.height, node6.height);
      }
      else
      {
        node2.child2 = child2_3;
        node1.child1 = child1_3;
        node6.parent = iA;
        node1.fatAabb.Combine(node3.fatAabb, node6.fatAabb);
        node2.fatAabb.Combine(node1.fatAabb, node7.fatAabb);
        node1.height = 1 + Mathf.Max(node3.height, node6.height);
        node2.height = 1 + Mathf.Max(node1.height, node7.height);
      }
      return child1_1;
    }

    private int ComputeHeight(int nodeId)
    {
      b2TreeNode node = this.m_nodes[nodeId];
      return node.IsLeaf() ? 0 : 1 + Mathf.Max(this.ComputeHeight(node.child1), this.ComputeHeight(node.child2));
    }

    private int ComputeHeight() => this.ComputeHeight(this.m_root);

    private void ValidateStructure(int index)
    {
      if (index == -1)
        return;
      b2TreeNode node = this.m_nodes[index];
      int child1 = node.child1;
      int child2 = node.child2;
      if (node.IsLeaf())
        return;
      this.ValidateStructure(child1);
      this.ValidateStructure(child2);
    }

    private void ValidateMetrics(int index)
    {
      if (index == -1)
        return;
      b2TreeNode node = this.m_nodes[index];
      int child1 = node.child1;
      int child2 = node.child2;
      if (node.IsLeaf())
        return;
      int num = 1 + Mathf.Max(this.m_nodes[child1].height, this.m_nodes[child2].height);
      new b2AABB().Combine(this.m_nodes[child1].fatAabb, this.m_nodes[child2].fatAabb);
      this.ValidateMetrics(child1);
      this.ValidateMetrics(child2);
    }
  }
}
