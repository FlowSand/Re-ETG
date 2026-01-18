using UnityEngine;

#nullable disable

  public static class TransformExtensions
  {
    public static Vector2 PositionVector2(this Transform t)
    {
      return new Vector2(t.position.x, t.position.y);
    }

    public static void MovePixelsWorld(this Transform t, IntVector2 offset)
    {
      t.MovePixelsWorld(offset.x, offset.y);
    }

    public static void MovePixelsWorld(this Transform t, int x, int y)
    {
      t.position += new Vector3((float) x * (1f / 16f), (float) y * (1f / 16f), 0.0f);
    }

    public static void MovePixelsWorld(this Transform t, int x, int y, int z)
    {
      t.position += new Vector3((float) x * (1f / 16f), (float) y * (1f / 16f), (float) z * (1f / 16f));
    }

    public static void MovePixelsLocal(this Transform t, IntVector2 offset)
    {
      t.MovePixelsLocal(offset.x, offset.y);
    }

    public static void MovePixelsLocal(this Transform t, int x, int y)
    {
      t.localPosition += new Vector3((float) x * (1f / 16f), (float) y * (1f / 16f), 0.0f);
    }

    public static void MovePixelsLocal(this Transform t, int x, int y, int z)
    {
      t.localPosition += new Vector3((float) x * (1f / 16f), (float) y * (1f / 16f), (float) z * (1f / 16f));
    }

    public static Transform GetFirstLeafChild(this Transform t)
    {
      Transform firstLeafChild = t;
      while (firstLeafChild.childCount > 0)
        firstLeafChild = firstLeafChild.GetChild(0);
      return firstLeafChild;
    }
  }

