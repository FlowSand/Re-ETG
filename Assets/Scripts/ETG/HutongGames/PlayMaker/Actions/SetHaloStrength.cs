// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetHaloStrength
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the size of light halos.")]
  [ActionCategory(ActionCategory.RenderSettings)]
  public class SetHaloStrength : FsmStateAction
  {
    [RequiredField]
    public FsmFloat haloStrength;
    public bool everyFrame;

    public override void Reset()
    {
      this.haloStrength = (FsmFloat) 0.5f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetHaloStrength();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetHaloStrength();

    private void DoSetHaloStrength() => RenderSettings.haloStrength = this.haloStrength.Value;
  }
}
