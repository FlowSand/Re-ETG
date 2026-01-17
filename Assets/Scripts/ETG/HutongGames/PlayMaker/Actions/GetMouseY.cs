// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetMouseY
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Y Position of the mouse and stores it in a Float Variable.")]
  [ActionCategory(ActionCategory.Input)]
  public class GetMouseY : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat storeResult;
    public bool normalize;

    public override void Reset()
    {
      this.storeResult = (FsmFloat) null;
      this.normalize = true;
    }

    public override void OnEnter() => this.DoGetMouseY();

    public override void OnUpdate() => this.DoGetMouseY();

    private void DoGetMouseY()
    {
      if (this.storeResult == null)
        return;
      float y = Input.mousePosition.y;
      if (this.normalize)
        y /= (float) Screen.height;
      this.storeResult.Value = y;
    }
  }
}
