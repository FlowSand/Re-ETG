// Decompiled with JetBrains decompiler
// Type: dfDragHandle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/User Interface/Drag Handle")]
[ExecuteInEditMode]
[Serializable]
public class dfDragHandle : dfControl
{
  private Vector3 lastPosition;

  public override void Start()
  {
    base.Start();
    if ((double) this.Size.magnitude > 1.4012984643248171E-45)
      return;
    if ((UnityEngine.Object) this.Parent != (UnityEngine.Object) null)
    {
      this.Size = new Vector2(this.Parent.Width, 30f);
      this.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Left | dfAnchorStyle.Right;
      this.RelativePosition = (Vector3) Vector2.zero;
    }
    else
      this.Size = new Vector2(200f, 25f);
  }

  protected internal override void OnMouseDown(dfMouseEventArgs args)
  {
    this.GetRootContainer().BringToFront();
    this.Parent.BringToFront();
    args.Use();
    Plane plane = new Plane(this.parent.transform.TransformDirection(Vector3.back), this.parent.transform.position);
    Ray ray = args.Ray;
    float enter = 0.0f;
    plane.Raycast(args.Ray, out enter);
    this.lastPosition = ray.origin + ray.direction * enter;
    base.OnMouseDown(args);
  }

  protected internal override void OnMouseMove(dfMouseEventArgs args)
  {
    args.Use();
    if (args.Buttons.IsSet(dfMouseButtons.Left))
    {
      Ray ray = args.Ray;
      float enter = 0.0f;
      new Plane(this.GetCamera().transform.TransformDirection(Vector3.back), this.lastPosition).Raycast(ray, out enter);
      Vector3 vector3 = (ray.origin + ray.direction * enter).Quantize(this.parent.PixelsToUnits());
      this.parent.transform.position = (this.parent.transform.position + (vector3 - this.lastPosition)).Quantize(this.parent.PixelsToUnits());
      this.lastPosition = vector3;
    }
    base.OnMouseMove(args);
  }

  protected internal override void OnMouseUp(dfMouseEventArgs args)
  {
    base.OnMouseUp(args);
    this.Parent.MakePixelPerfect();
  }
}
