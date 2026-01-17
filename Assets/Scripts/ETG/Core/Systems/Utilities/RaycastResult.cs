// Decompiled with JetBrains decompiler
// Type: RaycastResult
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class RaycastResult : CastResult
    {
      public IntVector2 HitPixel;
      public IntVector2 LastRayPixel;
      public float Distance;
      public SpeculativeRigidbody SpeculativeRigidbody;
      public static ObjectPool<RaycastResult> Pool;

      private RaycastResult()
      {
      }

      public static void Cleanup(RaycastResult raycastResult)
      {
        raycastResult.Contact.x = 0.0f;
        raycastResult.Contact.y = 0.0f;
        raycastResult.Normal.x = 0.0f;
        raycastResult.Normal.y = 0.0f;
        raycastResult.MyPixelCollider = (PixelCollider) null;
        raycastResult.OtherPixelCollider = (PixelCollider) null;
        raycastResult.HitPixel.x = 0;
        raycastResult.HitPixel.y = 0;
        raycastResult.LastRayPixel.x = 0;
        raycastResult.LastRayPixel.y = 0;
        raycastResult.Distance = 0.0f;
        raycastResult.SpeculativeRigidbody = (SpeculativeRigidbody) null;
      }

      static RaycastResult()
      {
        ObjectPool<RaycastResult>.Factory factory = (ObjectPool<RaycastResult>.Factory) (() => new RaycastResult());
        // ISSUE: reference to a compiler-generated field
        if (RaycastResult._f__mg_cache0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          RaycastResult._f__mg_cache0 = new ObjectPool<RaycastResult>.Cleanup(RaycastResult.Cleanup);
        }
        // ISSUE: reference to a compiler-generated field
        ObjectPool<RaycastResult>.Cleanup fMgCache0 = RaycastResult._f__mg_cache0;
        RaycastResult.Pool = new ObjectPool<RaycastResult>(factory, 10, fMgCache0);
      }
    }

}
