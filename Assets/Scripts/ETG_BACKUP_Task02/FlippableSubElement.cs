// Decompiled with JetBrains decompiler
// Type: FlippableSubElement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable
[Serializable]
public class FlippableSubElement
{
  public FlippableSubElement.SubElementStyle elementStyle;
  public bool isMandatory;
  public bool onlyOneOfThese;
  public float spawnChance = 1f;
  public float flipDelay;
  public bool requiresDirection;
  public DungeonData.Direction requiredDirection;
  [ShowInInspectorIf("elementStyle", 0, false)]
  public tk2dSpriteAnimator targetAnimator;
  [ShowInInspectorIf("elementStyle", 0, false)]
  public string northAnimation;
  [ShowInInspectorIf("elementStyle", 0, false)]
  public string eastAnimation;
  [ShowInInspectorIf("elementStyle", 0, false)]
  public string southAnimation;
  [ShowInInspectorIf("elementStyle", 0, false)]
  public string westAnimation;
  [ShowInInspectorIf("elementStyle", 0, false)]
  public float additionalHeightModification;
  [ShowInInspectorIf("elementStyle", 1, false)]
  public GoopDefinition goopToUse;
  [ShowInInspectorIf("elementStyle", 1, false)]
  public float goopConeLength = 5f;
  [ShowInInspectorIf("elementStyle", 1, false)]
  public float goopConeArc = 45f;
  [ShowInInspectorIf("elementStyle", 1, false)]
  public AnimationCurve goopCurve;
  [ShowInInspectorIf("elementStyle", 1, false)]
  public float goopDuration = 0.5f;

  public void Trigger(DungeonData.Direction flipDirection, tk2dBaseSprite sourceTable)
  {
    if (this.requiresDirection && this.requiredDirection != flipDirection)
      return;
    if (this.elementStyle == FlippableSubElement.SubElementStyle.ANIMATOR)
    {
      this.targetAnimator.gameObject.SetActive(true);
      string name = string.Empty;
      switch (flipDirection)
      {
        case DungeonData.Direction.NORTH:
          name = this.northAnimation;
          break;
        case DungeonData.Direction.EAST:
          name = this.eastAnimation;
          break;
        case DungeonData.Direction.SOUTH:
          name = this.southAnimation;
          break;
        case DungeonData.Direction.WEST:
          name = this.westAnimation;
          break;
      }
      if (string.IsNullOrEmpty(name))
        this.targetAnimator.Play();
      else
        this.targetAnimator.Play(name);
      this.targetAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleted);
    }
    else
    {
      if (this.elementStyle != FlippableSubElement.SubElementStyle.GOOP)
        return;
      DeadlyDeadlyGoopManager managerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopToUse);
      Vector2 worldCenter = sourceTable.WorldCenter;
      if (flipDirection == DungeonData.Direction.EAST || flipDirection == DungeonData.Direction.WEST)
        worldCenter += new Vector2(0.0f, -0.5f);
      managerForGoopType.TimedAddGoopArc(worldCenter, this.goopConeLength, this.goopConeArc, DungeonData.GetIntVector2FromDirection(flipDirection).ToVector2(), this.goopDuration, this.goopCurve);
    }
  }

  private void AnimationCompleted(tk2dSpriteAnimator source, tk2dSpriteAnimationClip clerp)
  {
    this.targetAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleted);
    source.Sprite.IsPerpendicular = false;
    source.Sprite.HeightOffGround = this.additionalHeightModification - 1f;
    source.Sprite.UpdateZDepth();
  }

  public enum SubElementStyle
  {
    ANIMATOR,
    GOOP,
  }
}
