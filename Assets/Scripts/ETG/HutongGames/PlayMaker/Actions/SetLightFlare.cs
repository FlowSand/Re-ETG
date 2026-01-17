// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetLightFlare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the Flare effect used by a Light.")]
  [ActionCategory(ActionCategory.Lights)]
  public class SetLightFlare : ComponentAction<Light>
  {
    [RequiredField]
    [CheckForComponent(typeof (Light))]
    public FsmOwnerDefault gameObject;
    public Flare lightFlare;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.lightFlare = (Flare) null;
    }

    public override void OnEnter()
    {
      this.DoSetLightRange();
      this.Finish();
    }

    private void DoSetLightRange()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.light.flare = this.lightFlare;
    }
  }
}
