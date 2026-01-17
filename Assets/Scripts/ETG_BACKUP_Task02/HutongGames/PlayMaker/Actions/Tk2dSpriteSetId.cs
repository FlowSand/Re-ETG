// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dSpriteSetId
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("2D Toolkit/Sprite")]
[HutongGames.PlayMaker.Tooltip("Set the sprite id of a sprite. Can use id or name. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
public class Tk2dSpriteSetId : FsmStateAction
{
  [CheckForComponent(typeof (tk2dBaseSprite))]
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.FsmInt)]
  [HutongGames.PlayMaker.Tooltip("The sprite Id")]
  public FsmInt spriteID;
  [HutongGames.PlayMaker.Tooltip("OR The sprite name ")]
  [UIHint(UIHint.FsmString)]
  public FsmString ORSpriteName;
  private tk2dBaseSprite _sprite;

  private void _getSprite()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
  }

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.spriteID = (FsmInt) null;
    this.ORSpriteName = (FsmString) null;
  }

  public override void OnEnter()
  {
    this._getSprite();
    this.DoSetSpriteID();
    this.Finish();
  }

  private void DoSetSpriteID()
  {
    if ((Object) this._sprite == (Object) null)
    {
      this.LogWarning("Missing tk2dBaseSprite component: " + this._sprite.gameObject.name);
    }
    else
    {
      int spriteIdByName = this.spriteID.Value;
      if (this.ORSpriteName.Value != string.Empty)
        spriteIdByName = this._sprite.GetSpriteIdByName(this.ORSpriteName.Value);
      if (spriteIdByName == this._sprite.spriteId)
        return;
      this._sprite.spriteId = spriteIdByName;
    }
  }
}
