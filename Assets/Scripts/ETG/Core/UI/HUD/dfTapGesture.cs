// Decompiled with JetBrains decompiler
// Type: dfTapGesture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [AddComponentMenu("Daikon Forge/Input/Gestures/Tap")]
    public class dfTapGesture : dfGestureBase
    {
      [SerializeField]
      private float timeout = 0.25f;
      [SerializeField]
      private float maxDistance = 25f;

      public event dfGestureEventHandler<dfTapGesture> TapGesture;

      public float Timeout
      {
        get => this.timeout;
        set => this.timeout = value;
      }

      public float MaximumDistance
      {
        get => this.maxDistance;
        set => this.maxDistance = value;
      }

      protected void Start()
      {
      }

      public void OnMouseDown(dfControl source, dfMouseEventArgs args)
      {
        Vector2 position = args.Position;
        this.CurrentPosition = position;
        this.StartPosition = position;
        this.State = dfGestureState.Possible;
        this.StartTime = UnityEngine.Time.realtimeSinceStartup;
      }

      public void OnMouseMove(dfControl source, dfMouseEventArgs args)
      {
        if (this.State != dfGestureState.Possible && this.State != dfGestureState.Began)
          return;
        this.CurrentPosition = args.Position;
        if ((double) Vector2.Distance(args.Position, this.StartPosition) <= (double) this.maxDistance)
          return;
        this.State = dfGestureState.Failed;
      }

      public void OnMouseUp(dfControl source, dfMouseEventArgs args)
      {
        if (this.State == dfGestureState.Possible)
        {
          if ((double) UnityEngine.Time.realtimeSinceStartup - (double) this.StartTime <= (double) this.timeout)
          {
            this.CurrentPosition = args.Position;
            this.State = dfGestureState.Ended;
            if (this.TapGesture != null)
              this.TapGesture(this);
            this.gameObject.Signal("OnTapGesture", (object) this);
          }
          else
            this.State = dfGestureState.Failed;
        }
        else
          this.State = dfGestureState.None;
      }

      public void OnMultiTouch(dfControl source, dfTouchEventArgs args)
      {
        this.State = dfGestureState.Failed;
      }
    }

}
