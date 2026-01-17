// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Input)]
[HutongGames.PlayMaker.Tooltip("Gets the pressed state of the specified Button and stores it in a Bool Variable. See Unity Input Manager docs.")]
public class GetButton : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The name of the button. Set in the Unity Input Manager.")]
  [RequiredField]
  public FsmString buttonName;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Store the result in a bool variable.")]
  [RequiredField]
  public FsmBool storeResult;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.buttonName = (FsmString) "Fire1";
    this.storeResult = (FsmBool) null;
    this.everyFrame = true;
  }

  public override void OnEnter()
  {
    this.DoGetButton();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetButton();

  private void DoGetButton() => this.storeResult.Value = Input.GetButton(this.buttonName.Value);
}
