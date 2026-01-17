// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetParent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GameObject)]
  [HutongGames.PlayMaker.Tooltip("Sets the Parent of a Game Object.")]
  public class SetParent : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Game Object to parent.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The new parent for the Game Object.")]
    public FsmGameObject parent;
    [HutongGames.PlayMaker.Tooltip("Set the local position to 0,0,0 after parenting.")]
    public FsmBool resetLocalPosition;
    [HutongGames.PlayMaker.Tooltip("Set the local rotation to 0,0,0 after parenting.")]
    public FsmBool resetLocalRotation;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.parent = (FsmGameObject) null;
      this.resetLocalPosition = (FsmBool) null;
      this.resetLocalRotation = (FsmBool) null;
    }

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget != (Object) null)
      {
        ownerDefaultTarget.transform.parent = !((Object) this.parent.Value == (Object) null) ? this.parent.Value.transform : (Transform) null;
        if (this.resetLocalPosition.Value)
          ownerDefaultTarget.transform.localPosition = Vector3.zero;
        if (this.resetLocalRotation.Value)
          ownerDefaultTarget.transform.localRotation = Quaternion.identity;
      }
      this.Finish();
    }
  }
}
