// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetChildNum
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Child of a GameObject by Index.\nE.g., O to get the first child. HINT: Use this with an integer variable to iterate through children.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class GetChildNum : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The GameObject to search.")]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The index of the child to find.")]
    public FsmInt childIndex;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the child in a GameObject variable.")]
    [RequiredField]
    public FsmGameObject store;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.childIndex = (FsmInt) 0;
      this.store = (FsmGameObject) null;
    }

    public override void OnEnter()
    {
      this.store.Value = this.DoGetChildNum(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
      this.Finish();
    }

    private GameObject DoGetChildNum(GameObject go)
    {
      return (Object) go == (Object) null ? (GameObject) null : go.transform.GetChild(this.childIndex.Value % go.transform.childCount).gameObject;
    }
  }
}
