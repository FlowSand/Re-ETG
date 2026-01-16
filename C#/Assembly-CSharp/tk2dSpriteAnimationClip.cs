// Decompiled with JetBrains decompiler
// Type: tk2dSpriteAnimationClip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class tk2dSpriteAnimationClip
{
  public string name = "Default";
  public tk2dSpriteAnimationFrame[] frames;
  public float fps = 30f;
  public int loopStart;
  public tk2dSpriteAnimationClip.WrapMode wrapMode;
  public float minFidgetDuration = 1f;
  public float maxFidgetDuration = 2f;

  public tk2dSpriteAnimationClip()
  {
  }

  public tk2dSpriteAnimationClip(tk2dSpriteAnimationClip source) => this.CopyFrom(source);

  public float BaseClipLength => (float) this.frames.Length / this.fps;

  public void CopyFrom(tk2dSpriteAnimationClip source)
  {
    this.name = source.name;
    if (source.frames == null)
    {
      this.frames = (tk2dSpriteAnimationFrame[]) null;
    }
    else
    {
      this.frames = new tk2dSpriteAnimationFrame[source.frames.Length];
      for (int index = 0; index < this.frames.Length; ++index)
      {
        if (source.frames[index] == null)
        {
          this.frames[index] = (tk2dSpriteAnimationFrame) null;
        }
        else
        {
          this.frames[index] = new tk2dSpriteAnimationFrame();
          this.frames[index].CopyFrom(source.frames[index]);
        }
      }
    }
    this.fps = source.fps;
    this.loopStart = source.loopStart;
    this.wrapMode = source.wrapMode;
    this.minFidgetDuration = source.minFidgetDuration;
    this.maxFidgetDuration = source.maxFidgetDuration;
    if (this.wrapMode != tk2dSpriteAnimationClip.WrapMode.Single || this.frames.Length <= 1)
      return;
    this.frames = new tk2dSpriteAnimationFrame[1]
    {
      this.frames[0]
    };
    Debug.LogError((object) $"Clip: '{this.name}' Fixed up frames for WrapMode.Single");
  }

  public void Clear()
  {
    this.name = string.Empty;
    this.frames = new tk2dSpriteAnimationFrame[0];
    this.fps = 30f;
    this.loopStart = 0;
    this.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
  }

  public bool Empty => this.name.Length == 0 || this.frames == null || this.frames.Length == 0;

  public tk2dSpriteAnimationFrame GetFrame(int frame) => this.frames[frame];

  public enum WrapMode
  {
    Loop,
    LoopSection,
    Once,
    PingPong,
    RandomFrame,
    RandomLoop,
    Single,
    LoopFidget,
  }
}
