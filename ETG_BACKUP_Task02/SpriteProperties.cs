// Decompiled with JetBrains decompiler
// Type: SpriteProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Sprites/Sprite Properties")]
public class SpriteProperties : MonoBehaviour
{
  public Texture2D blankTexture;
  public dfSprite sprite;

  private void Awake()
  {
    if ((Object) this.sprite == (Object) null)
      this.sprite = this.GetComponent<dfSprite>();
    this.ShowBorders = true;
    this.useGUILayout = false;
  }

  private void Start()
  {
  }

  private void OnGUI()
  {
    if (!this.ShowBorders || (Object) this.blankTexture == (Object) null)
      return;
    Rect screenRect = this.sprite.GetScreenRect();
    RectOffset border = this.sprite.SpriteInfo.border;
    float x = screenRect.x;
    float y = screenRect.y;
    float width = screenRect.width;
    float height = screenRect.height;
    int num1 = border.left;
    int num2 = border.right;
    int num3 = border.top;
    int num4 = border.bottom;
    if (this.sprite.Flip.IsSet(dfSpriteFlip.FlipHorizontal))
    {
      num1 = border.right;
      num2 = border.left;
    }
    if (this.sprite.Flip.IsSet(dfSpriteFlip.FlipVertical))
    {
      num3 = border.bottom;
      num4 = border.top;
    }
    GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    GUI.DrawTexture(new Rect(x - 10f, y + (float) num3, width + 20f, 1f), (Texture) this.blankTexture);
    GUI.DrawTexture(new Rect(x - 10f, y + height - (float) num4, width + 20f, 1f), (Texture) this.blankTexture);
    GUI.DrawTexture(new Rect(x + (float) num1, y - 10f, 1f, height + 20f), (Texture) this.blankTexture);
    GUI.DrawTexture(new Rect(x + width - (float) num2, y - 10f, 1f, height + 20f), (Texture) this.blankTexture);
  }

  public bool ShowBorders { get; set; }

  public float PatternScaleX
  {
    get => ((dfTiledSprite) this.sprite).TileScale.x;
    set
    {
      dfTiledSprite sprite = this.sprite as dfTiledSprite;
      sprite.TileScale = new Vector2(value, sprite.TileScale.y);
    }
  }

  public float PatternScaleY
  {
    get => ((dfTiledSprite) this.sprite).TileScale.y;
    set
    {
      dfTiledSprite sprite = this.sprite as dfTiledSprite;
      sprite.TileScale = new Vector2(sprite.TileScale.x, value);
    }
  }

  public float PatternOffsetX
  {
    get => ((dfTiledSprite) this.sprite).TileScroll.x;
    set
    {
      dfTiledSprite sprite = this.sprite as dfTiledSprite;
      sprite.TileScroll = new Vector2(value, sprite.TileScroll.y);
    }
  }

  public float PatternOffsetY
  {
    get => ((dfTiledSprite) this.sprite).TileScroll.y;
    set
    {
      dfTiledSprite sprite = this.sprite as dfTiledSprite;
      sprite.TileScroll = new Vector2(sprite.TileScroll.x, value);
    }
  }

  public int FillOrigin
  {
    get => (int) ((dfRadialSprite) this.sprite).FillOrigin;
    set => ((dfRadialSprite) this.sprite).FillOrigin = (dfPivotPoint) value;
  }

  public bool FillVertical
  {
    get => this.sprite.FillDirection == dfFillDirection.Vertical;
    set
    {
      if (value)
        this.sprite.FillDirection = dfFillDirection.Vertical;
      else
        this.sprite.FillDirection = dfFillDirection.Horizontal;
    }
  }

  public bool FlipHorizontal
  {
    get => this.sprite.Flip.IsSet(dfSpriteFlip.FlipHorizontal);
    set => this.sprite.Flip = this.sprite.Flip.SetFlag(dfSpriteFlip.FlipHorizontal, value);
  }

  public bool FlipVertical
  {
    get => this.sprite.Flip.IsSet(dfSpriteFlip.FlipVertical);
    set => this.sprite.Flip = this.sprite.Flip.SetFlag(dfSpriteFlip.FlipVertical, value);
  }
}
