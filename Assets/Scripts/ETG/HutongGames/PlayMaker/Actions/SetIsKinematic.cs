// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetIsKinematic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Controls whether physics affects the Game Object.")]
  [ActionCategory(ActionCategory.Physics)]
  public class SetIsKinematic : ComponentAction<Rigidbody>
  {
    [CheckForComponent(typeof (Rigidbody))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    public FsmBool isKinematic;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.isKinematic = (FsmBool) false;
    }

    public override void OnEnter()
    {
      this.DoSetIsKinematic();
      this.Finish();
    }

    private void DoSetIsKinematic()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.rigidbody.isKinematic = this.isKinematic.Value;
    }
  }
}
