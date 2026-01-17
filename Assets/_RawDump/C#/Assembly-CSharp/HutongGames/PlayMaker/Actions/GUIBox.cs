// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUIBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GUI)]
[HutongGames.PlayMaker.Tooltip("GUI Box.")]
public class GUIBox : GUIContentAction
{
  public override void OnGUI()
  {
    base.OnGUI();
    if (string.IsNullOrEmpty(this.style.Value))
      GUI.Box(this.rect, this.content);
    else
      GUI.Box(this.rect, this.content, (GUIStyle) this.style.Value);
  }
}
