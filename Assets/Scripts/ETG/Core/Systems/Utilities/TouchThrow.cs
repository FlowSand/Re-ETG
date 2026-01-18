// Decompiled with JetBrains decompiler
// Type: TouchThrow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Touch/Touch Throw")]
public class TouchThrow : MonoBehaviour
  {
    private dfControl control;
    private dfGUIManager manager;
    private Vector2 momentum;
    private Vector3 lastPosition;
    private bool animating;
    private bool dragging;

    public void Start()
    {
      this.control = this.GetComponent<dfControl>();
      this.manager = this.control.GetManager();
    }

    public void Update()
    {
      Vector2 screenSize = this.control.GetManager().GetScreenSize();
      Vector2 relativePosition = (Vector2) this.control.RelativePosition;
      Vector2 rhs1 = relativePosition;
      if (this.animating)
      {
        if ((double) relativePosition.x + (double) this.momentum.x < 0.0 || (double) relativePosition.x + (double) this.momentum.x + (double) this.control.Width > (double) screenSize.x)
          this.momentum.x *= -1f;
        if ((double) relativePosition.y + (double) this.momentum.y < 0.0 || (double) relativePosition.y + (double) this.momentum.y + (double) this.control.Height > (double) screenSize.y)
          this.momentum.y *= -1f;
        rhs1 += this.momentum;
        this.momentum *= 1f - UnityEngine.Time.fixedDeltaTime;
      }
      Vector2 rhs2 = Vector2.Max(Vector2.zero, rhs1);
      Vector2 a = Vector2.Min(screenSize - this.control.Size, rhs2);
      if ((double) Vector2.Distance(a, relativePosition) <= 1.4012984643248171E-45)
        return;
      this.control.RelativePosition = (Vector3) a;
    }

    public void OnMultiTouch(dfControl control, dfTouchEventArgs touchData)
    {
      this.momentum = Vector2.zero;
      control.Color = (Color32) Color.yellow;
      dfTouchInfo touch1 = touchData.Touches[0];
      dfTouchInfo touch2 = touchData.Touches[1];
      Vector2 vector2_1 = (touch1.deltaPosition * (BraveTime.DeltaTime / touch1.deltaTime)).Scale(1f, -1f);
      Vector2 vector2_2 = (touch2.deltaPosition * (BraveTime.DeltaTime / touch2.deltaTime)).Scale(1f, -1f);
      Vector2 gui1 = this.screenToGUI(touch1.position);
      Vector2 gui2 = this.screenToGUI(touch2.position);
      float f = (gui1 - gui2).magnitude - (gui1 - vector2_1 - (gui2 - vector2_2)).magnitude;
      if ((double) Mathf.Abs(f) <= 1.4012984643248171E-45)
        return;
      Vector3 vector3_1 = Vector3.Min((Vector3) gui1, (Vector3) gui2);
      Vector3 vector3_2 = vector3_1 - control.RelativePosition;
      control.Size += Vector2.one * f;
      control.RelativePosition = vector3_1 + vector3_2;
    }

    private Vector2 screenToGUI(Vector2 position)
    {
      position.y = this.manager.GetScreenSize().y - position.y;
      return position;
    }

    public void OnMouseMove(dfControl control, dfMouseEventArgs args)
    {
      if (this.animating || !this.dragging)
        return;
      this.momentum = (this.momentum + args.MoveDelta.Scale(1f, -1f)) * 0.5f;
      args.Use();
      if (!args.Buttons.IsSet(dfMouseButtons.Left))
        return;
      Ray ray = args.Ray;
      float enter = 0.0f;
      new Plane(Camera.main.transform.TransformDirection(Vector3.back), this.lastPosition).Raycast(ray, out enter);
      Vector3 vector3_1 = (ray.origin + ray.direction * enter).Quantize(control.PixelsToUnits());
      Vector3 vector3_2 = vector3_1 - this.lastPosition;
      Vector3 vector3_3 = (control.transform.position + vector3_2).Quantize(control.PixelsToUnits());
      control.transform.position = vector3_3;
      this.lastPosition = vector3_1;
    }

    public void OnMouseEnter(dfControl control, dfMouseEventArgs args)
    {
      control.Color = (Color32) Color.white;
    }

    public void OnMouseDown(dfControl control, dfMouseEventArgs args)
    {
      control.BringToFront();
      this.animating = false;
      this.momentum = Vector2.zero;
      this.dragging = true;
      args.Use();
      Plane plane = new Plane(control.transform.TransformDirection(Vector3.back), control.transform.position);
      Ray ray = args.Ray;
      float enter = 0.0f;
      plane.Raycast(args.Ray, out enter);
      this.lastPosition = ray.origin + ray.direction * enter;
    }

    public void OnMouseUp()
    {
      this.animating = true;
      this.dragging = false;
      this.control.Color = (Color32) Color.white;
    }
  }

