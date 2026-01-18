using System;

#nullable disable
namespace tk2dRuntime.TileMap
{
    [Serializable]
    public class LayerSprites
    {
        public int[] spriteIds;

        public LayerSprites() => this.spriteIds = new int[0];
    }
}
