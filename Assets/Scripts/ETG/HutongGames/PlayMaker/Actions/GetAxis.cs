// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAxis
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Input)]
  [HutongGames.PlayMaker.Tooltip("Gets the value of the specified Input Axis and stores it in a Float Variable. See Unity Input Manager docs.")]
  public class GetAxis : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The name of the axis. Set in the Unity Input Manager.")]
    [RequiredField]
    public FsmString axisName;
    [HutongGames.PlayMaker.Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range.")]
    public FsmFloat multiplier;
    [HutongGames.PlayMaker.Tooltip("Store the result in a float variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat store;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Typically this would be set to True.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.axisName = (FsmString) string.Empty;
      this.multiplier = (FsmFloat) 1f;
      this.store = (FsmFloat) null;
      this.everyFrame = true;
    }

    public override void OnEnter()
    {
      this.DoGetAxis();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetAxis();

    private void DoGetAxis()
    {
      if (FsmString.IsNullOrEmpty(this.axisName))
        return;
      float axis = Input.GetAxis(this.axisName.Value);
      if (!this.multiplier.IsNone)
        axis *= this.multiplier.Value;
      this.store.Value = axis;
    }
  }
}
