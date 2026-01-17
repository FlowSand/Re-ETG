// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.TweenManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween;

public class TweenManager : MonoBehaviour
{
  private static TweenManager instance;
  internal static float realDeltaTime;
  private static float lastFrameTime = 0.0f;
  internal float realTimeSinceStartup;
  private List<ITweenUpdatable> playingTweens = new List<ITweenUpdatable>();
  private Queue<ITweenUpdatable> addTweenQueue = new Queue<ITweenUpdatable>();
  private Queue<ITweenUpdatable> removeTweenQueue = new Queue<ITweenUpdatable>();

  static TweenManager() => TweenManager.realDeltaTime = 0.0f;

  public static TweenManager Instance
  {
    get
    {
      lock ((object) typeof (TweenManager))
      {
        if ((Object) TweenManager.instance == (Object) null)
        {
          GameObject gameObject = new GameObject("_TweenManager_");
          gameObject.hideFlags = HideFlags.HideInHierarchy;
          TweenManager.instance = gameObject.AddComponent<TweenManager>();
        }
        return TweenManager.instance;
      }
    }
  }

  public void RegisterTween(ITweenUpdatable tween)
  {
    lock ((object) this.playingTweens)
    {
      if (this.playingTweens.Contains(tween) && !this.removeTweenQueue.Contains(tween))
        return;
      lock ((object) this.addTweenQueue)
        this.addTweenQueue.Enqueue(tween);
    }
  }

  public void UnregisterTween(ITweenUpdatable tween)
  {
    lock ((object) this.removeTweenQueue)
    {
      if (!this.playingTweens.Contains(tween) || this.removeTweenQueue.Contains(tween))
        return;
      this.removeTweenQueue.Enqueue(tween);
    }
  }

  public void Clear()
  {
    lock ((object) this.playingTweens)
    {
      this.playingTweens.Clear();
      this.removeTweenQueue.Clear();
    }
  }

  public virtual void OnDestroy() => TweenManager.instance = (TweenManager) null;

  public virtual void Update()
  {
    this.realTimeSinceStartup = Time.realtimeSinceStartup;
    TweenManager.realDeltaTime = this.realTimeSinceStartup - TweenManager.lastFrameTime;
    TweenManager.lastFrameTime = this.realTimeSinceStartup;
    lock ((object) this.playingTweens)
    {
      lock ((object) this.addTweenQueue)
      {
        while (this.addTweenQueue.Count > 0)
          this.playingTweens.Add(this.addTweenQueue.Dequeue());
      }
      lock ((object) this.removeTweenQueue)
      {
        while (this.removeTweenQueue.Count > 0)
          this.playingTweens.Remove(this.removeTweenQueue.Dequeue());
      }
      int count = this.playingTweens.Count;
      for (int index = 0; index < count; ++index)
      {
        ITweenUpdatable playingTween = this.playingTweens[index];
        switch (playingTween.State)
        {
          case TweenState.Playing:
          case TweenState.Started:
            playingTween.Update();
            break;
        }
      }
    }
  }
}
