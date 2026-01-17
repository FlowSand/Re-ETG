// Decompiled with JetBrains decompiler
// Type: dfResizeHandle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[AddComponentMenu("Daikon Forge/User Interface/Resize Handle")]
[Serializable]
public class dfResizeHandle : dfControl
{
  [SerializeField]
  protected dfAtlas atlas;
  [SerializeField]
  protected string backgroundSprite = string.Empty;
  [SerializeField]
  protected dfResizeHandle.ResizeEdge edges = dfResizeHandle.ResizeEdge.Right | dfResizeHandle.ResizeEdge.Bottom;
  private Vector3 mouseAnchorPos;
  private Vector3 startPosition;
  private Vector2 startSize;
  private Vector2 minEdgePos;
  private Vector2 maxEdgePos;

  public dfAtlas Atlas
  {
    get
    {
      if ((UnityEngine.Object) this.atlas == (UnityEngine.Object) null)
      {
        dfGUIManager manager = this.GetManager();
        if ((UnityEngine.Object) manager != (UnityEngine.Object) null)
          return this.atlas = manager.DefaultAtlas;
      }
      return this.atlas;
    }
    set
    {
      if (dfAtlas.Equals(value, this.atlas))
        return;
      this.atlas = value;
      this.Invalidate();
    }
  }

  public string BackgroundSprite
  {
    get => this.backgroundSprite;
    set
    {
      if (!(value != this.backgroundSprite))
        return;
      this.backgroundSprite = value;
      this.Invalidate();
    }
  }

  public dfResizeHandle.ResizeEdge Edges
  {
    get => this.edges;
    set => this.edges = value;
  }

  public override void Start()
  {
    base.Start();
    if ((double) this.Size.magnitude > 1.4012984643248171E-45)
      return;
    this.Size = new Vector2(25f, 25f);
    if (!((UnityEngine.Object) this.Parent != (UnityEngine.Object) null))
      return;
    this.RelativePosition = (Vector3) (this.Parent.Size - this.Size);
    this.Anchor = dfAnchorStyle.Bottom | dfAnchorStyle.Right;
  }

  protected override void OnRebuildRenderData()
  {
    if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null || string.IsNullOrEmpty(this.backgroundSprite))
      return;
    dfAtlas.ItemInfo atla = this.Atlas[this.backgroundSprite];
    if (atla == (dfAtlas.ItemInfo) null)
      return;
    this.renderData.Material = this.Atlas.Material;
    Color32 color32 = this.ApplyOpacity(!this.IsEnabled ? this.disabledColor : this.color);
    dfSprite.RenderOptions options = new dfSprite.RenderOptions()
    {
      atlas = this.atlas,
      color = color32,
      fillAmount = 1f,
      flip = dfSpriteFlip.None,
      offset = this.pivot.TransformToUpperLeft(this.Size),
      pixelsToUnits = this.PixelsToUnits(),
      size = this.Size,
      spriteInfo = atla
    };
    if (atla.border.horizontal == 0 && atla.border.vertical == 0)
      dfSprite.renderSprite(this.renderData, options);
    else
      dfSlicedSprite.renderSprite(this.renderData, options);
  }

  protected internal override void OnMouseDown(dfMouseEventArgs args)
  {
    args.Use();
    Plane plane = new Plane(this.parent.transform.TransformDirection(Vector3.back), this.parent.transform.position);
    Ray ray = args.Ray;
    float enter = 0.0f;
    plane.Raycast(args.Ray, out enter);
    this.mouseAnchorPos = ray.origin + ray.direction * enter;
    this.startSize = this.parent.Size;
    this.startPosition = this.parent.RelativePosition;
    this.minEdgePos = (Vector2) this.startPosition;
    this.maxEdgePos = (Vector2) this.startPosition + this.startSize;
    Vector2 minimumSize = this.parent.CalculateMinimumSize();
    Vector2 vector2 = this.parent.MaximumSize;
    if ((double) vector2.magnitude <= 1.4012984643248171E-45)
      vector2 = Vector2.one * 2048f;
    if ((this.Edges & dfResizeHandle.ResizeEdge.Left) == dfResizeHandle.ResizeEdge.Left)
    {
      this.minEdgePos.x = this.maxEdgePos.x - vector2.x;
      this.maxEdgePos.x -= minimumSize.x;
    }
    else if ((this.Edges & dfResizeHandle.ResizeEdge.Right) == dfResizeHandle.ResizeEdge.Right)
    {
      this.minEdgePos.x = this.startPosition.x + minimumSize.x;
      this.maxEdgePos.x = this.startPosition.x + vector2.x;
    }
    if ((this.Edges & dfResizeHandle.ResizeEdge.Top) == dfResizeHandle.ResizeEdge.Top)
    {
      this.minEdgePos.y = this.maxEdgePos.y - vector2.y;
      this.maxEdgePos.y -= minimumSize.y;
    }
    else if ((this.Edges & dfResizeHandle.ResizeEdge.Bottom) == dfResizeHandle.ResizeEdge.Bottom)
    {
      this.minEdgePos.y = this.startPosition.y + minimumSize.y;
      this.maxEdgePos.y = this.startPosition.y + vector2.y;
    }
    base.OnMouseDown(args);
  }

  protected internal override void OnMouseMove(dfMouseEventArgs args)
  {
    if (!args.Buttons.IsSet(dfMouseButtons.Left) || this.Edges == dfResizeHandle.ResizeEdge.None)
      return;
    args.Use();
    Ray ray = args.Ray;
    float enter = 0.0f;
    new Plane(this.GetCamera().transform.TransformDirection(Vector3.back), this.mouseAnchorPos).Raycast(ray, out enter);
    float units = this.PixelsToUnits();
    Vector3 vector3 = (ray.origin + ray.direction * enter - this.mouseAnchorPos) / units;
    vector3.y *= -1f;
    float x = this.startPosition.x;
    float y = this.startPosition.y;
    float num1 = x + this.startSize.x;
    float num2 = y + this.startSize.y;
    if ((this.Edges & dfResizeHandle.ResizeEdge.Left) == dfResizeHandle.ResizeEdge.Left)
      x = Mathf.Min(this.maxEdgePos.x, Mathf.Max(this.minEdgePos.x, x + vector3.x));
    else if ((this.Edges & dfResizeHandle.ResizeEdge.Right) == dfResizeHandle.ResizeEdge.Right)
      num1 = Mathf.Min(this.maxEdgePos.x, Mathf.Max(this.minEdgePos.x, num1 + vector3.x));
    if ((this.Edges & dfResizeHandle.ResizeEdge.Top) == dfResizeHandle.ResizeEdge.Top)
      y = Mathf.Min(this.maxEdgePos.y, Mathf.Max(this.minEdgePos.y, y + vector3.y));
    else if ((this.Edges & dfResizeHandle.ResizeEdge.Bottom) == dfResizeHandle.ResizeEdge.Bottom)
      num2 = Mathf.Min(this.maxEdgePos.y, Mathf.Max(this.minEdgePos.y, num2 + vector3.y));
    this.parent.Size = new Vector2(num1 - x, num2 - y);
    this.parent.RelativePosition = new Vector3(x, y, 0.0f);
    if (!this.parent.GetManager().PixelPerfectMode)
      return;
    this.parent.MakePixelPerfect();
  }

  protected internal override void OnMouseUp(dfMouseEventArgs args)
  {
    this.Parent.MakePixelPerfect();
    args.Use();
    base.OnMouseUp(args);
  }

  [Flags]
  public enum ResizeEdge
  {
    None = 0,
    Left = 1,
    Right = 2,
    Top = 4,
    Bottom = 8,
  }
}
