// Decompiled with JetBrains decompiler
// Type: PathBlocker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class PathBlocker : BraveBehaviour
    {
      public bool BlocksGoopsToo;

      public static void BlockRigidbody(SpeculativeRigidbody rigidbody, bool blockGoopsToo)
      {
        foreach (PixelCollider pixelCollider in rigidbody.PixelColliders)
        {
          if (!pixelCollider.IsTrigger && (pixelCollider.CollisionLayer == CollisionLayer.LowObstacle || pixelCollider.CollisionLayer == CollisionLayer.HighObstacle || pixelCollider.CollisionLayer == CollisionLayer.EnemyBlocker))
          {
            if (pixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.Line)
            {
              Vector2 a = rigidbody.transform.position.XY() + PhysicsEngine.PixelToUnit(new IntVector2(pixelCollider.ManualLeftX, pixelCollider.ManualLeftY));
              Vector2 vector2 = rigidbody.transform.position.XY() + PhysicsEngine.PixelToUnit(new IntVector2(pixelCollider.ManualRightX, pixelCollider.ManualRightY));
              float num1 = Vector2.Distance(a, vector2);
              Vector2 normalized = (vector2 - a).normalized;
              for (float num2 = 0.0f; (double) num2 <= (double) num1; num2 += 0.1f)
              {
                IntVector2 intVector2 = (a + normalized * num2).ToIntVector2(VectorConversions.Floor);
                GameManager.Instance.Dungeon.data[intVector2].isOccupied = true;
                if (blockGoopsToo)
                  GameManager.Instance.Dungeon.data[intVector2].forceDisallowGoop = true;
              }
              GameManager.Instance.Dungeon.data[vector2.ToIntVector2(VectorConversions.Floor)].isOccupied = true;
              if (blockGoopsToo)
                GameManager.Instance.Dungeon.data[vector2.ToIntVector2(VectorConversions.Floor)].forceDisallowGoop = true;
            }
            else
            {
              IntVector2 intVector2_1 = pixelCollider.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
              IntVector2 intVector2_2 = pixelCollider.UnitTopRight.ToIntVector2(VectorConversions.Ceil);
              for (int x = intVector2_1.x; x < intVector2_2.x; ++x)
              {
                for (int y = intVector2_1.y; y < intVector2_2.y; ++y)
                {
                  if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(new IntVector2(x, y)))
                  {
                    GameManager.Instance.Dungeon.data[x, y].isOccupied = true;
                    if (blockGoopsToo)
                      GameManager.Instance.Dungeon.data[x, y].forceDisallowGoop = true;
                  }
                }
              }
            }
          }
        }
      }

      public void Start()
      {
        if (!(bool) (Object) this.specRigidbody)
          return;
        this.specRigidbody.Initialize();
        PathBlocker.BlockRigidbody(this.specRigidbody, this.BlocksGoopsToo);
      }
    }

}
