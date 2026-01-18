using System;

using UnityEngine;

#nullable disable

[Serializable]
public class dfAnchorMargins
    {
        [SerializeField]
        public float left;
        [SerializeField]
        public float top;
        [SerializeField]
        public float right;
        [SerializeField]
        public float bottom;

        public override string ToString()
        {
            return $"[L:{this.left},T:{this.top},R:{this.right},B:{this.bottom}]";
        }
    }

