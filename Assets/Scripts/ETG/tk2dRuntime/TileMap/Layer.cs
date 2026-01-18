using System;
using UnityEngine;

#nullable disable
namespace tk2dRuntime.TileMap
{
  [Serializable]
  public class Layer
  {
    public int hash;
    public SpriteChannel spriteChannel;
    private const int tileMask = 16777215 /*0xFFFFFF*/;
    private const int flagMask = -16777216 /*0xFF000000*/;
    public int width;
    public int height;
    public int numColumns;
    public int numRows;
    public int divX;
    public int divY;
    public GameObject gameObject;

    public Layer(int hash, int width, int height, int divX, int divY)
    {
      this.spriteChannel = new SpriteChannel();
      this.Init(hash, width, height, divX, divY);
    }

    public void Init(int hash, int width, int height, int divX, int divY)
    {
      this.divX = divX;
      this.divY = divY;
      this.hash = hash;
      this.numColumns = (width + divX - 1) / divX;
      this.numRows = (height + divY - 1) / divY;
      this.width = width;
      this.height = height;
      this.spriteChannel.chunks = new SpriteChunk[this.numColumns * this.numRows];
      for (int index1 = 0; index1 < this.numRows; ++index1)
      {
        for (int index2 = 0; index2 < this.numColumns; ++index2)
        {
          int sX = index2 * divX;
          int eX = (index2 + 1) * divX;
          int sY = index1 * divY;
          int eY = (index1 + 1) * divY;
          this.spriteChannel.chunks[index1 * this.numColumns + index2] = new SpriteChunk(sX, sY, eX, eY);
        }
      }
    }

    public bool IsEmpty => this.spriteChannel.chunks.Length == 0;

    public void Create()
    {
      this.spriteChannel.chunks = new SpriteChunk[this.numColumns * this.numRows];
    }

    public int[] GetChunkData(int x, int y) => this.GetChunk(x, y).spriteIds;

    public SpriteChunk GetChunk(int x, int y) => this.spriteChannel.chunks[y * this.numColumns + x];

    private SpriteChunk FindChunkAndCoordinate(int x, int y, out int offset)
    {
      int num1 = x / this.divX;
      int num2 = y / this.divY;
      SpriteChunk chunk = this.spriteChannel.chunks[num2 * this.numColumns + num1];
      int num3 = x - num1 * this.divX;
      int num4 = y - num2 * this.divY;
      offset = num4 * this.divX + num3;
      return chunk;
    }

    private bool GetRawTileValue(int x, int y, ref int value)
    {
      int offset;
      SpriteChunk chunkAndCoordinate = this.FindChunkAndCoordinate(x, y, out offset);
      if (chunkAndCoordinate.spriteIds == null || chunkAndCoordinate.spriteIds.Length == 0)
        return false;
      value = chunkAndCoordinate.spriteIds[offset];
      return true;
    }

    private void SetRawTileValue(int x, int y, int value)
    {
      int offset;
      SpriteChunk chunkAndCoordinate = this.FindChunkAndCoordinate(x, y, out offset);
      if (chunkAndCoordinate == null)
        return;
      this.CreateChunk(chunkAndCoordinate);
      chunkAndCoordinate.spriteIds[offset] = value;
      chunkAndCoordinate.Dirty = true;
    }

    public void DestroyGameData(tk2dTileMap tilemap)
    {
      foreach (SpriteChunk chunk in this.spriteChannel.chunks)
      {
        if (chunk.HasGameData)
        {
          chunk.DestroyColliderData(tilemap);
          chunk.DestroyGameData(tilemap);
        }
      }
    }

    public int GetTile(int x, int y)
    {
      int num = 0;
      return this.GetRawTileValue(x, y, ref num) && num != -1 ? num & 16777215 /*0xFFFFFF*/ : -1;
    }

    public tk2dTileFlags GetTileFlags(int x, int y)
    {
      int num = 0;
      return this.GetRawTileValue(x, y, ref num) && num != -1 ? (tk2dTileFlags) (num & -16777216 /*0xFF000000*/) : tk2dTileFlags.None;
    }

    public int GetRawTile(int x, int y)
    {
      int num = 0;
      return this.GetRawTileValue(x, y, ref num) ? num : -1;
    }

    public void SetTile(int x, int y, int tile)
    {
      tk2dTileFlags tileFlags = this.GetTileFlags(x, y);
      int num = tile != -1 ? (int) ((tk2dTileFlags) tile | tileFlags) : -1;
      this.SetRawTileValue(x, y, num);
    }

    public void SetTileFlags(int x, int y, tk2dTileFlags flags)
    {
      int tile = this.GetTile(x, y);
      if (tile == -1)
        return;
      int num = (int) ((tk2dTileFlags) tile | flags);
      this.SetRawTileValue(x, y, num);
    }

    public void ClearTile(int x, int y) => this.SetTile(x, y, -1);

    public void SetRawTile(int x, int y, int rawTile) => this.SetRawTileValue(x, y, rawTile);

    public void CreateOverrideChunk(SpriteChunk chunk)
    {
      if (chunk.spriteIds == null || chunk.spriteIds.Length == 0)
        chunk.spriteIds = new int[chunk.Width * chunk.Height];
      int num = 0;
      for (int index1 = 0; index1 < chunk.Width; ++index1)
      {
        for (int index2 = 0; index2 < chunk.Height; ++index2)
        {
          IntVector2 intVector2 = new IntVector2(chunk.startX + index1, chunk.startY + index2);
          int offset = 0;
          SpriteChunk chunkAndCoordinate = this.FindChunkAndCoordinate(intVector2.x, intVector2.y, out offset);
          if (offset >= 0 && offset < chunkAndCoordinate.spriteIds.Length)
          {
            chunk.spriteIds[index2 * chunk.Width + index1] = chunkAndCoordinate.spriteIds[offset];
            ++num;
          }
          else
            chunk.spriteIds[index2 * chunk.Width + index1] = -1;
        }
      }
      if (num != 0)
        return;
      chunk.spriteIds = new int[0];
    }

    private void CreateChunk(SpriteChunk chunk)
    {
      if (chunk.spriteIds != null && chunk.spriteIds.Length != 0)
        return;
      chunk.spriteIds = new int[this.divX * this.divY];
      for (int index = 0; index < this.divX * this.divY; ++index)
        chunk.spriteIds[index] = -1;
    }

    private void Optimize(SpriteChunk chunk)
    {
      bool flag = true;
      foreach (int spriteId in chunk.spriteIds)
      {
        if (spriteId != -1)
        {
          flag = false;
          break;
        }
      }
      if (!flag)
        return;
      chunk.spriteIds = new int[0];
    }

    public void Optimize()
    {
      foreach (SpriteChunk chunk in this.spriteChannel.chunks)
        this.Optimize(chunk);
    }

    public void OptimizeIncremental()
    {
      foreach (SpriteChunk chunk in this.spriteChannel.chunks)
      {
        if (chunk.Dirty)
          this.Optimize(chunk);
      }
    }

    public void ClearDirtyFlag()
    {
      foreach (SpriteChunk chunk in this.spriteChannel.chunks)
        chunk.Dirty = false;
    }

    public int NumActiveChunks
    {
      get
      {
        int numActiveChunks = 0;
        foreach (SpriteChunk chunk in this.spriteChannel.chunks)
        {
          if (!chunk.IsEmpty)
            ++numActiveChunks;
        }
        return numActiveChunks;
      }
    }
  }
}
