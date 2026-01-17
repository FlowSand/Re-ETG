// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.BaseFsmVariableIndexAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.StateMachine)]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  public abstract class BaseFsmVariableIndexAction : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The event to trigger if the index is out of range")]
    [ActionSection("Events")]
    public FsmEvent indexOutOfRange;
    [HutongGames.PlayMaker.Tooltip("The event to send if the FSM is not found.")]
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
}
