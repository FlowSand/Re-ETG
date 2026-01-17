// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAllFsmGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Set the value of a Game Object Variable in another All FSM. Accept null reference")]
[ActionCategory(ActionCategory.StateMachine)]
public class SetAllFsmGameObject : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  public bool everyFrame;

  public override void Reset()
  {
  }

  public override void OnEnter()
  {
    if (this.everyFrame)
      return;
    this.Finish();
  }

  private void DoSetFsmGameObject()
  {
  }
}
