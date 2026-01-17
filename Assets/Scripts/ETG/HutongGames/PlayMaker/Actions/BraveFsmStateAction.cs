// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.BraveFsmStateAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

public class BraveFsmStateAction : FsmStateAction
{
  protected void SetReplacementString(string targetString)
  {
    FsmString fsmString = this.Fsm.Variables.GetFsmString("npcReplacementString");
    if (fsmString == null)
      return;
    fsmString.Value = targetString;
  }

  protected T GetActionInPreviousNode<T>() where T : FsmStateAction
  {
    for (int index = 0; index < this.Fsm.PreviousActiveState.Actions.Length; ++index)
    {
      if (this.Fsm.PreviousActiveState.Actions[index] is T)
        return this.Fsm.PreviousActiveState.Actions[index] as T;
    }
    return (T) null;
  }

  protected T FindActionOfType<T>() where T : FsmStateAction
  {
    for (int index1 = 0; index1 < this.Fsm.States.Length; ++index1)
    {
      for (int index2 = 0; index2 < this.Fsm.States[index1].Actions.Length; ++index2)
      {
        if (this.Fsm.States[index1].Actions[index2] is T)
          return this.Fsm.States[index1].Actions[index2] as T;
      }
    }
    return (T) null;
  }
}
