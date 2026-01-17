// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.UseGravity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets whether a Game Object's Rigidy Body is affected by Gravity.")]
  [ActionCategory(ActionCategory.Physics)]
  public class UseGravity : ComponentAction<Rigidbody>
  {
    [CheckForComponent(typeof (Rigidbody))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    public FsmBool useGravity;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.useGravity = (FsmBool) true;
    }

    public override void OnEnter()
    {
      this.DoUseGravity();
      this.Finish();
    }

    private void DoUseGravity()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.rigidbody.useGravity = this.useGravity.Value;
    }
  }
}
