// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetCollider2dIsTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Set the isTrigger option of a Collider2D. Optionally set all collider2D found on the gameobject Target.")]
[ActionCategory(ActionCategory.Physics2D)]
public class SetCollider2dIsTrigger : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject with the Collider2D attached")]
  [CheckForComponent(typeof (Collider2D))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The flag value")]
  [RequiredField]
  public FsmBool isTrigger;
  [HutongGames.PlayMaker.Tooltip("Set all Colliders on the GameObject target")]
  public bool setAllColliders;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.isTrigger = (FsmBool) false;
    this.setAllColliders = false;
  }

  public override void OnEnter()
  {
    this.DoSetIsTrigger();
    this.Finish();
  }

  private void DoSetIsTrigger()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    if (this.setAllColliders)
    {
      foreach (Collider2D component in ownerDefaultTarget.GetComponents<Collider2D>())
        component.isTrigger = this.isTrigger.Value;
    }
    else
    {
      if (!((Object) ownerDefaultTarget.GetComponent<Collider2D>() != (Object) null))
        return;
      ownerDefaultTarget.GetComponent<Collider2D>().isTrigger = this.isTrigger.Value;
    }
  }
}
