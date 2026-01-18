using System.Collections.Generic;

#nullable disable

[dfMarkupTagInfo("span")]
public class dfMarkupTagSpan : dfMarkupTag
  {
    private static Queue<dfMarkupTagSpan> objectPool = new Queue<dfMarkupTagSpan>();

    public dfMarkupTagSpan()
      : base("span")
    {
    }

    public dfMarkupTagSpan(dfMarkupTag original)
      : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      style = this.applyTextStyleAttributes(style);
      dfMarkupBox dfMarkupBox = container;
      dfMarkupAttribute attribute = this.findAttribute("margin");
      if (attribute != null)
      {
        dfMarkupBox = new dfMarkupBox((dfMarkupElement) this, dfMarkupDisplayType.inlineBlock, style)
        {
          Margins = dfMarkupBorders.Parse(attribute.Value)
        };
        dfMarkupBox.Margins.top = 0;
        dfMarkupBox.Margins.bottom = 0;
        container.AddChild(dfMarkupBox);
      }
      for (int index = 0; index < this.ChildNodes.Count; ++index)
      {
        dfMarkupElement childNode = this.ChildNodes[index];
        if (childNode is dfMarkupString && (childNode as dfMarkupString).Text == "\n")
        {
          if (style.PreserveWhitespace)
            dfMarkupBox.AddLineBreak();
        }
        else
          childNode.PerformLayout(dfMarkupBox, style);
      }
    }

    internal static dfMarkupTagSpan Obtain()
    {
      return dfMarkupTagSpan.objectPool.Count > 0 ? dfMarkupTagSpan.objectPool.Dequeue() : new dfMarkupTagSpan();
    }

    internal override void Release()
    {
      base.Release();
      dfMarkupTagSpan.objectPool.Enqueue(this);
    }
  }

