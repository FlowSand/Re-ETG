// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetRandomChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GameObject)]
  [HutongGames.PlayMaker.Tooltip("Gets a Random Child of a Game Object.")]
  public class GetRandomChild : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmGameObject storeResult;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.storeResult = (FsmGameObject) null;
    }

    public override void OnEnter()
    {
      this.DoGetRandomChild();
      this.Finish();
    }

    private void DoGetRandomChild()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      int childCount = ownerDefaultTarget.transform.childCount;
      if (childCount == 0)
        return;
      this.storeResult.Value = ownerDefaultTarget.transform.GetChild(Random.Range(0, childCount)).gameObject;
    }
  }
}
