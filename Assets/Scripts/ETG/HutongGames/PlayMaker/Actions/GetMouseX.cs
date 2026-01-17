// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetMouseX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the X Position of the mouse and stores it in a Float Variable.")]
  [ActionCategory(ActionCategory.Input)]
  public class GetMouseX : FsmStateAction
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

    public override void OnEnter() => this.DoGetMouseX();

    public override void OnUpdate() => this.DoGetMouseX();

    private void DoGetMouseX()
    {
      if (this.storeResult == null)
        return;
      float x = Input.mousePosition.x;
      if (this.normalize)
        x /= (float) Screen.width;
      this.storeResult.Value = x;
    }
  }
}
