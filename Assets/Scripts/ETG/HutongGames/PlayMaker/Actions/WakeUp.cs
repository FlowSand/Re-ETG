// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.WakeUp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics)]
  [HutongGames.PlayMaker.Tooltip("Forces a Game Object's Rigid Body to wake up.")]
  public class WakeUp : ComponentAction<Rigidbody>
  {
    [CheckForComponent(typeof (Rigidbody))]
    [RequiredField]
    public FsmOwnerDefault gameObject;

    public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

    public override void OnEnter()
    {
      this.DoWakeUp();
      this.Finish();
    }

    private void DoWakeUp()
    {
      if (!this.UpdateCache(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner))
        return;
      this.rigidbody.WakeUp();
    }
  }
}
