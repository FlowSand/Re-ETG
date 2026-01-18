// Decompiled with JetBrains decompiler
// Type: SpriteAnimatorKiller
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

public class SpriteAnimatorKiller : MonoBehaviour
  {
    public bool onlyDisable;
    [FormerlySerializedAs("deparentOnAwake")]
    public bool deparentOnStart;
    public List<GameObject> childObjectToDisable;
    public tk2dSpriteAnimator animator;
    public dfSpriteAnimation dfAnimation;
    public bool hasChildAnimators;
    public bool deparentAllChildren;
    public bool disableRendererOnDelay;
    public float delayDestructionTime;
    public float fadeTime;
    private bool m_initialized;
    private Renderer m_renderer;
    private float m_killTimer;
    private float m_fadeTimer;

    public void Awake()
    {
      if (!(bool) (UnityEngine.Object) this.animator)
        this.animator = this.GetComponent<tk2dSpriteAnimator>();
      if (!(bool) (UnityEngine.Object) this.dfAnimation)
        this.dfAnimation = this.GetComponent<dfSpriteAnimation>();
      this.m_renderer = !(bool) (UnityEngine.Object) this.animator ? this.GetComponent<Renderer>() : this.animator.renderer;
    }

    public void Start()
    {
      if (this.m_initialized || !this.enabled)
        return;
      if (this.onlyDisable)
      {
        this.m_renderer.enabled = true;
        this.animator.enabled = true;
      }
      if ((UnityEngine.Object) this.animator != (UnityEngine.Object) null)
        this.animator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
      if ((UnityEngine.Object) this.dfAnimation != (UnityEngine.Object) null)
        this.dfAnimation.AnimationCompleted += new TweenNotification(this.dfAnimationComplete);
      if (this.deparentOnStart)
        this.transform.parent = SpawnManager.Instance.VFX;
      this.m_initialized = true;
    }

    public void Update()
    {
      if ((double) this.m_killTimer > 0.0)
      {
        this.m_killTimer -= BraveTime.DeltaTime;
        if ((double) this.m_killTimer > 0.0)
          return;
        this.BeginDeath();
      }
      else
      {
        if ((double) this.m_fadeTimer <= 0.0)
          return;
        this.m_fadeTimer -= BraveTime.DeltaTime;
        this.animator.sprite.color = this.animator.sprite.color.WithAlpha(Mathf.Clamp01(this.m_fadeTimer / this.fadeTime));
        if ((double) this.m_fadeTimer > 0.0)
          return;
        this.FinishDeath();
      }
    }

    public void OnSpawned()
    {
      if (this.enabled)
        this.Start();
      if (!this.hasChildAnimators)
        return;
      SpriteAnimatorKiller[] componentsInChildren = this.GetComponentsInChildren<SpriteAnimatorKiller>();
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        if (!((UnityEngine.Object) componentsInChildren[index] == (UnityEngine.Object) this))
          componentsInChildren[index].OnSpawned();
      }
    }

    public void OnDespawned()
    {
      this.Cleanup();
      if (!this.hasChildAnimators)
        return;
      SpriteAnimatorKiller[] componentsInChildren = this.GetComponentsInChildren<SpriteAnimatorKiller>();
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        if (!((UnityEngine.Object) componentsInChildren[index] == (UnityEngine.Object) this))
          componentsInChildren[index].OnDespawned();
      }
    }

    public void Cleanup()
    {
      if ((UnityEngine.Object) this.animator != (UnityEngine.Object) null)
        this.animator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
      if ((UnityEngine.Object) this.dfAnimation != (UnityEngine.Object) null)
        this.dfAnimation.AnimationCompleted -= new TweenNotification(this.dfAnimationComplete);
      if ((bool) (UnityEngine.Object) this && (bool) (UnityEngine.Object) this.transform && (bool) (UnityEngine.Object) SpawnManager.Instance && (UnityEngine.Object) this.transform.parent != (UnityEngine.Object) SpawnManager.Instance.VFX && ((UnityEngine.Object) this.transform.parent == (UnityEngine.Object) null || (UnityEngine.Object) this.transform.parent.GetComponent<SpriteAnimatorKiller>() == (UnityEngine.Object) null))
        this.transform.parent = SpawnManager.Instance.VFX;
      this.m_initialized = false;
    }

    public void Restart()
    {
      if ((bool) (UnityEngine.Object) this.animator)
      {
        this.animator.enabled = true;
        this.animator.PlayFrom(0.0f);
      }
      if ((bool) (UnityEngine.Object) this.dfAnimation)
        Debug.LogWarning((object) "unsupported");
      this.m_renderer.enabled = true;
      for (int index = 0; index < this.childObjectToDisable.Count; ++index)
        this.childObjectToDisable[index].SetActive(true);
    }

    public void Disable()
    {
      if ((bool) (UnityEngine.Object) this.animator)
        this.animator.enabled = false;
      if ((bool) (UnityEngine.Object) this.dfAnimation)
        this.dfAnimation.enabled = false;
      this.m_renderer.enabled = false;
      for (int index = 0; index < this.childObjectToDisable.Count; ++index)
        this.childObjectToDisable[index].SetActive(false);
    }

    public void OnAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
    {
      if ((double) this.delayDestructionTime > 0.0)
      {
        if ((bool) (UnityEngine.Object) this.m_renderer && this.disableRendererOnDelay)
          this.m_renderer.enabled = false;
        this.m_killTimer = this.delayDestructionTime;
      }
      else
        this.BeginDeath();
    }

    public void dfAnimationComplete(dfTweenPlayableBase source)
    {
      if ((double) this.delayDestructionTime > 0.0)
        this.m_killTimer = this.delayDestructionTime;
      else
        this.BeginDeath();
    }

    private void BeginDeath()
    {
      if ((double) this.fadeTime > 0.0)
        this.m_fadeTimer = this.fadeTime;
      else
        this.FinishDeath();
    }

    private void FinishDeath()
    {
      if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.transform)
        return;
      if (this.deparentAllChildren)
      {
        while (this.transform.childCount > 0)
          this.transform.GetChild(0).parent = this.transform.parent;
      }
      if ((double) this.fadeTime > 0.0 && (bool) (UnityEngine.Object) this.animator && (bool) (UnityEngine.Object) this.animator.sprite)
        this.animator.sprite.color = this.animator.sprite.color.WithAlpha(1f);
      if (this.onlyDisable)
      {
        this.Disable();
      }
      else
      {
        this.Cleanup();
        SpawnManager.Despawn(this.gameObject);
      }
    }
  }

