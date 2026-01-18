using UnityEngine;

#nullable disable

  public abstract class dfGestureBase : MonoBehaviour
  {
    private dfControl control;

    public dfGestureState State { get; protected set; }

    public Vector2 StartPosition { get; protected set; }

    public Vector2 CurrentPosition { get; protected set; }

    public float StartTime { get; protected set; }

    public dfControl Control
    {
      get
      {
        if ((Object) this.control == (Object) null)
          this.control = this.GetComponent<dfControl>();
        return this.control;
      }
    }
  }

