// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetVector3Value
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sets the value of a Vector3 Variable.")]
  [ActionCategory(ActionCategory.Vector3)]
  public class SetVector3Value : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector3 vector3Variable;
    [RequiredField]
    public FsmVector3 vector3Value;
    public bool everyFrame;

    public override void Reset()
    {
      this.vector3Variable = (FsmVector3) null;
      this.vector3Value = (FsmVector3) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.vector3Variable.Value = this.vector3Value.Value;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.vector3Variable.Value = this.vector3Value.Value;
  }
}
