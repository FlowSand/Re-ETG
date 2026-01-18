using System;

using UnityEngine;

#nullable disable
namespace tk2dRuntime.TileMap
{
    [Serializable]
    public class ColorChunk
    {
        public Color32[] colors;
        public Color32[,] colorOverrides;

        public ColorChunk()
        {
            this.colors = new Color32[0];
            this.colorOverrides = new Color32[0, 0];
        }

        public bool Dirty { get; set; }

        public bool Empty => this.colors.Length == 0;
    }
}
