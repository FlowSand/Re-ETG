// Decompiled with JetBrains decompiler
// Type: dfHoldGesture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Input/Gestures/Hold")]
public class dfHoldGesture : dfGestureBase
  {
    [SerializeField]
    private float minTime = 0.75f;
    [SerializeField]
    private float maxDistance = 25f;

    public event dfGestureEventHandler<dfHoldGesture> HoldGestureStart;

    public event dfGestureEventHandler<dfHoldGesture> HoldGestureEnd;

    public float MinimumTime
    {
      get => this.minTime;
      set => this.minTime = value;
    }

    public float MaximumDistance
    {
      get => this.maxDistance;
      set => this.maxDistance = value;
    }

    public float HoldLength
    {
      get => this.State == dfGestureState.Began ? UnityEngine.Time.realtimeSinceStartup - this.StartTime : 0.0f;
    }

    protected void Start()
    {
    }

    protected void Update()
    {
      if (this.State != dfGestureState.Possible || (double) UnityEngine.Time.realtimeSinceStartup - (double) this.StartTime < (double) this.minTime)
        return;
      this.State = dfGestureState.Began;
      if (this.HoldGestureStart != null)
        this.HoldGestureStart(this);
      this.gameObject.Signal("OnHoldGestureStart", (object) this);
    }

    public void OnMouseDown(dfControl source, dfMouseEventArgs args)
    {
      this.State = dfGestureState.Possible;
      Vector2 position = args.Position;
      this.CurrentPosition = position;
      this.StartPosition = position;
      this.StartTime = UnityEngine.Time.realtimeSinceStartup;
    }

    public void OnMouseMove(dfControl source, dfMouseEventArgs args)
    {
      if (this.State != dfGestureState.Possible && this.State != dfGestureState.Began)
        return;
      this.CurrentPosition = args.Position;
      if ((double) Vector2.Distance(args.Position, this.StartPosition) <= (double) this.maxDistance)
        return;
      if (this.State == dfGestureState.Possible)
      {
        this.State = dfGestureState.Failed;
      }
      else
      {
        if (this.State != dfGestureState.Began)
          return;
        this.State = dfGestureState.Cancelled;
        if (this.HoldGestureEnd != null)
          this.HoldGestureEnd(this);
        this.gameObject.Signal("OnHoldGestureEnd", (object) this);
      }
    }

    public void OnMouseUp(dfControl source, dfMouseEventArgs args)
    {
      if (this.State == dfGestureState.Began)
      {
        this.CurrentPosition = args.Position;
        this.State = dfGestureState.Ended;
        if (this.HoldGestureEnd != null)
          this.HoldGestureEnd(this);
        this.gameObject.Signal("OnHoldGestureEnd", (object) this);
      }
      this.State = dfGestureState.None;
    }

    public void OnMultiTouch(dfControl source, dfTouchEventArgs args)
    {
      if (this.State == dfGestureState.Began)
      {
        this.State = dfGestureState.Cancelled;
        if (this.HoldGestureEnd != null)
          this.HoldGestureEnd(this);
        this.gameObject.Signal("OnHoldGestureEnd", (object) this);
      }
      else
        this.State = dfGestureState.Failed;
    }
  }

