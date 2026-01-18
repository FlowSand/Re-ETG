using UnityEngine;

#nullable disable

public class CheckSpriteAttribute : PropertyAttribute
    {
        public string sprite;

        public CheckSpriteAttribute(string sprite = null) => this.sprite = sprite;
    }

