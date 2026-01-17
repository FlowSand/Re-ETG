// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetParent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Parent of a Game Object.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class GetParent : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    public FsmGameObject storeResult;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.storeResult = (FsmGameObject) null;
    }

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      this.storeResult.Value = !((Object) ownerDefaultTarget != (Object) null) ? (GameObject) null : (!((Object) ownerDefaultTarget.transform.parent == (Object) null) ? ownerDefaultTarget.transform.parent.gameObject : (GameObject) null);
      this.Finish();
    }
  }
}
