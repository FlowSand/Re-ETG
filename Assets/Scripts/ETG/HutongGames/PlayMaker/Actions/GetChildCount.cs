// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetChildCount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the number of children that a GameObject has.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class GetChildCount : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the number of children in an int variable.")]
    public FsmInt storeResult;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.storeResult = (FsmInt) null;
    }

    public override void OnEnter()
    {
      this.DoGetChildCount();
      this.Finish();
    }

    private void DoGetChildCount()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      this.storeResult.Value = ownerDefaultTarget.transform.childCount;
    }
  }
}
