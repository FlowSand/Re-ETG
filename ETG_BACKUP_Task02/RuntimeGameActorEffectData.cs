// Decompiled with JetBrains decompiler
// Type: RuntimeGameActorEffectData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class RuntimeGameActorEffectData
{
  public GameActor actor;
  public float elapsed;
  public float tickCounter;
  public GameActor.MovementModifier MovementModifier;
  public Action<Vector2> OnActorPreDeath;
  public Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> OnFlameAnimationCompleted;
  public float onActorVFXTimer;
  public tk2dBaseSprite instanceOverheadVFX;
  public float accumulator;
  public bool destroyVfx;
  public List<Tuple<GameObject, float>> vfxObjects;
}
