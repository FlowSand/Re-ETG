// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetPreviousStateName
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.StateMachine)]
  [Tooltip("Gets the name of the previously active state and stores it in a String Variable.")]
  public class GetPreviousStateName : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    public FsmString storeName;

    public override void Reset() => this.storeName = (FsmString) null;

    public override void OnEnter()
    {
      this.storeName.Value = this.Fsm.PreviousActiveState != null ? this.Fsm.PreviousActiveState.Name : (string) null;
      this.Finish();
    }
  }
}
