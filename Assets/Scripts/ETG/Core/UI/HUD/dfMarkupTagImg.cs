// Decompiled with JetBrains decompiler
// Type: dfMarkupTagImg
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [dfMarkupTagInfo("img")]
    public class dfMarkupTagImg : dfMarkupTag
    {
      public dfMarkupTagImg()
        : base("img")
      {
        this.IsClosedTag = true;
      }

      public dfMarkupTagImg(dfMarkupTag original)
        : base(original)
      {
        this.IsClosedTag = true;
      }

      protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
      {
        if ((Object) this.Owner == (Object) null)
        {
          Debug.LogError((object) ("Tag has no parent: " + (object) this));
        }
        else
        {
          style = this.applyStyleAttributes(style);
          dfMarkupAttribute attribute1 = this.findAttribute("src");
          if (attribute1 == null)
            return;
          dfMarkupBox imageBox = this.createImageBox(this.Owner.Atlas, attribute1.Value, style);
          if (imageBox == null)
            return;
          Vector2 vector2 = Vector2.zero;
          dfMarkupAttribute attribute2 = this.findAttribute("height");
          if (attribute2 != null)
            vector2.y = (float) dfMarkupStyle.ParseSize(attribute2.Value, (int) imageBox.Size.y);
          dfMarkupAttribute attribute3 = this.findAttribute("width");
          if (attribute3 != null)
            vector2.x = (float) dfMarkupStyle.ParseSize(attribute3.Value, (int) imageBox.Size.x);
          if ((double) vector2.sqrMagnitude <= 1.4012984643248171E-45)
            vector2 = imageBox.Size;
          else if ((double) vector2.x <= 1.4012984643248171E-45)
            vector2.x = vector2.y * (imageBox.Size.x / imageBox.Size.y);
          else if ((double) vector2.y <= 1.4012984643248171E-45)
            vector2.y = vector2.x * (imageBox.Size.y / imageBox.Size.x);
          imageBox.Size = vector2;
          imageBox.Baseline = (int) vector2.y;
          container.AddChild(imageBox);
        }
      }

      private dfMarkupStyle applyStyleAttributes(dfMarkupStyle style)
      {
        dfMarkupAttribute attribute1 = this.findAttribute("valign");
        if (attribute1 != null)
          style.VerticalAlign = dfMarkupStyle.ParseVerticalAlignment(attribute1.Value);
        dfMarkupAttribute attribute2 = this.findAttribute("color");
        if (attribute2 != null)
        {
          Color color = dfMarkupStyle.ParseColor(attribute2.Value, (Color) this.Owner.Color) with
          {
            a = style.Opacity
          };
          style.Color = color;
        }
        return style;
      }

      private dfMarkupBox createImageBox(dfAtlas atlas, string source, dfMarkupStyle style)
      {
        if (source.ToLowerInvariant().StartsWith("http://"))
          return (dfMarkupBox) null;
        if ((Object) atlas != (Object) null && atlas[source] != (dfAtlas.ItemInfo) null)
        {
          dfMarkupBoxSprite imageBox = new dfMarkupBoxSprite((dfMarkupElement) this, dfMarkupDisplayType.inline, style);
          imageBox.LoadImage(atlas, source);
          return (dfMarkupBox) imageBox;
        }
        Texture texture = dfMarkupImageCache.Load(source);
        if (!((Object) texture != (Object) null))
          return (dfMarkupBox) null;
        dfMarkupBoxTexture imageBox1 = new dfMarkupBoxTexture((dfMarkupElement) this, dfMarkupDisplayType.inline, style);
        imageBox1.LoadTexture(texture);
        return (dfMarkupBox) imageBox1;
      }
    }

}
