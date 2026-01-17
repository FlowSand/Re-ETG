// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiCommentUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
  public static class fiCommentUtility
  {
    public static int GetCommentHeight(string comment, CommentType commentType)
    {
      int val2 = 38;
      if (commentType == CommentType.None)
        val2 = 17;
      return Math.Max((int) ((GUIStyle) "HelpBox").CalcHeight(new GUIContent(comment), (float) Screen.width), val2);
    }
  }
}
