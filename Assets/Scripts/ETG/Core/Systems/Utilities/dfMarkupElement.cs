// Decompiled with JetBrains decompiler
// Type: dfMarkupElement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

