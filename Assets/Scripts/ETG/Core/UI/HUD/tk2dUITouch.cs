// Decompiled with JetBrains decompiler
// Type: tk2dUITouch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct tk2dUITouch
    {
      public const int MOUSE_POINTER_FINGER_ID = 9999;

      public tk2dUITouch(
        TouchPhase _phase,
        int _fingerId,
        Vector2 _position,
        Vector2 _deltaPosition,
        float _deltaTime)
        : this()
      {
        this.phase = _phase;
        this.fingerId = _fingerId;
        this.position = _position;
        this.deltaPosition = _deltaPosition;
        this.deltaTime = _deltaTime;
      }

      public tk2dUITouch(Touch touch)
        : this()
      {
        this.phase = touch.phase;
        this.fingerId = touch.fingerId;
        this.position = touch.position;
        this.deltaPosition = this.deltaPosition;
        this.deltaTime = this.deltaTime;
      }

      public TouchPhase phase { get; private set; }

      public int fingerId { get; private set; }

      public Vector2 position { get; private set; }

      public Vector2 deltaPosition { get; private set; }

      public float deltaTime { get; private set; }

      public override string ToString()
      {
        return $"{this.phase.ToString()},{(object) this.fingerId},{(object) this.position},{(object) this.deltaPosition},{(object) this.deltaTime}";
      }
    }

}
