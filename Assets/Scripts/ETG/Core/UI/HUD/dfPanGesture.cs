using System.Collections.Generic;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Input/Gestures/Pan")]
public class dfPanGesture : dfGestureBase
  {
    [SerializeField]
    protected float minDistance = 25f;
    private bool multiTouchMode;

    public event dfGestureEventHandler<dfPanGesture> PanGestureStart;

    public event dfGestureEventHandler<dfPanGesture> PanGestureMove;

    public event dfGestureEventHandler<dfPanGesture> PanGestureEnd;

    public float MinimumDistance
    {
      get => this.minDistance;
      set => this.minDistance = value;
    }

    public Vector2 Delta { get; protected set; }

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
      this.Delta = Vector2.zero;
    }

    public void OnMouseMove(dfControl source, dfMouseEventArgs args)
    {
      if (this.State == dfGestureState.Possible)
      {
        if ((double) Vector2.Distance(args.Position, this.StartPosition) < (double) this.minDistance)
          return;
        this.State = dfGestureState.Began;
        this.CurrentPosition = args.Position;
        this.Delta = args.Position - this.StartPosition;
        if (this.PanGestureStart != null)
          this.PanGestureStart(this);
        this.gameObject.Signal("OnPanGestureStart", (object) this);
      }
      else
      {
        if (this.State != dfGestureState.Began && this.State != dfGestureState.Changed)
          return;
        this.State = dfGestureState.Changed;
        this.Delta = args.Position - this.CurrentPosition;
        this.CurrentPosition = args.Position;
        if (this.PanGestureMove != null)
          this.PanGestureMove(this);
        this.gameObject.Signal("OnPanGestureMove", (object) this);
      }
    }

    public void OnMouseUp(dfControl source, dfMouseEventArgs args) => this.endPanGesture();

    public void OnMultiTouchEnd()
    {
      this.endPanGesture();
      this.multiTouchMode = false;
    }

    public void OnMultiTouch(dfControl source, dfTouchEventArgs args)
    {
      Vector2 center = this.getCenter(args.Touches);
      if (!this.multiTouchMode)
      {
        this.endPanGesture();
        this.multiTouchMode = true;
        this.State = dfGestureState.Possible;
        this.StartPosition = center;
      }
      else if (this.State == dfGestureState.Possible)
      {
        if ((double) Vector2.Distance(center, this.StartPosition) < (double) this.minDistance)
          return;
        this.State = dfGestureState.Began;
        this.CurrentPosition = center;
        this.Delta = this.CurrentPosition - this.StartPosition;
        if (this.PanGestureStart != null)
          this.PanGestureStart(this);
        this.gameObject.Signal("OnPanGestureStart", (object) this);
      }
      else
      {
        if (this.State != dfGestureState.Began && this.State != dfGestureState.Changed)
          return;
        this.State = dfGestureState.Changed;
        this.Delta = center - this.CurrentPosition;
        this.CurrentPosition = center;
        if (this.PanGestureMove != null)
          this.PanGestureMove(this);
        this.gameObject.Signal("OnPanGestureMove", (object) this);
      }
    }

    private Vector2 getCenter(List<dfTouchInfo> list)
    {
      Vector2 zero = Vector2.zero;
      for (int index = 0; index < list.Count; ++index)
        zero += list[index].position;
      return zero / (float) list.Count;
    }

    private void endPanGesture()
    {
      this.Delta = Vector2.zero;
      this.StartPosition = Vector2.one * float.MinValue;
      if (this.State == dfGestureState.Began || this.State == dfGestureState.Changed)
      {
        this.State = dfGestureState.Ended;
        if (this.PanGestureEnd != null)
          this.PanGestureEnd(this);
        this.gameObject.Signal("OnPanGestureEnd", (object) this);
      }
      else
      {
        if (this.State != dfGestureState.Possible)
          return;
        this.State = dfGestureState.Cancelled;
      }
    }
  }

