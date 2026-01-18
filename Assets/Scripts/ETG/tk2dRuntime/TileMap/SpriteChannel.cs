using System;

#nullable disable
namespace tk2dRuntime.TileMap
{
  [Serializable]
  public class SpriteChannel
  {
    public SpriteChunk[] chunks;

    public SpriteChannel() => this.chunks = new SpriteChunk[0];
  }
}
