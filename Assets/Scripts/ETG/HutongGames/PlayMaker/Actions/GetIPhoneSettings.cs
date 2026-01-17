// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetIPhoneSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Device)]
  [Tooltip("Get various iPhone settings.")]
  public class GetIPhoneSettings : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [Tooltip("Allows device to fall into 'sleep' state with screen being dim if no touches occurred. Default value is true.")]
    public FsmBool getScreenCanDarken;
    [Tooltip("A unique device identifier string. It is guaranteed to be unique for every device (Read Only).")]
    [UIHint(UIHint.Variable)]
    public FsmString getUniqueIdentifier;
    [Tooltip("The user defined name of the device (Read Only).")]
    [UIHint(UIHint.Variable)]
    public FsmString getName;
    [Tooltip("The model of the device (Read Only).")]
    [UIHint(UIHint.Variable)]
    public FsmString getModel;
    [Tooltip("The name of the operating system running on the device (Read Only).")]
    [UIHint(UIHint.Variable)]
    public FsmString getSystemName;
    [UIHint(UIHint.Variable)]
    [Tooltip("The generation of the device (Read Only).")]
    public FsmString getGeneration;

    public override void Reset()
    {
      this.getScreenCanDarken = (FsmBool) null;
      this.getUniqueIdentifier = (FsmString) null;
      this.getName = (FsmString) null;
      this.getModel = (FsmString) null;
      this.getSystemName = (FsmString) null;
      this.getGeneration = (FsmString) null;
    }

    public override void OnEnter() => this.Finish();
  }
}
