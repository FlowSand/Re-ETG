// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetShadowStrength
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Lights)]
  [HutongGames.PlayMaker.Tooltip("Sets the strength of the shadows cast by a Light.")]
  public class SetShadowStrength : ComponentAction<Light>
  {
    [CheckForComponent(typeof (Light))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    public FsmFloat shadowStrength;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.shadowStrength = (FsmFloat) 0.8f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetShadowStrength();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetShadowStrength();

    private void DoSetShadowStrength()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.light.shadowStrength = this.shadowStrength.Value;
    }
  }
}
