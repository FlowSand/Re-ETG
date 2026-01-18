using UnityEngine;

#nullable disable

[dfMarkupTagInfo("li")]
public class dfMarkupTagListItem : dfMarkupTag
  {
    public dfMarkupTagListItem()
      : base("li")
    {
    }

    public dfMarkupTagListItem(dfMarkupTag original)
      : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      if (this.ChildNodes.Count == 0)
        return;
      float x1 = container.Size.x;
      dfMarkupBox box1 = new dfMarkupBox((dfMarkupElement) this, dfMarkupDisplayType.listItem, style);
      box1.Margins.top = 10;
      container.AddChild(box1);
      if (!(this.Parent is dfMarkupTagList parent))
      {
        base._PerformLayoutImpl(container, style);
      }
      else
      {
        style.VerticalAlign = dfMarkupVerticalAlign.Baseline;
        string text = "•";
        if (parent.TagName == "ol")
          text = container.Children.Count.ToString() + ".";
        dfMarkupBoxText box2 = dfMarkupBoxText.Obtain((dfMarkupElement) this, dfMarkupDisplayType.inlineBlock, style with
        {
          VerticalAlign = dfMarkupVerticalAlign.Baseline,
          Align = dfMarkupTextAlign.Right
        });
        box2.SetText(text);
        box2.Width = parent.BulletWidth;
        box2.Margins.left = style.FontSize * 2;
        box1.AddChild((dfMarkupBox) box2);
        dfMarkupBox dfMarkupBox = new dfMarkupBox((dfMarkupElement) this, dfMarkupDisplayType.inlineBlock, style);
        int fontSize = style.FontSize;
        float x2 = x1 - box2.Size.x - (float) box2.Margins.left - (float) fontSize;
        dfMarkupBox.Size = new Vector2(x2, (float) fontSize);
        dfMarkupBox.Margins.left = (int) ((double) style.FontSize * 0.5);
        box1.AddChild(dfMarkupBox);
        for (int index = 0; index < this.ChildNodes.Count; ++index)
          this.ChildNodes[index].PerformLayout(dfMarkupBox, style);
        dfMarkupBox.FitToContents();
        if (dfMarkupBox.Parent != null)
          dfMarkupBox.Parent.FitToContents();
        box1.FitToContents();
      }
    }
  }

