// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetLightSpotAngle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the Spot Angle of a Light.")]
  [ActionCategory(ActionCategory.Lights)]
  public class SetLightSpotAngle : ComponentAction<Light>
  {
    [CheckForComponent(typeof (Light))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    public FsmFloat lightSpotAngle;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.lightSpotAngle = (FsmFloat) 20f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetLightRange();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetLightRange();

    private void DoSetLightRange()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.light.spotAngle = this.lightSpotAngle.Value;
    }
  }
}
