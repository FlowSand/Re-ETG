// Decompiled with JetBrains decompiler
// Type: Dungeonator.SpiralPointLayoutHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace Dungeonator;

public class SpiralPointLayoutHandler
{
  public static Queue<IntVector2> spiralOffsets;
  public static int nextElementIndex;
  public static IntVector2 resultOffset;
  public static int currentResultElementIndex = -1;
  private SemioticLayoutManager canvas;
  private SemioticLayoutManager otherCanvas;
  private IntVector2 otherCanvasOffset;
  private int currentElementIndex = -1;

  public SpiralPointLayoutHandler(SemioticLayoutManager c1, SemioticLayoutManager c2, int id)
  {
    this.canvas = c1;
    this.otherCanvas = c2;
    this.currentElementIndex = -1;
  }

  public void ThreadRun()
  {
    while (SpiralPointLayoutHandler.currentResultElementIndex == -1)
    {
      lock ((object) SpiralPointLayoutHandler.spiralOffsets)
      {
        if (SpiralPointLayoutHandler.spiralOffsets.Count > 0)
        {
          this.otherCanvasOffset = SpiralPointLayoutHandler.spiralOffsets.Dequeue();
          this.currentElementIndex = SpiralPointLayoutHandler.nextElementIndex;
          ++SpiralPointLayoutHandler.nextElementIndex;
        }
        else
          this.currentElementIndex = -1;
      }
      if (this.currentElementIndex < 0)
        break;
      this.CheckRectangleDecompositionCollisions();
    }
  }

  public void CheckRectangleDecompositionCollisions()
  {
    bool flag = true;
    for (int index1 = 0; index1 < this.otherCanvas.RectangleDecomposition.Count; ++index1)
    {
      Tuple<IntVector2, IntVector2> tuple1 = this.otherCanvas.RectangleDecomposition[index1];
      for (int index2 = 0; index2 < this.canvas.RectangleDecomposition.Count; ++index2)
      {
        Tuple<IntVector2, IntVector2> tuple2 = this.canvas.RectangleDecomposition[index2];
        if (IntVector2.AABBOverlap(tuple1.First + this.otherCanvasOffset, tuple1.Second, tuple2.First, tuple2.Second))
        {
          flag = false;
          break;
        }
      }
      if (!flag)
        break;
    }
    if (!flag)
      return;
    lock ((object) SpiralPointLayoutHandler.spiralOffsets)
    {
      if (SpiralPointLayoutHandler.currentResultElementIndex != -1 && this.currentElementIndex >= SpiralPointLayoutHandler.currentResultElementIndex)
        return;
      SpiralPointLayoutHandler.spiralOffsets.Clear();
      SpiralPointLayoutHandler.currentResultElementIndex = this.currentElementIndex;
      SpiralPointLayoutHandler.resultOffset = this.otherCanvasOffset;
    }
  }
}
