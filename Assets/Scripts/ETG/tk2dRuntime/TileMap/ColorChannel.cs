using System;

using UnityEngine;

#nullable disable
namespace tk2dRuntime.TileMap
{
    [Serializable]
    public class ColorChannel
    {
        public Color clearColor = Color.white;
        public ColorChunk[] chunks;
        public int numColumns;
        public int numRows;
        public int divX;
        public int divY;

        public ColorChannel(int width, int height, int divX, int divY)
        {
            this.Init(width, height, divX, divY);
        }

        public ColorChannel() => this.chunks = new ColorChunk[0];

        public void Init(int width, int height, int divX, int divY)
        {
            this.numColumns = (width + divX - 1) / divX;
            this.numRows = (height + divY - 1) / divY;
            this.chunks = new ColorChunk[0];
            this.divX = divX;
            this.divY = divY;
        }

        public ColorChunk FindChunkAndCoordinate(int x, int y, out int offset)
        {
            int num1 = x / this.divX;
            int num2 = y / this.divY;
            int num3 = Mathf.Clamp(num1, 0, this.numColumns - 1);
            int num4 = Mathf.Clamp(num2, 0, this.numRows - 1);
            ColorChunk chunk = this.chunks[num4 * this.numColumns + num3];
            int num5 = x - num3 * this.divX;
            int num6 = y - num4 * this.divY;
            offset = num6 * (this.divX + 1) + num5;
            return chunk;
        }

        public Color GetColor(int x, int y)
        {
            if (this.IsEmpty)
                return this.clearColor;
            int offset;
            ColorChunk chunkAndCoordinate = this.FindChunkAndCoordinate(x, y, out offset);
            return chunkAndCoordinate.colors.Length == 0 ? this.clearColor : (Color) chunkAndCoordinate.colors[offset];
        }

        private void InitChunk(ColorChunk chunk)
        {
            if (chunk.colors.Length != 0)
                return;
            chunk.colors = new Color32[(this.divX + 1) * (this.divY + 1)];
            for (int index = 0; index < chunk.colors.Length; ++index)
                chunk.colors[index] = (Color32) this.clearColor;
            chunk.colorOverrides = new Color32[(this.divX + 1) * (this.divY + 1), 4];
            for (int index = 0; index < chunk.colorOverrides.GetLength(0); ++index)
            {
                chunk.colorOverrides[index, 0] = (Color32) this.clearColor;
                chunk.colorOverrides[index, 1] = (Color32) this.clearColor;
                chunk.colorOverrides[index, 2] = (Color32) this.clearColor;
                chunk.colorOverrides[index, 3] = (Color32) this.clearColor;
            }
        }

        public void SetTileColorOverride(int x, int y, Color32 color)
        {
            if (this.IsEmpty)
                this.Create();
            int x1 = x / this.divX;
            int y1 = y / this.divY;
            ColorChunk chunk = this.GetChunk(x1, y1, true);
            int num1 = x - x1 * this.divX;
            int num2 = y - y1 * this.divY;
            int num3 = this.divX + 1;
            chunk.colorOverrides[num2 * num3 + num1, 0] = color;
            chunk.colorOverrides[num2 * num3 + num1, 1] = color;
            chunk.colorOverrides[num2 * num3 + num1, 2] = color;
            chunk.colorOverrides[num2 * num3 + num1, 3] = color;
            chunk.Dirty = true;
        }

        public void SetTileColorGradient(
            int x,
            int y,
            Color32 bottomLeft,
            Color32 bottomRight,
            Color32 topLeft,
            Color32 topRight)
        {
            if (this.IsEmpty)
                this.Create();
            int num1 = this.divX + 1;
            int x1 = x / this.divX;
            int y1 = y / this.divY;
            ColorChunk chunk = this.GetChunk(x1, y1, true);
            int num2 = x - x1 * this.divX;
            int num3 = y - y1 * this.divY;
            chunk.colorOverrides[num3 * num1 + num2, 0] = bottomLeft;
            chunk.colorOverrides[num3 * num1 + num2, 1] = bottomRight;
            chunk.colorOverrides[num3 * num1 + num2, 2] = topLeft;
            chunk.colorOverrides[num3 * num1 + num2, 3] = topRight;
            chunk.Dirty = true;
        }

        public void SetColor(int x, int y, Color color)
        {
            if (this.IsEmpty)
                this.Create();
            int num1 = this.divX + 1;
            int x1 = Mathf.Max(x - 1, 0) / this.divX;
            int y1 = Mathf.Max(y - 1, 0) / this.divY;
            ColorChunk chunk1 = this.GetChunk(x1, y1, true);
            int num2 = x - x1 * this.divX;
            int num3 = y - y1 * this.divY;
            chunk1.colors[num3 * num1 + num2] = (Color32) color;
            chunk1.Dirty = true;
            bool flag1 = false;
            bool flag2 = false;
            if (x != 0 && x % this.divX == 0 && x1 + 1 < this.numColumns)
                flag1 = true;
            if (y != 0 && y % this.divY == 0 && y1 + 1 < this.numRows)
                flag2 = true;
            if (flag1)
            {
                int x2 = x1 + 1;
                ColorChunk chunk2 = this.GetChunk(x2, y1, true);
                int num4 = x - x2 * this.divX;
                int num5 = y - y1 * this.divY;
                chunk2.colors[num5 * num1 + num4] = (Color32) color;
                chunk2.Dirty = true;
            }
            if (flag2)
            {
                int y2 = y1 + 1;
                ColorChunk chunk3 = this.GetChunk(x1, y2, true);
                int num6 = x - x1 * this.divX;
                int num7 = y - y2 * this.divY;
                chunk3.colors[num7 * num1 + num6] = (Color32) color;
                chunk3.Dirty = true;
            }
            if (!flag1 || !flag2)
                return;
            int x3 = x1 + 1;
            int y3 = y1 + 1;
            ColorChunk chunk4 = this.GetChunk(x3, y3, true);
            int num8 = x - x3 * this.divX;
            int num9 = y - y3 * this.divY;
            chunk4.colors[num9 * num1 + num8] = (Color32) color;
            chunk4.Dirty = true;
        }

        public ColorChunk GetChunk(int x, int y)
        {
            return this.chunks == null || this.chunks.Length == 0 ? (ColorChunk) null : this.chunks[y * this.numColumns + x];
        }

        public ColorChunk GetChunk(int x, int y, bool init)
        {
            if (this.chunks == null || this.chunks.Length == 0)
                return (ColorChunk) null;
            ColorChunk chunk = this.chunks[y * this.numColumns + x];
            this.InitChunk(chunk);
            return chunk;
        }

        public void ClearChunk(ColorChunk chunk)
        {
            for (int index = 0; index < chunk.colors.Length; ++index)
                chunk.colors[index] = (Color32) this.clearColor;
            for (int index = 0; index < chunk.colorOverrides.GetLength(0); ++index)
            {
                chunk.colorOverrides[index, 0] = (Color32) this.clearColor;
                chunk.colorOverrides[index, 1] = (Color32) this.clearColor;
                chunk.colorOverrides[index, 2] = (Color32) this.clearColor;
                chunk.colorOverrides[index, 3] = (Color32) this.clearColor;
            }
        }

        public void ClearDirtyFlag()
        {
            foreach (ColorChunk chunk in this.chunks)
                chunk.Dirty = false;
        }

        public void Clear(Color color)
        {
            this.clearColor = color;
            foreach (ColorChunk chunk in this.chunks)
                this.ClearChunk(chunk);
            this.Optimize();
        }

        public void Delete() => this.chunks = new ColorChunk[0];

        public void Create()
        {
            this.chunks = new ColorChunk[this.numColumns * this.numRows];
            for (int index = 0; index < this.chunks.Length; ++index)
                this.chunks[index] = new ColorChunk();
        }

        private void Optimize(ColorChunk chunk)
        {
            bool flag = true;
            Color32 clearColor = (Color32) this.clearColor;
            foreach (Color32 color in chunk.colors)
            {
                if ((int) color.r != (int) clearColor.r || (int) color.g != (int) clearColor.g || (int) color.b != (int) clearColor.b || (int) color.a != (int) clearColor.a)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                Color32[,] colorOverrides = chunk.colorOverrides;
                int length1 = colorOverrides.GetLength(0);
                int length2 = colorOverrides.GetLength(1);
                for (int index1 = 0; index1 < length1; ++index1)
                {
                    for (int index2 = 0; index2 < length2; ++index2)
                    {
                        Color32 color32 = colorOverrides[index1, index2];
                        if ((int) color32.r != (int) clearColor.r || (int) color32.g != (int) clearColor.g || (int) color32.b != (int) clearColor.b || (int) color32.a != (int) clearColor.a)
                        {
                            flag = false;
                            goto label_14;
                        }
                    }
                }
            }
    label_14:
            if (!flag)
                return;
            chunk.colors = new Color32[0];
            chunk.colorOverrides = new Color32[0, 0];
        }

        public void Optimize()
        {
            foreach (ColorChunk chunk in this.chunks)
                this.Optimize(chunk);
        }

        public bool IsEmpty => this.chunks.Length == 0;

        public int NumActiveChunks
        {
            get
            {
                int numActiveChunks = 0;
                foreach (ColorChunk chunk in this.chunks)
                {
                    if (chunk != null && chunk.colors != null && chunk.colors.Length > 0)
                        ++numActiveChunks;
                }
                return numActiveChunks;
            }
        }
    }
}
