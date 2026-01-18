using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class dfTouchJoystick : MonoBehaviour
  {
    private static Dictionary<string, dfTouchJoystick> joysticks = new Dictionary<string, dfTouchJoystick>();
    [SerializeField]
    public string JoystickID = "Joystick";
    [SerializeField]
    public dfTouchJoystick.TouchJoystickType JoystickType;
    [SerializeField]
    public int Radius = 80 /*0x50*/;
    [SerializeField]
    public float DeadzoneRadius = 0.25f;
    [SerializeField]
    public bool DynamicThumb;
    [SerializeField]
    public bool HideThumb;
    [SerializeField]
    public dfControl ThumbControl;
    [SerializeField]
    public dfControl AreaControl;
    private dfControl control;
    private Vector2 joystickPos = Vector2.zero;

    public Vector2 Position => this.joystickPos;

    public static Vector2 GetJoystickPosition(string joystickID)
    {
      if (!dfTouchJoystick.joysticks.ContainsKey(joystickID))
        throw new Exception("Joystick not registered: " + joystickID);
      return dfTouchJoystick.joysticks[joystickID].Position;
    }

    public static void ResetJoystickPosition(string joystickID)
    {
      dfTouchJoystick dfTouchJoystick = dfTouchJoystick.joysticks.ContainsKey(joystickID) ? dfTouchJoystick.joysticks[joystickID] : throw new Exception("Joystick not registered: " + joystickID);
      if (dfTouchJoystick.JoystickType == dfTouchJoystick.TouchJoystickType.Trackpad)
        dfTouchJoystick.joystickPos = Vector2.zero;
      else
        dfTouchJoystick.recenter();
    }

    public void Start()
    {
      this.control = this.GetComponent<dfControl>();
      if ((this.JoystickType != dfTouchJoystick.TouchJoystickType.Trackpad || !((UnityEngine.Object) this.control != (UnityEngine.Object) null)) && (!((UnityEngine.Object) this.control != (UnityEngine.Object) null) || !((UnityEngine.Object) this.ThumbControl != (UnityEngine.Object) null) || !((UnityEngine.Object) this.AreaControl != (UnityEngine.Object) null)))
      {
        Debug.LogError((object) "Invalid virtual joystick configuration", (UnityEngine.Object) this);
        this.enabled = false;
      }
      else
      {
        dfTouchJoystick.joysticks.Add(this.JoystickID, this);
        if ((UnityEngine.Object) this.ThumbControl != (UnityEngine.Object) null && this.HideThumb)
        {
          this.ThumbControl.Hide();
          if (this.DynamicThumb)
            this.AreaControl.Hide();
        }
        this.recenter();
      }
    }

    public void OnDestroy() => dfTouchJoystick.joysticks.Remove(this.JoystickID);

    public void OnMouseDown(dfControl control, dfMouseEventArgs args)
    {
      if (this.JoystickType == dfTouchJoystick.TouchJoystickType.Trackpad)
        return;
      Vector2 position;
      control.GetHitPosition(args.Ray, out position, true);
      if (this.HideThumb)
      {
        this.ThumbControl.Show();
        this.AreaControl.Show();
      }
      if (this.DynamicThumb)
      {
        this.AreaControl.RelativePosition = (Vector3) (position - this.AreaControl.Size * 0.5f);
        this.centerThumbInArea();
      }
      else
        this.recenter();
      this.processTouch(args);
    }

    public void OnMouseHover()
    {
      if (this.JoystickType != dfTouchJoystick.TouchJoystickType.Trackpad)
        return;
      this.joystickPos = Vector2.zero;
    }

    public void OnMouseMove(dfControl control, dfMouseEventArgs args)
    {
      if (this.JoystickType == dfTouchJoystick.TouchJoystickType.Trackpad && args.Buttons.IsSet(dfMouseButtons.Left))
      {
        this.joystickPos = args.MoveDelta * 0.25f;
      }
      else
      {
        if (!args.Buttons.IsSet(dfMouseButtons.Left))
          return;
        this.processTouch(args);
      }
    }

    public void OnMouseUp(dfControl control, dfMouseEventArgs args)
    {
      if (this.JoystickType == dfTouchJoystick.TouchJoystickType.Trackpad)
      {
        this.joystickPos = Vector2.zero;
      }
      else
      {
        this.recenter();
        if (!this.HideThumb)
          return;
        this.ThumbControl.Hide();
        if (!this.DynamicThumb)
          return;
        this.AreaControl.Hide();
      }
    }

    private void recenter()
    {
      if (this.JoystickType == dfTouchJoystick.TouchJoystickType.Trackpad)
        return;
      this.AreaControl.RelativePosition = (Vector3) ((this.control.Size - this.AreaControl.Size) * 0.5f);
      this.ThumbControl.RelativePosition = this.AreaControl.RelativePosition + (Vector3) this.AreaControl.Size * 0.5f - (Vector3) this.ThumbControl.Size * 0.5f;
      this.joystickPos = Vector2.zero;
    }

    private void centerThumbInArea()
    {
      this.ThumbControl.RelativePosition = this.AreaControl.RelativePosition + (Vector3) (this.AreaControl.Size - this.ThumbControl.Size) * 0.5f;
    }

    private void processTouch(dfMouseEventArgs evt)
    {
      Vector2 vector2 = this.raycast(evt.Ray);
      Vector3 vector3_1 = this.AreaControl.RelativePosition + (Vector3) this.AreaControl.Size * 0.5f;
      Vector3 vector3_2 = (Vector3) vector2 - vector3_1;
      if ((double) vector3_2.magnitude > (double) this.Radius)
        vector3_2 = vector3_2.normalized * (float) this.Radius;
      Vector3 vector3_3 = (Vector3) this.ThumbControl.Size * 0.5f;
      this.ThumbControl.RelativePosition = vector3_1 - vector3_3 + vector3_2;
      Vector3 vector3_4 = vector3_2 / (float) this.Radius;
      if ((double) vector3_4.magnitude <= (double) this.DeadzoneRadius)
        this.joystickPos = Vector2.zero;
      else
        this.joystickPos = (Vector2) new Vector3(vector3_4.x, -vector3_4.y);
    }

    private Vector2 raycast(Ray ray)
    {
      Vector3[] corners = this.control.GetCorners();
      Plane plane = new Plane(corners[0], corners[1], corners[3]);
      float enter = 0.0f;
      plane.Raycast(ray, out enter);
      return (Vector2) ((ray.GetPoint(enter) - corners[0]).Scale(1f, -1f, 0.0f) / this.control.GetManager().PixelsToUnits());
    }

    public enum TouchJoystickType
    {
      Joystick,
      Trackpad,
    }
  }

