// Decompiled with JetBrains decompiler
// Type: LinearCastResult
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class LinearCastResult : CastResult
    {
      public float TimeUsed;
      public bool CollidedX;
      public bool CollidedY;
      public IntVector2 NewPixelsToMove;
      public bool Overlap;
      public static ObjectPool<LinearCastResult> Pool;

      private LinearCastResult()
      {
      }

      public static void Cleanup(LinearCastResult linearCastResults)
      {
        linearCastResults.Contact.x = 0.0f;
        linearCastResults.Contact.y = 0.0f;
        linearCastResults.Normal.x = 0.0f;
        linearCastResults.Normal.y = 0.0f;
        linearCastResults.MyPixelCollider = (PixelCollider) null;
        linearCastResults.OtherPixelCollider = (PixelCollider) null;
        linearCastResults.TimeUsed = 0.0f;
        linearCastResults.CollidedX = false;
        linearCastResults.CollidedY = false;
        linearCastResults.NewPixelsToMove.x = 0;
        linearCastResults.NewPixelsToMove.y = 0;
        linearCastResults.Overlap = false;
      }

      static LinearCastResult()
      {
        ObjectPool<LinearCastResult>.Factory factory = (ObjectPool<LinearCastResult>.Factory) (() => new LinearCastResult());
        // ISSUE: reference to a compiler-generated field
        if (LinearCastResult.<>f__mg_cache0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          LinearCastResult.<>f__mg_cache0 = new ObjectPool<LinearCastResult>.Cleanup(LinearCastResult.Cleanup);
        }
        // ISSUE: reference to a compiler-generated field
        ObjectPool<LinearCastResult>.Cleanup fMgCache0 = LinearCastResult.<>f__mg_cache0;
        LinearCastResult.Pool = new ObjectPool<LinearCastResult>(factory, 10, fMgCache0);
      }
    }

}
