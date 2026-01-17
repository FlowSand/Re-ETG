// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vector3Normalize
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Normalizes a Vector3 Variable.")]
  [ActionCategory(ActionCategory.Vector3)]
  public class Vector3Normalize : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector3 vector3Variable;
    public bool everyFrame;

    public override void Reset()
    {
      this.vector3Variable = (FsmVector3) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.vector3Variable.Value = this.vector3Variable.Value.normalized;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      this.vector3Variable.Value = this.vector3Variable.Value.normalized;
    }
  }
}
