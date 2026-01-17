// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetMass
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Mass of a Game Object's Rigid Body.")]
  [ActionCategory(ActionCategory.Physics)]
  public class GetMass : ComponentAction<Rigidbody>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the Rigidbody")]
    [CheckForComponent(typeof (Rigidbody))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the mass in a float variable.")]
    public FsmFloat storeResult;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.storeResult = (FsmFloat) null;
    }

    public override void OnEnter()
    {
      this.DoGetMass();
      this.Finish();
    }

    private void DoGetMass()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.storeResult.Value = this.rigidbody.mass;
    }
  }
}
