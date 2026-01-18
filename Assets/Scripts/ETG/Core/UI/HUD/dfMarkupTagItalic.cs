using UnityEngine;

#nullable disable

[dfMarkupTagInfo("i")]
[dfMarkupTagInfo("em")]
public class dfMarkupTagItalic : dfMarkupTag
    {
        public dfMarkupTagItalic()
            : base("i")
        {
        }

        public dfMarkupTagItalic(dfMarkupTag original)
            : base(original)
        {
        }

        protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
        {
            style = this.applyTextStyleAttributes(style);
            if (style.FontStyle == FontStyle.Normal)
                style.FontStyle = FontStyle.Italic;
            else if (style.FontStyle == FontStyle.Bold)
                style.FontStyle = FontStyle.BoldAndItalic;
            base._PerformLayoutImpl(container, style);
        }
    }

