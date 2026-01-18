using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable

public class dfTouchEventArgs : dfMouseEventArgs
  {
    public dfTouchEventArgs(dfControl Source, dfTouchInfo touch, Ray ray)
      : base(Source, dfMouseButtons.Left, touch.tapCount, ray, touch.position, 0.0f)
    {
      this.Touch = touch;
      this.Touches = new List<dfTouchInfo>() { touch };
      float deltaTime = BraveTime.DeltaTime;
      if ((double) touch.deltaTime > 1.4012984643248171E-45 && (double) deltaTime > 1.4012984643248171E-45)
        this.MoveDelta = touch.deltaPosition * (deltaTime / touch.deltaTime);
      else
        this.MoveDelta = touch.deltaPosition;
    }

    public dfTouchEventArgs(dfControl source, List<dfTouchInfo> touches, Ray ray)
      : this(source, touches.First<dfTouchInfo>(), ray)
    {
      this.Touches = touches;
    }

    public dfTouchEventArgs(dfControl Source)
      : base(Source)
    {
      this.Position = Vector2.zero;
    }

    public dfTouchInfo Touch { get; private set; }

    public List<dfTouchInfo> Touches { get; private set; }

    public bool IsMultiTouch => this.Touches.Count > 1;
  }

