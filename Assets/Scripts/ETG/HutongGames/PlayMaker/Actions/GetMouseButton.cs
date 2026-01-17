// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetMouseButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the pressed state of the specified Mouse Button and stores it in a Bool Variable. See Unity Input Manager doc.")]
  [ActionCategory(ActionCategory.Input)]
  public class GetMouseButton : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The mouse button to test.")]
    [RequiredField]
    public MouseButton button;
    [HutongGames.PlayMaker.Tooltip("Store the pressed state in a Bool Variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmBool storeResult;

    public override void Reset()
    {
      this.button = MouseButton.Left;
      this.storeResult = (FsmBool) null;
    }

    public override void OnEnter()
    {
      this.storeResult.Value = Input.GetMouseButton((int) this.button);
    }

    public override void OnUpdate()
    {
      this.storeResult.Value = Input.GetMouseButton((int) this.button);
    }
  }
}
