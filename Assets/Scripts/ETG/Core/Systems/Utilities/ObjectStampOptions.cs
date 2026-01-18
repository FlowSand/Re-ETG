using UnityEngine;

#nullable disable

public class ObjectStampOptions : MonoBehaviour
  {
    public Vector2 xPositionRange;
    public Vector2 yPositionRange;

    public Vector3 GetPositionOffset()
    {
      return new Vector3(Random.Range(this.xPositionRange.x, this.xPositionRange.y), Random.Range(this.yPositionRange.x, this.yPositionRange.y));
    }
  }

