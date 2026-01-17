// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetSkybox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.RenderSettings)]
  [HutongGames.PlayMaker.Tooltip("Sets the global Skybox.")]
  public class SetSkybox : FsmStateAction
  {
    public FsmMaterial skybox;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the Skybox is changing.")]
    public bool everyFrame;

    public override void Reset() => this.skybox = (FsmMaterial) null;

    public override void OnEnter()
    {
      RenderSettings.skybox = this.skybox.Value;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => RenderSettings.skybox = this.skybox.Value;
  }
}
