// Decompiled with JetBrains decompiler
// Type: FullInspector.CommentAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector
{
  [Obsolete("Use [InspectorComment] instead of [Comment]")]
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field)]
  public class CommentAttribute : Attribute, IInspectorAttributeOrder
  {
    public string Comment;
    public CommentType Type;
    public double Order = 100.0;

    public CommentAttribute(string comment)
      : this(CommentType.Info, comment)
    {
    }

    public CommentAttribute(CommentType type, string comment)
    {
      this.Type = type;
      this.Comment = comment;
    }

    double IInspectorAttributeOrder.Order => this.Order;
  }
}
