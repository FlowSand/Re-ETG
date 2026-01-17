// Decompiled with JetBrains decompiler
// Type: PointcastResult
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class PointcastResult : IComparable<PointcastResult>
    {
      public RaycastResult hitResult;
      public int pointIndex;
      public int boneIndex;
      public HitDirection hitDirection;
      public static ObjectPool<PointcastResult> Pool;

      private PointcastResult()
      {
      }

      public void SetAll(
        HitDirection hitDirection,
        int pointIndex,
        int boneIndex,
        RaycastResult hitResult)
      {
        this.hitDirection = hitDirection;
        this.pointIndex = pointIndex;
        this.boneIndex = boneIndex;
        this.hitResult = hitResult;
      }

      public static void Cleanup(PointcastResult pointcastResult)
      {
        pointcastResult.hitDirection = HitDirection.Unknown;
        pointcastResult.pointIndex = 0;
        pointcastResult.boneIndex = 0;
        RaycastResult.Pool.Free(ref pointcastResult.hitResult);
      }

      public int CompareTo(PointcastResult other)
      {
        int num = this.boneIndex - other.boneIndex;
        return num != 0 ? num : this.pointIndex - other.pointIndex;
      }

      static PointcastResult()
      {
        ObjectPool<PointcastResult>.Factory factory = (ObjectPool<PointcastResult>.Factory) (() => new PointcastResult());
        // ISSUE: reference to a compiler-generated field
        if (PointcastResult.<>f__mg$cache0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PointcastResult.<>f__mg$cache0 = new ObjectPool<PointcastResult>.Cleanup(PointcastResult.Cleanup);
        }
        // ISSUE: reference to a compiler-generated field
        ObjectPool<PointcastResult>.Cleanup fMgCache0 = PointcastResult.<>f__mg$cache0;
        PointcastResult.Pool = new ObjectPool<PointcastResult>(factory, 10, fMgCache0);
      }
    }

}
