// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetSystemDateTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Gets system date and time info and stores it in a string variable. An optional format string gives you a lot of control over the formatting (see online docs for format syntax).")]
  [ActionCategory(ActionCategory.Time)]
  public class GetSystemDateTime : FsmStateAction
  {
    [Tooltip("Store System DateTime as a string.")]
    [UIHint(UIHint.Variable)]
    public FsmString storeString;
    [Tooltip("Optional format string. E.g., MM/dd/yyyy HH:mm")]
    public FsmString format;
    [Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.storeString = (FsmString) null;
      this.format = (FsmString) "MM/dd/yyyy HH:mm";
    }

    public override void OnEnter()
    {
      this.storeString.Value = DateTime.Now.ToString(this.format.Value);
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      this.storeString.Value = DateTime.Now.ToString(this.format.Value);
    }
  }
}
