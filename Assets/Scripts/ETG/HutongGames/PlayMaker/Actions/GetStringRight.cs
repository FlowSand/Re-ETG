// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetStringRight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Right n characters from a String.")]
  [ActionCategory(ActionCategory.String)]
  public class GetStringRight : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString stringVariable;
    [HutongGames.PlayMaker.Tooltip("Number of characters to get.")]
    public FsmInt charCount;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.stringVariable = (FsmString) null;
      this.charCount = (FsmInt) 0;
      this.storeResult = (FsmString) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetStringRight();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetStringRight();

    private void DoGetStringRight()
    {
      if (this.stringVariable.IsNone || this.storeResult.IsNone)
        return;
      string str = this.stringVariable.Value;
      int length = Mathf.Clamp(this.charCount.Value, 0, str.Length);
      this.storeResult.Value = str.Substring(str.Length - length, length);
    }
  }
}
