// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GameObjectCompare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  [HutongGames.PlayMaker.Tooltip("Compares 2 Game Objects and sends Events based on the result.")]
  public class GameObjectCompare : FsmStateAction
  {
    [Title("Game Object")]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("A Game Object variable to compare.")]
    [RequiredField]
    public FsmOwnerDefault gameObjectVariable;
    [HutongGames.PlayMaker.Tooltip("Compare the variable with this Game Object")]
    [RequiredField]
    public FsmGameObject compareTo;
    [HutongGames.PlayMaker.Tooltip("Send this event if Game Objects are equal")]
    public FsmEvent equalEvent;
    [HutongGames.PlayMaker.Tooltip("Send this event if Game Objects are not equal")]
    public FsmEvent notEqualEvent;
    [HutongGames.PlayMaker.Tooltip("Store the result of the check in a Bool Variable. (True if equal, false if not equal).")]
    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you're waiting for a true or false result.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObjectVariable = (FsmOwnerDefault) null;
      this.compareTo = (FsmGameObject) null;
      this.equalEvent = (FsmEvent) null;
      this.notEqualEvent = (FsmEvent) null;
      this.storeResult = (FsmBool) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGameObjectCompare();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGameObjectCompare();

    private void DoGameObjectCompare()
    {
      bool flag = (Object) this.Fsm.GetOwnerDefaultTarget(this.gameObjectVariable) == (Object) this.compareTo.Value;
      this.storeResult.Value = flag;
      if (flag && this.equalEvent != null)
      {
        this.Fsm.Event(this.equalEvent);
      }
      else
      {
        if (flag || this.notEqualEvent == null)
          return;
        this.Fsm.Event(this.notEqualEvent);
      }
    }
  }
}
