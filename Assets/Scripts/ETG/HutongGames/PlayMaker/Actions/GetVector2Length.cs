// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetVector2Length
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Vector2)]
  [Tooltip("Get Vector2 Length.")]
  public class GetVector2Length : FsmStateAction
  {
    [Tooltip("The Vector2 to get the length from")]
    public FsmVector2 vector2;
    [RequiredField]
    [Tooltip("The Vector2 the length")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeLength;
    [Tooltip("Repeat every frame")]
    public bool everyFrame;

    public override void Reset()
    {
      this.vector2 = (FsmVector2) null;
      this.storeLength = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoVectorLength();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoVectorLength();

    private void DoVectorLength()
    {
      if (this.vector2 == null || this.storeLength == null)
        return;
      this.storeLength.Value = this.vector2.Value.magnitude;
    }
  }
}
