using System.Collections.Generic;

#nullable disable

  public abstract class dfMarkupElement
  {
    public dfMarkupElement() => this.ChildNodes = new List<dfMarkupElement>();

    public dfMarkupElement Parent { get; protected set; }

    protected List<dfMarkupElement> ChildNodes { get; private set; }

    public void AddChildNode(dfMarkupElement node)
    {
      node.Parent = this;
      this.ChildNodes.Add(node);
    }

    public void PerformLayout(dfMarkupBox container, dfMarkupStyle style)
    {
      this._PerformLayoutImpl(container, style);
    }

    internal virtual void Release()
    {
      this.Parent = (dfMarkupElement) null;
      this.ChildNodes.Clear();
    }

    protected abstract void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style);
  }

