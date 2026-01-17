// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetMass
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics)]
  [HutongGames.PlayMaker.Tooltip("Sets the Mass of a Game Object's Rigid Body.")]
  public class SetMass : ComponentAction<Rigidbody>
  {
    [RequiredField]
    [CheckForComponent(typeof (Rigidbody))]
    public FsmOwnerDefault gameObject;
    [HasFloatSlider(0.1f, 10f)]
    [RequiredField]
    public FsmFloat mass;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.mass = (FsmFloat) 1f;
    }

    public override void OnEnter()
    {
      this.DoSetMass();
      this.Finish();
    }

    private void DoSetMass()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.rigidbody.mass = this.mass.Value;
    }
  }
}
