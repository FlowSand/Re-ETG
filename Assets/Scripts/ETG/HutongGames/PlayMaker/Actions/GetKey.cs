// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetKey
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the pressed state of a Key.")]
  [ActionCategory(ActionCategory.Input)]
  public class GetKey : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The key to test.")]
    [RequiredField]
    public KeyCode key;
    [HutongGames.PlayMaker.Tooltip("Store if the key is down (True) or up (False).")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmBool storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you're waiting for a key press/release.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.key = KeyCode.None;
      this.storeResult = (FsmBool) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetKey();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetKey();

    private void DoGetKey() => this.storeResult.Value = Input.GetKey(this.key);
  }
}
