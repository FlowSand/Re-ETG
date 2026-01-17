// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetVectorLength
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Vector3)]
[Tooltip("Get Vector3 Length.")]
public class GetVectorLength : FsmStateAction
{
  public FsmVector3 vector3;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat storeLength;

  public override void Reset()
  {
    this.vector3 = (FsmVector3) null;
    this.storeLength = (FsmFloat) null;
  }

  public override void OnEnter()
  {
    this.DoVectorLength();
    this.Finish();
  }

  private void DoVectorLength()
  {
    if (this.vector3 == null || this.storeLength == null)
      return;
    this.storeLength.Value = this.vector3.Value.magnitude;
  }
}
