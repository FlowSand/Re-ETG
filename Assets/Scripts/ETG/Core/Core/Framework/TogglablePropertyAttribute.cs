using UnityEngine;

#nullable disable

public class TogglablePropertyAttribute : PropertyAttribute
    {
        public string TogglePropertyName;
        public string Label;

        public TogglablePropertyAttribute(string togglePropertyName, string label = null)
        {
            this.TogglePropertyName = togglePropertyName;
            this.Label = label;
        }
    }

