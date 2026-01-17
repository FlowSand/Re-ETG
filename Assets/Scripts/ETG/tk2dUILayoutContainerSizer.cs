// Decompiled with JetBrains decompiler
// Type: tk2dUILayoutContainerSizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("2D Toolkit/UI/Core/tk2dUILayoutContainerSizer")]
public class tk2dUILayoutContainerSizer : tk2dUILayoutContainer
{
  public bool horizontal;
  public bool expand = true;
  public Vector2 margin = Vector2.zero;
  public float spacing;

  protected override void DoChildLayout()
  {
    int count = this.layoutItems.Count;
    if (count == 0)
      return;
    float num1 = (float) ((double) this.bMax.x - (double) this.bMin.x - 2.0 * (double) this.margin.x);
    float num2 = (float) ((double) this.bMax.y - (double) this.bMin.y - 2.0 * (double) this.margin.y);
    float num3 = (float) ((!this.horizontal ? (double) num2 : (double) num1) - (double) this.spacing * (double) (count - 1));
    float num4 = 1f;
    float num5 = num3;
    float num6 = 0.0f;
    float[] numArray = new float[count];
    for (int index = 0; index < count; ++index)
    {
      tk2dUILayoutItem layoutItem = this.layoutItems[index];
      if (layoutItem.fixedSize)
      {
        numArray[index] = !this.horizontal ? layoutItem.layout.bMax.y - layoutItem.layout.bMin.y : layoutItem.layout.bMax.x - layoutItem.layout.bMin.x;
        num5 -= numArray[index];
      }
      else if ((double) layoutItem.fillPercentage > 0.0)
      {
        float num7 = (float) ((double) num4 * (double) layoutItem.fillPercentage / 100.0);
        numArray[index] = num3 * num7;
        num5 -= numArray[index];
        num4 -= num7;
      }
      else
        num6 += layoutItem.sizeProportion;
    }
    for (int index = 0; index < count; ++index)
    {
      tk2dUILayoutItem layoutItem = this.layoutItems[index];
      if (!layoutItem.fixedSize && (double) layoutItem.fillPercentage <= 0.0)
        numArray[index] = num5 * layoutItem.sizeProportion / num6;
    }
    Vector3 zero1 = Vector3.zero;
    Vector3 zero2 = Vector3.zero;
    float num8 = 0.0f;
    Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
    if (this.horizontal)
      this.innerSize = new Vector2((float) (2.0 * (double) this.margin.x + (double) this.spacing * (double) (count - 1)), this.bMax.y - this.bMin.y);
    else
      this.innerSize = new Vector2(this.bMax.x - this.bMin.x, (float) (2.0 * (double) this.margin.y + (double) this.spacing * (double) (count - 1)));
    for (int index = 0; index < count; ++index)
    {
      tk2dUILayoutItem layoutItem = this.layoutItems[index];
      Matrix4x4 matrix4x4 = layoutItem.gameObj.transform.localToWorldMatrix * this.transform.worldToLocalMatrix;
      if (this.horizontal)
      {
        if (this.expand)
        {
          zero1.y = this.bMin.y + this.margin.y;
          zero2.y = this.bMax.y - this.margin.y;
        }
        else
        {
          zero1.y = matrix4x4.MultiplyPoint(layoutItem.layout.bMin).y;
          zero2.y = matrix4x4.MultiplyPoint(layoutItem.layout.bMax).y;
        }
        zero1.x = this.bMin.x + this.margin.x + num8;
        zero2.x = zero1.x + numArray[index];
      }
      else
      {
        if (this.expand)
        {
          zero1.x = this.bMin.x + this.margin.x;
          zero2.x = this.bMax.x - this.margin.x;
        }
        else
        {
          zero1.x = matrix4x4.MultiplyPoint(layoutItem.layout.bMin).x;
          zero2.x = matrix4x4.MultiplyPoint(layoutItem.layout.bMax).x;
        }
        zero2.y = this.bMax.y - this.margin.y - num8;
        zero1.y = zero2.y - numArray[index];
      }
      layoutItem.layout.SetBounds(localToWorldMatrix.MultiplyPoint(zero1), localToWorldMatrix.MultiplyPoint(zero2));
      num8 += numArray[index] + this.spacing;
      if (this.horizontal)
        this.innerSize.x += numArray[index];
      else
        this.innerSize.y += numArray[index];
    }
  }
}
