// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TransformPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Transform)]
  [HutongGames.PlayMaker.Tooltip("Transforms a Position from a Game Object's local space to world space.")]
  public class TransformPoint : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    public FsmVector3 localPosition;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector3 storeResult;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.localPosition = (FsmVector3) null;
      this.storeResult = (FsmVector3) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoTransformPoint();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoTransformPoint();

    private void DoTransformPoint()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      this.storeResult.Value = ownerDefaultTarget.transform.TransformPoint(this.localPosition.Value);
    }
  }
}
