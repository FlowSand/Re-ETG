// Decompiled with JetBrains decompiler
// Type: CutsceneMotion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class CutsceneMotion
    {
      public Transform transform;
      public CameraController camera;
      public Vector2 lerpStart;
      public Vector2? lerpEnd;
      public float lerpProgress;
      public float speed;
      public float zOffset;
      public bool isSmoothStepped = true;

      public CutsceneMotion(Transform t, Vector2? targetPosition, float s, float z = 0.0f)
      {
        this.transform = t;
        this.lerpStart = t.position.XY();
        this.lerpEnd = targetPosition;
        this.lerpProgress = 0.0f;
        this.speed = s;
        this.zOffset = z;
      }
    }

}
