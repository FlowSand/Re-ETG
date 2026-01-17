// Decompiled with JetBrains decompiler
// Type: ObjectStampOptions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class ObjectStampOptions : MonoBehaviour
    {
      public Vector2 xPositionRange;
      public Vector2 yPositionRange;

      public Vector3 GetPositionOffset()
      {
        return new Vector3(Random.Range(this.xPositionRange.x, this.xPositionRange.y), Random.Range(this.yPositionRange.x, this.yPositionRange.y));
      }
    }

}
