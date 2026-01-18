using System;

#nullable disable
namespace FullInspector
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field)]
    public class InspectorCommentAttribute : Attribute, IInspectorAttributeOrder
    {
        public string Comment;
        public CommentType Type;
        public double Order = 100.0;

        public InspectorCommentAttribute(string comment)
            : this(fiSettings.DefaultCommentType, comment)
        {
        }

        public InspectorCommentAttribute(CommentType type, string comment)
        {
            this.Type = type;
            this.Comment = comment;
        }

        double IInspectorAttributeOrder.Order => this.Order;
    }
}
