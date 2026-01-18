using UnityEngine;

#nullable disable
namespace FullInspector
{
    public class fiGUIContent
    {
        public static fiGUIContent Empty = new fiGUIContent();
        private string _text;
        private string _tooltip;
        private Texture _image;

        public fiGUIContent()
            : this(string.Empty, string.Empty, (Texture) null)
        {
        }

        public fiGUIContent(string text)
            : this(text, string.Empty, (Texture) null)
        {
        }

        public fiGUIContent(string text, string tooltip)
            : this(text, tooltip, (Texture) null)
        {
        }

        public fiGUIContent(string text, string tooltip, Texture image)
        {
            this._text = text;
            this._tooltip = tooltip;
            this._image = image;
        }

        public fiGUIContent(Texture image)
            : this(string.Empty, string.Empty, image)
        {
        }

        public fiGUIContent(Texture image, string tooltip)
            : this(string.Empty, tooltip, image)
        {
        }

        public GUIContent AsGUIContent => new GUIContent(this._text, this._image, this._tooltip);

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(this._text) && string.IsNullOrEmpty(this._tooltip) && !((Object) this._image != (Object) null);
            }
        }

        public static implicit operator GUIContent(fiGUIContent label)
        {
            return label == null ? GUIContent.none : label.AsGUIContent;
        }

        public static implicit operator fiGUIContent(string text)
        {
            return new fiGUIContent() { _text = text };
        }

        public static implicit operator fiGUIContent(GUIContent label)
        {
            return new fiGUIContent()
            {
                _text = label.text,
                _tooltip = label.tooltip,
                _image = label.image
            };
        }
    }
}
