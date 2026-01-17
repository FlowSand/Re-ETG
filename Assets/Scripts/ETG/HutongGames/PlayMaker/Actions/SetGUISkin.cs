// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGUISkin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the GUISkin used by GUI elements.")]
  [ActionCategory(ActionCategory.GUI)]
  public class SetGUISkin : FsmStateAction
  {
    [RequiredField]
    public GUISkin skin;
    public FsmBool applyGlobally;

    public override void Reset()
    {
      this.skin = (GUISkin) null;
      this.applyGlobally = (FsmBool) true;
    }

    public override void OnGUI()
    {
      if ((Object) this.skin != (Object) null)
        GUI.skin = this.skin;
      if (!this.applyGlobally.Value)
        return;
      PlayMakerGUI.GUISkin = this.skin;
      this.Finish();
    }
  }
}
