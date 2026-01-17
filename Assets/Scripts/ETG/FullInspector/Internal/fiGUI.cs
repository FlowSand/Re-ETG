// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiGUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal;

public static class fiGUI
{
  private static readonly List<float> s_regionWidths = new List<float>();
  private static readonly Stack<float> s_savedLabelWidths = new Stack<float>();

  public static float PushLabelWidth(GUIContent controlLabel, float controlWidth)
  {
    fiGUI.s_regionWidths.Add(controlWidth);
    fiGUI.s_savedLabelWidths.Push(controlWidth);
    return fiGUI.ComputeActualLabelWidth(fiGUI.s_regionWidths[0], controlLabel, controlWidth);
  }

  public static float PopLabelWidth()
  {
    fiGUI.s_regionWidths.RemoveAt(fiGUI.s_regionWidths.Count - 1);
    return fiGUI.s_savedLabelWidths.Pop();
  }

  public static float ComputeActualLabelWidth(
    float inspectorWidth,
    GUIContent controlLabel,
    float controlWidth)
  {
    float num = inspectorWidth - controlWidth;
    return Mathf.Clamp(Mathf.Max(inspectorWidth * fiSettings.LabelWidthPercentage - fiSettings.LabelWidthOffset, 120f) - num, Mathf.Max(fiLateBindings.EditorStyles.label.CalcSize(controlLabel).x, fiSettings.LabelWidthMin), fiSettings.LabelWidthMax);
  }
}
