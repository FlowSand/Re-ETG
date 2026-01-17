// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.BoolNoneTrue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Tests if all the Bool Variables are False.\nSend an event or store the result.")]
[ActionCategory(ActionCategory.Logic)]
public class BoolNoneTrue : FsmStateAction
{
  [Tooltip("The Bool variables to check.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmBool[] boolVariables;
  [Tooltip("Event to send if none of the Bool variables are True.")]
  public FsmEvent sendEvent;
  [UIHint(UIHint.Variable)]
  [Tooltip("Store the result in a Bool variable.")]
  public FsmBool storeResult;
  [Tooltip("Repeat every frame while the state is active.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.boolVariables = (FsmBool[]) null;
    this.sendEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoNoneTrue();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoNoneTrue();

  private void DoNoneTrue()
  {
    if (this.boolVariables.Length == 0)
      return;
    bool flag = true;
    for (int index = 0; index < this.boolVariables.Length; ++index)
    {
      if (this.boolVariables[index].Value)
      {
        flag = false;
        break;
      }
    }
    if (flag)
      this.Fsm.Event(this.sendEvent);
    this.storeResult.Value = flag;
  }
}
