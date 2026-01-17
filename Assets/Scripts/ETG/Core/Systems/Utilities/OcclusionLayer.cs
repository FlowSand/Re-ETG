// Decompiled with JetBrains decompiler
// Type: OcclusionLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class OcclusionLayer
    {
      public Color occludedColor;
      public int cachedX;
      public int cachedY;
      private GameManager m_gameManagerCached;
      private PlayerController[] m_allPlayersCached;
      private Pixelator m_pixelatorCached;
      private bool m_playerOneDead;
      private bool m_playerTwoDead;
      protected Texture2D m_occlusionTexture;
      protected int textureMultiplier = 1;
      protected float[] KERNEL = new float[5]
      {
        0.12f,
        0.25f,
        0.3f,
        0.25f,
        0.12f
      };
      protected Color[] m_colorCache;

      public Texture2D SourceOcclusionTexture
      {
        get => this.m_occlusionTexture;
        set => this.m_occlusionTexture = value;
      }

      protected float GetCellOcclusion(int x0, int y0, DungeonData d)
      {
        float a = d.cellData[x0][y0] != null ? d.cellData[x0][y0].occlusionData.cellOcclusion : 1f;
        if (this.m_pixelatorCached.UseTexturedOcclusion || x0 < 2 || y0 < 2 || x0 >= d.Width - 2 || y0 >= d.Height - 2)
          return a;
        float num1 = 0.0f;
        float num2 = 0.0f;
        for (int index1 = -2; index1 <= 2; ++index1)
        {
          for (int index2 = -2; index2 <= 2; ++index2)
          {
            float num3 = this.KERNEL[index1 + 2] * this.KERNEL[index2 + 2];
            float num4 = d.cellData[x0 + index1][y0 + index2] != null ? d.cellData[x0 + index1][y0 + index2].occlusionData.cellOcclusion * num3 : 1f * num3;
            num1 += num4;
            num2 += num3;
          }
        }
        return Mathf.Min(a, num1 / num2);
      }

      protected float GetGValueForCell(int x0, int y0, DungeonData d)
      {
        float gvalueForCell = 0.0f;
        if (x0 < 0 || x0 >= d.Width || y0 < 0 || y0 >= d.Height)
          return gvalueForCell;
        CellData cellData = d[x0, y0];
        if (cellData == null)
          return gvalueForCell;
        bool texturedOcclusion = this.m_pixelatorCached.UseTexturedOcclusion;
        if (cellData.type == CellType.FLOOR || cellData.type == CellType.PIT || cellData.IsLowerFaceWall() || cellData.IsUpperFacewall() && !texturedOcclusion)
        {
          if (cellData.nearestRoom.visibility == RoomHandler.VisibilityStatus.CURRENT)
            gvalueForCell = (float) (1.0 * (1.0 - (double) cellData.occlusionData.minCellOccluionHistory));
          else if (cellData.nearestRoom.hasEverBeenVisited && cellData.nearestRoom.visibility != RoomHandler.VisibilityStatus.REOBSCURED)
            gvalueForCell = 1f;
        }
        return gvalueForCell;
      }

      protected float GetRValueForCell(int x0, int y0, DungeonData d)
      {
        float rvalueForCell = 0.0f;
        if (this.m_pixelatorCached.UseTexturedOcclusion || x0 < 0 || x0 >= d.Width || y0 < 0 || y0 >= d.Height)
          return rvalueForCell;
        CellData cellData = d[x0, y0];
        if (cellData == null || cellData.isExitCell || cellData.type == CellType.WALL && !cellData.IsAnyFaceWall() || y0 - 2 >= 0 && d[x0, y0 - 2] != null && d[x0, y0 - 2].isExitCell)
          return rvalueForCell;
        RoomHandler roomHandler = d[x0, y0].parentRoom ?? d[x0, y0].nearestRoom;
        bool flag = false;
        if (roomHandler != null)
        {
          for (int index = 0; index < this.m_allPlayersCached.Length; ++index)
          {
            if ((index != 0 || !this.m_playerOneDead) && (index != 1 || !this.m_playerTwoDead) && this.m_allPlayersCached[index].CurrentRoom != null && this.m_allPlayersCached[index].CurrentRoom.connectedRooms != null && this.m_allPlayersCached[index].CurrentRoom.connectedRooms.Contains(roomHandler))
              flag = true;
          }
        }
        if (x0 < 1 || x0 > d.Width - 2 || y0 < 3 || y0 > d.Height - 2 || roomHandler != null && roomHandler.visibility != RoomHandler.VisibilityStatus.OBSCURED && roomHandler.visibility != RoomHandler.VisibilityStatus.REOBSCURED || flag && (cellData.isExitNonOccluder || cellData.isExitCell || y0 > 1 && d[x0, y0 - 1] != null && d[x0, y0 - 1].isExitCell || y0 > 2 && d[x0, y0 - 2] != null && d[x0, y0 - 2].isExitCell || y0 > 3 && d[x0, y0 - 3] != null && d[x0, y0 - 3].isExitCell || x0 > 1 && d[x0 - 1, y0] != null && d[x0 - 1, y0].isExitCell || x0 < d.Width - 1 && d[x0 + 1, y0] != null && d[x0 + 1, y0].isExitCell))
          return rvalueForCell;
        rvalueForCell = 1f;
        return rvalueForCell;
      }

      protected Color GetInterpolatedValueAtPoint(
        int baseX,
        int baseY,
        float worldX,
        float worldY,
        DungeonData d)
      {
        int num1 = baseX + Mathf.FloorToInt(worldX);
        int num2 = baseY + Mathf.FloorToInt(worldY);
        int num3 = baseX + (int) worldX;
        int num4 = baseY + (int) worldY;
        float rvalueForCell = this.GetRValueForCell(num3, num4, d);
        float gvalueForCell = this.GetGValueForCell(num3, num4, d);
        if (!d.CheckInBounds(num3, num4))
          return new Color(rvalueForCell, gvalueForCell, 0.0f, 0.0f);
        float num5 = Mathf.Clamp01(this.GetCellOcclusion(num3, num4, d));
        float a = (float) (1.0 - (double) num5 * (double) num5);
        return new Color(rvalueForCell, gvalueForCell, 0.0f, a);
      }

      public Texture2D GenerateOcclusionTexture(int baseX, int baseY, DungeonData d)
      {
        this.m_gameManagerCached = GameManager.Instance;
        this.m_pixelatorCached = Pixelator.Instance;
        this.m_allPlayersCached = GameManager.Instance.AllPlayers;
        this.m_playerOneDead = !(bool) (Object) this.m_gameManagerCached.PrimaryPlayer || this.m_gameManagerCached.PrimaryPlayer.healthHaver.IsDead;
        if (this.m_gameManagerCached.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          this.m_playerTwoDead = !(bool) (Object) this.m_gameManagerCached.SecondaryPlayer || this.m_gameManagerCached.SecondaryPlayer.healthHaver.IsDead;
        int num1 = this.m_pixelatorCached.CurrentMacroResolutionX / 16 /*0x10*/ + 4;
        int num2 = this.m_pixelatorCached.CurrentMacroResolutionY / 16 /*0x10*/ + 4;
        int width = num1 * this.textureMultiplier;
        int height = num2 * this.textureMultiplier;
        if ((Object) this.m_occlusionTexture == (Object) null || this.m_occlusionTexture.width != width || this.m_occlusionTexture.height != height)
        {
          if ((Object) this.m_occlusionTexture != (Object) null)
          {
            this.m_occlusionTexture.Resize(width, height);
          }
          else
          {
            this.m_occlusionTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            this.m_occlusionTexture.filterMode = FilterMode.Bilinear;
            this.m_occlusionTexture.wrapMode = TextureWrapMode.Clamp;
          }
        }
        if (this.m_colorCache == null || this.m_colorCache.Length != width * height)
          this.m_colorCache = new Color[width * height];
        this.cachedX = baseX;
        this.cachedY = baseY;
        if (!this.m_gameManagerCached.IsLoadingLevel)
        {
          for (int index1 = 0; index1 < width; ++index1)
          {
            for (int index2 = 0; index2 < height; ++index2)
            {
              int index3 = index2 * width + index1;
              float worldX = (float) index1 / (float) this.textureMultiplier;
              float worldY = (float) index2 / (float) this.textureMultiplier;
              this.m_colorCache[index3] = this.GetInterpolatedValueAtPoint(baseX, baseY, worldX, worldY, d);
            }
          }
        }
        this.m_occlusionTexture.SetPixels(this.m_colorCache);
        this.m_occlusionTexture.Apply();
        return this.m_occlusionTexture;
      }
    }

}
