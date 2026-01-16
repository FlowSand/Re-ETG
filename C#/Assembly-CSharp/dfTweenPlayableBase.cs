// Decompiled with JetBrains decompiler
// Type: dfTweenPlayableBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public abstract class dfTweenPlayableBase : MonoBehaviour
{
  public abstract string TweenName { get; set; }

  public abstract bool IsPlaying { get; }

  public abstract void Play();

  public abstract void Stop();

  public abstract void Reset();

  public void Enable() => this.enabled = true;

  public void Disable() => this.enabled = false;

  public override string ToString() => $"{this.TweenName} - {base.ToString()}";
}
