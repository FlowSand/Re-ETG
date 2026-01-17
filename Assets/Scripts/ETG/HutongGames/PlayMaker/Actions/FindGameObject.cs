// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FindGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Finds a Game Object by Name and/or Tag.")]
[ActionCategory(ActionCategory.GameObject)]
public class FindGameObject : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The name of the GameObject to find. You can leave this empty if you specify a Tag.")]
  public FsmString objectName;
  [HutongGames.PlayMaker.Tooltip("Find a GameObject with this tag. If Object Name is specified then both name and Tag must match.")]
  [UIHint(UIHint.Tag)]
  public FsmString withTag;
  [HutongGames.PlayMaker.Tooltip("Store the result in a GameObject variable.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmGameObject store;

  public override void Reset()
  {
    this.objectName = (FsmString) string.Empty;
    this.withTag = (FsmString) "Untagged";
    this.store = (FsmGameObject) null;
  }

  public override void OnEnter()
  {
    this.Find();
    this.Finish();
  }

  private void Find()
  {
    if (this.withTag.Value != "Untagged")
    {
      if (!string.IsNullOrEmpty(this.objectName.Value))
      {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(this.withTag.Value))
        {
          if (gameObject.name == this.objectName.Value)
          {
            this.store.Value = gameObject;
            return;
          }
        }
        this.store.Value = (GameObject) null;
      }
      else
        this.store.Value = GameObject.FindGameObjectWithTag(this.withTag.Value);
    }
    else
      this.store.Value = GameObject.Find(this.objectName.Value);
  }

  public override string ErrorCheck()
  {
    return string.IsNullOrEmpty(this.objectName.Value) && string.IsNullOrEmpty(this.withTag.Value) ? "Specify Name, Tag, or both." : (string) null;
  }
}
