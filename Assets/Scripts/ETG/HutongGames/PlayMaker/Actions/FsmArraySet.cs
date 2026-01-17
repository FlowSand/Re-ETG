// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FsmArraySet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Obsolete("This action was wip and accidentally released.")]
  [HutongGames.PlayMaker.Tooltip("Set an item in an Array Variable in another FSM.")]
  [ActionCategory(ActionCategory.Array)]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  public class FsmArraySet : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmName)]
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object.")]
    public FsmString fsmName;
    [HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
    [RequiredField]
    public FsmString variableName;
    [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
    public FsmString setValue;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
    public bool everyFrame;
    private GameObject goLastFrame;
    private PlayMakerFSM fsm;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.setValue = (FsmString) null;
    }

    public override void OnEnter()
    {
      this.DoSetFsmString();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoSetFsmString()
    {
      if (this.setValue == null)
        return;
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((UnityEngine.Object) ownerDefaultTarget == (UnityEngine.Object) null)
        return;
      if ((UnityEngine.Object) ownerDefaultTarget != (UnityEngine.Object) this.goLastFrame)
      {
        this.goLastFrame = ownerDefaultTarget;
        this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
      }
      if ((UnityEngine.Object) this.fsm == (UnityEngine.Object) null)
      {
        this.LogWarning("Could not find FSM: " + this.fsmName.Value);
      }
      else
      {
        FsmString fsmString = this.fsm.FsmVariables.GetFsmString(this.variableName.Value);
        if (fsmString != null)
          fsmString.Value = this.setValue.Value;
        else
          this.LogWarning("Could not find variable: " + this.variableName.Value);
      }
    }

    public override void OnUpdate() => this.DoSetFsmString();
  }
}
