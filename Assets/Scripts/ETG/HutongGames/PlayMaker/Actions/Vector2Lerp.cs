// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vector2Lerp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Linearly interpolates between 2 vectors.")]
  [ActionCategory(ActionCategory.Vector2)]
  public class Vector2Lerp : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("First Vector.")]
    public FsmVector2 fromVector;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Second Vector.")]
    public FsmVector2 toVector;
    [HutongGames.PlayMaker.Tooltip("Interpolate between From Vector and ToVector by this amount. Value is clamped to 0-1 range. 0 = From Vector; 1 = To Vector; 0.5 = half way between.")]
    [RequiredField]
    public FsmFloat amount;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Store the result in this vector variable.")]
    public FsmVector2 storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if any of the values are changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      FsmVector2 fsmVector2_1 = new FsmVector2();
      fsmVector2_1.UseVariable = true;
      this.fromVector = fsmVector2_1;
      FsmVector2 fsmVector2_2 = new FsmVector2();
      fsmVector2_2.UseVariable = true;
      this.toVector = fsmVector2_2;
      this.storeResult = (FsmVector2) null;
      this.everyFrame = true;
    }

    public override void OnEnter()
    {
      this.DoVector2Lerp();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoVector2Lerp();

    private void DoVector2Lerp()
    {
      this.storeResult.Value = Vector2.Lerp(this.fromVector.Value, this.toVector.Value, this.amount.Value);
    }
  }
}
