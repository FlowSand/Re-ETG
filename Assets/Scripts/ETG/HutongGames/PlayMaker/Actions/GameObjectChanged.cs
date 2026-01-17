// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GameObjectChanged
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  [HutongGames.PlayMaker.Tooltip("Tests if the value of a GameObject variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
  public class GameObjectChanged : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject variable to watch for a change.")]
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmGameObject gameObjectVariable;
    [HutongGames.PlayMaker.Tooltip("Event to send if the variable changes.")]
    public FsmEvent changedEvent;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Set to True if the variable changes.")]
    public FsmBool storeResult;
    private GameObject previousValue;

    public override void Reset()
    {
      this.gameObjectVariable = (FsmGameObject) null;
      this.changedEvent = (FsmEvent) null;
      this.storeResult = (FsmBool) null;
    }

    public override void OnEnter()
    {
      if (this.gameObjectVariable.IsNone)
        this.Finish();
      else
        this.previousValue = this.gameObjectVariable.Value;
    }

    public override void OnUpdate()
    {
      this.storeResult.Value = false;
      if (!((Object) this.gameObjectVariable.Value != (Object) this.previousValue))
        return;
      this.storeResult.Value = true;
      this.Fsm.Event(this.changedEvent);
    }
  }
}
