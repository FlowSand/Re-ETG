#nullable disable

[dfMarkupTagInfo("a")]
public class dfMarkupTagAnchor : dfMarkupTag
  {
    public dfMarkupTagAnchor()
      : base("a")
    {
    }

    public dfMarkupTagAnchor(dfMarkupTag original)
      : base(original)
    {
    }

    public string HRef
    {
      get
      {
        dfMarkupAttribute attribute = this.findAttribute("href");
        return attribute != null ? attribute.Value : string.Empty;
      }
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      style.TextDecoration = dfMarkupTextDecoration.Underline;
      style = this.applyTextStyleAttributes(style);
      for (int index = 0; index < this.ChildNodes.Count; ++index)
      {
        dfMarkupElement childNode = this.ChildNodes[index];
        if (childNode is dfMarkupString && (childNode as dfMarkupString).Text == "\n")
        {
          if (style.PreserveWhitespace)
            container.AddLineBreak();
        }
        else
          childNode.PerformLayout(container, style);
      }
    }
  }

