// Decompiled with JetBrains decompiler
// Type: dfFlickGesture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Input/Gestures/Flick")]
public class dfFlickGesture : dfGestureBase
  {
    [SerializeField]
    private float timeout = 0.25f;
    [SerializeField]
    private float minDistance = 25f;
    private float hoverTime;

    public event dfGestureEventHandler<dfFlickGesture> FlickGesture;

    public float Timeout
    {
      get => this.timeout;
      set => this.timeout = value;
    }

    public float MinimumDistance
    {
      get => this.minDistance;
      set => this.minDistance = value;
    }

    public float DeltaTime { get; protected set; }

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
      this.hoverTime = UnityEngine.Time.realtimeSinceStartup;
    }

    public void OnMouseHover(dfControl source, dfMouseEventArgs args)
    {
      if (this.State != dfGestureState.Possible || (double) UnityEngine.Time.realtimeSinceStartup - (double) this.hoverTime < (double) this.timeout)
        return;
      Vector2 position = args.Position;
      this.CurrentPosition = position;
      this.StartPosition = position;
      this.StartTime = UnityEngine.Time.realtimeSinceStartup;
    }

    public void OnMouseMove(dfControl source, dfMouseEventArgs args)
    {
      this.hoverTime = UnityEngine.Time.realtimeSinceStartup;
      if (this.State != dfGestureState.Possible && this.State != dfGestureState.Began)
        return;
      this.State = dfGestureState.Began;
      this.CurrentPosition = args.Position;
    }

    public void OnMouseUp(dfControl source, dfMouseEventArgs args)
    {
      if (this.State != dfGestureState.Began)
        return;
      this.CurrentPosition = args.Position;
      if ((double) UnityEngine.Time.realtimeSinceStartup - (double) this.StartTime <= (double) this.timeout)
      {
        if ((double) Vector2.Distance(this.CurrentPosition, this.StartPosition) >= (double) this.minDistance)
        {
          this.State = dfGestureState.Ended;
          this.DeltaTime = UnityEngine.Time.realtimeSinceStartup - this.StartTime;
          if (this.FlickGesture != null)
            this.FlickGesture(this);
          this.gameObject.Signal("OnFlickGesture", (object) this);
        }
        else
          this.State = dfGestureState.Failed;
      }
      else
        this.State = dfGestureState.Failed;
    }

    public void OnMultiTouchEnd() => this.endGesture();

    public void OnMultiTouch() => this.endGesture();

    private void endGesture() => this.State = dfGestureState.None;
  }

