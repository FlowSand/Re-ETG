using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class DEMO_PictureManipulation : MonoBehaviour
  {
    private dfTextureSprite control;
    private bool isMouseDown;

    public void Start() => this.control = this.GetComponent<dfTextureSprite>();

    protected void OnMouseUp() => this.isMouseDown = false;

    protected void OnMouseDown()
    {
      this.isMouseDown = true;
      this.control.BringToFront();
    }

    public IEnumerator OnFlickGesture(dfGestureBase gesture) => this.handleMomentum(gesture);

    public IEnumerator OnDoubleTapGesture() => this.resetZoom();

    public void OnRotateGestureBegin(dfRotateGesture gesture)
    {
      this.rotateAroundPoint(gesture.StartPosition, gesture.AngleDelta * 0.5f);
    }

    public void OnRotateGestureUpdate(dfRotateGesture gesture)
    {
      this.rotateAroundPoint(gesture.CurrentPosition, gesture.AngleDelta * 0.5f);
    }

    public void OnResizeGestureUpdate(dfResizeGesture gesture)
    {
      this.zoomToPoint(gesture.StartPosition, gesture.SizeDelta * 1f);
    }

    public void OnPanGestureStart(dfPanGesture gesture)
    {
      this.control.BringToFront();
      dfTextureSprite control = this.control;
      control.RelativePosition = control.RelativePosition + (Vector3) gesture.Delta.Scale(1f, -1f);
    }

    public void OnPanGestureMove(dfPanGesture gesture)
    {
      dfTextureSprite control = this.control;
      control.RelativePosition = control.RelativePosition + (Vector3) gesture.Delta.Scale(1f, -1f);
    }

    public void OnMouseWheel(dfControl sender, dfMouseEventArgs args)
    {
      this.zoomToPoint(args.Position, Mathf.Sign(args.WheelDelta) * 75f);
    }

    [DebuggerHidden]
    private IEnumerator resetZoom()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DEMO_PictureManipulation__resetZoomc__Iterator0()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator handleMomentum(dfGestureBase gesture)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DEMO_PictureManipulation__handleMomentumc__Iterator1()
      {
        gesture = gesture,
        _this = this
      };
    }

    private void rotateAroundPoint(Vector2 point, float delta)
    {
      Transform transform = this.control.transform;
      Vector3[] corners = this.control.GetCorners();
      Plane plane = new Plane(corners[0], corners[1], corners[2]);
      Ray ray = this.control.GetCamera().ScreenPointToRay((Vector3) point);
      float enter = 0.0f;
      plane.Raycast(ray, out enter);
      Vector3 point1 = ray.GetPoint(enter);
      Vector3 angles = new Vector3(0.0f, 0.0f, delta);
      Vector3 vector3_1 = transform.eulerAngles + angles;
      Vector3 vector3_2 = this.rotatePointAroundPivot(transform.position, point1, angles);
      transform.position = vector3_2;
      transform.eulerAngles = vector3_1;
    }

    private Vector3 rotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
      return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    private void zoomToPoint(Vector2 point, float delta)
    {
      Transform transform = this.control.transform;
      Vector3[] corners = this.control.GetCorners();
      float units = this.control.PixelsToUnits();
      Plane plane = new Plane(corners[0], corners[1], corners[2]);
      Ray ray = this.control.GetCamera().ScreenPointToRay((Vector3) point);
      float enter = 0.0f;
      plane.Raycast(ray, out enter);
      Vector3 point1 = ray.GetPoint(enter);
      Vector2 vector2 = this.control.Size * units;
      Vector3 vector3_1 = transform.position - point1;
      Vector3 vector3_2 = new Vector3(vector3_1.x / vector2.x, vector3_1.y / vector2.y);
      float num = this.control.Height / this.control.Width;
      float x = this.control.Width + delta;
      float y = x * num;
      if ((double) x < 256.0 || (double) y < 256.0)
        return;
      this.control.Size = new Vector2(x, y);
      Vector3 vector3_3 = new Vector3(this.control.Width * vector3_2.x * units, this.control.Height * vector3_2.y * units, vector3_1.z);
      transform.position += vector3_3 - vector3_1;
    }

    private static Vector2 fitImage(
      float maxWidth,
      float maxHeight,
      float imageWidth,
      float imageHeight)
    {
      float num = Mathf.Min(maxWidth / imageWidth, maxHeight / imageHeight);
      return new Vector2(Mathf.Floor(imageWidth * num), Mathf.Ceil(imageHeight * num));
    }
  }

