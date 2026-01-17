// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DontDestroyOnLoad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Level)]
[HutongGames.PlayMaker.Tooltip("Makes the Game Object not be destroyed automatically when loading a new scene.")]
public class DontDestroyOnLoad : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("GameObject to mark as DontDestroyOnLoad.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;

  public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

  public override void OnEnter()
  {
    Object.DontDestroyOnLoad((Object) this.Owner.transform.root.gameObject);
    this.Finish();
  }
}
