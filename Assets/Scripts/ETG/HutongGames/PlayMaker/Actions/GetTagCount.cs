// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetTagCount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the number of Game Objects in the scene with the specified Tag.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class GetTagCount : FsmStateAction
  {
    [UIHint(UIHint.Tag)]
    public FsmString tag;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmInt storeResult;

    public override void Reset()
    {
      this.tag = (FsmString) "Untagged";
      this.storeResult = (FsmInt) null;
    }

    public override void OnEnter()
    {
      GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag(this.tag.Value);
      if (this.storeResult != null)
        this.storeResult.Value = gameObjectsWithTag == null ? 0 : gameObjectsWithTag.Length;
      this.Finish();
    }
  }
}
