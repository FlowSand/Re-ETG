using UnityEngine;

#nullable disable

public class TogglesPropertyAttribute : PropertyAttribute
    {
        public string PropertyName;
        public string Label;

        public TogglesPropertyAttribute(string propertyName, string label = null)
        {
            this.PropertyName = propertyName;
            this.Label = label;
        }
    }

