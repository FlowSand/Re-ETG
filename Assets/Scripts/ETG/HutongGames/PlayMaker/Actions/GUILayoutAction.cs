// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("GUILayout base action - don't use!")]
public abstract class GUILayoutAction : FsmStateAction
{
  public LayoutOption[] layoutOptions;
  private GUILayoutOption[] options;

  public GUILayoutOption[] LayoutOptions
  {
    get
    {
      if (this.options == null)
      {
        this.options = new GUILayoutOption[this.layoutOptions.Length];
        for (int index = 0; index < this.layoutOptions.Length; ++index)
          this.options[index] = this.layoutOptions[index].GetGUILayoutOption();
      }
      return this.options;
    }
  }

  public override void Reset() => this.layoutOptions = new LayoutOption[0];
}
