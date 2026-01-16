// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.BaseFsmVariableAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
[ActionCategory(ActionCategory.StateMachine)]
public abstract class BaseFsmVariableAction : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The event to send if the FSM is not found.")]
  [ActionSection("Events")]
  public FsmEvent fsmNotFound;
  [HutongGames.PlayMaker.Tooltip("The event to send if the Variable is not found.")]
  public FsmEvent variableNotFound;
  private GameObject cachedGameObject;
  private string cachedFsmName;
  protected PlayMakerFSM fsm;

  public override void Reset()
  {
    this.fsmNotFound = (FsmEvent) null;
    this.variableNotFound = (FsmEvent) null;
  }

  protected bool UpdateCache(GameObject go, string fsmName)
  {
    if ((Object) go == (Object) null)
      return false;
    if ((Object) this.fsm == (Object) null || (Object) this.cachedGameObject != (Object) go || this.cachedFsmName != fsmName)
    {
      this.fsm = ActionHelpers.GetGameObjectFsm(go, fsmName);
      this.cachedGameObject = go;
      this.cachedFsmName = fsmName;
      if ((Object) this.fsm == (Object) null)
      {
        this.LogWarning("Could not find FSM: " + fsmName);
        this.Fsm.Event(this.fsmNotFound);
      }
    }
    return true;
  }

  protected void DoVariableNotFound(string variableName)
  {
    this.LogWarning("Could not find variable: " + variableName);
    this.Fsm.Event(this.variableNotFound);
  }
}
