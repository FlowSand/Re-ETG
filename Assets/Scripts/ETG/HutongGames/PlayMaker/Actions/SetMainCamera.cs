// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetMainCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Camera)]
  [HutongGames.PlayMaker.Tooltip("Sets the Main Camera.")]
  public class SetMainCamera : FsmStateAction
  {
    [CheckForComponent(typeof (Camera))]
    [HutongGames.PlayMaker.Tooltip("The GameObject to set as the main camera (should have a Camera component).")]
    [RequiredField]
    public FsmGameObject gameObject;

    public override void Reset() => this.gameObject = (FsmGameObject) null;

    public override void OnEnter()
    {
      if ((Object) this.gameObject.Value != (Object) null)
      {
        if ((Object) Camera.main != (Object) null)
          Camera.main.gameObject.tag = "Untagged";
        this.gameObject.Value.tag = "MainCamera";
      }
      this.Finish();
    }
  }
}
