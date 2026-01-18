// Decompiled with JetBrains decompiler
// Type: GameUIUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class GameUIUtility
  {
    public static float GetCurrentTK2D_DFScale(dfGUIManager manager)
    {
      return Pixelator.Instance.CurrentTileScale * 16f * manager.PixelsToUnits();
    }

    public static Vector2 TK2DtoDF(Vector2 input, float p2u)
    {
      float num = 64f * p2u;
      return input * num;
    }

    public static Vector2 DFtoTK2D(Vector2 input, float p2u)
    {
      float num = 64f * p2u;
      return input / num;
    }

    public static Vector2 TK2DtoDF(Vector2 input)
    {
      return input * Pixelator.Instance.ScaleTileScale * 16f * GameUIRoot.Instance.PixelsToUnits();
    }

    public static Vector2 QuantizeUIPosition(Vector2 input)
    {
      float currentTileScale = Pixelator.Instance.CurrentTileScale;
      return new Vector2((float) Mathf.RoundToInt(input.x / currentTileScale * currentTileScale), (float) Mathf.RoundToInt(input.y / currentTileScale * currentTileScale));
    }
  }

