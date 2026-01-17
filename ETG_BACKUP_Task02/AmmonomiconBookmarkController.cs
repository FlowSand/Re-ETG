// Decompiled with JetBrains decompiler
// Type: AmmonomiconBookmarkController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class AmmonomiconBookmarkController : MonoBehaviour
{
  public dfAnimationClip AppearClip;
  public string SelectSpriteName;
  public dfAnimationClip SelectClip;
  public string DeselectSelectedSpriteName;
  public string TargetNewPageLeft;
  public AmmonomiconPageRenderer.PageType LeftPageType;
  public string TargetNewPageRight;
  public AmmonomiconPageRenderer.PageType RightPageType;
  private bool m_isCurrentPage;
  private dfButton m_sprite;
  private dfSpriteAnimation m_animator;
  private AmmonomiconInstanceManager m_ammonomiconInstance;

  public bool IsFocused => this.m_sprite.HasFocus;

  public bool IsCurrentPage
  {
    get => this.m_isCurrentPage;
    set
    {
      if (this.m_isCurrentPage == value)
        return;
      int selectedTabIndex = this.m_ammonomiconInstance.CurrentlySelectedTabIndex;
      this.m_isCurrentPage = value;
      if (this.m_isCurrentPage)
      {
        this.m_sprite.BackgroundSprite = this.DeselectSelectedSpriteName;
        this.TriggerSelectedAnimation();
        for (int index = 0; index < this.m_ammonomiconInstance.bookmarks.Length; ++index)
        {
          if ((UnityEngine.Object) this.m_ammonomiconInstance.bookmarks[index] != (UnityEngine.Object) this && this.m_ammonomiconInstance.bookmarks[index].IsCurrentPage)
            this.m_ammonomiconInstance.bookmarks[index].IsCurrentPage = false;
        }
        this.m_ammonomiconInstance.CurrentlySelectedTabIndex = Array.IndexOf<AmmonomiconBookmarkController>(this.m_ammonomiconInstance.bookmarks, this);
        if (this.m_ammonomiconInstance.CurrentlySelectedTabIndex > selectedTabIndex)
          AmmonomiconController.Instance.TurnToNextPage(this.TargetNewPageLeft, this.LeftPageType, this.TargetNewPageRight, this.RightPageType);
        else if (this.m_ammonomiconInstance.CurrentlySelectedTabIndex < selectedTabIndex)
          AmmonomiconController.Instance.TurnToPreviousPage(this.TargetNewPageLeft, this.LeftPageType, this.TargetNewPageRight, this.RightPageType);
        this.m_sprite.Focus(true);
      }
      else
      {
        this.m_animator.Stop();
        this.m_sprite.BackgroundSprite = this.AppearClip.Sprites[this.AppearClip.Sprites.Count - 1];
      }
    }
  }

  private void Start()
  {
    this.m_sprite = this.GetComponent<dfButton>();
    this.m_ammonomiconInstance = this.m_sprite.GetManager().GetComponent<AmmonomiconInstanceManager>();
    this.m_sprite.IsVisible = false;
    this.m_animator = this.gameObject.AddComponent<dfSpriteAnimation>();
    this.m_animator.LoopType = dfTweenLoopType.Once;
    this.m_animator.Target = new dfComponentMemberInfo();
    dfComponentMemberInfo target = this.m_animator.Target;
    target.Component = (Component) this.m_sprite;
    target.MemberName = "BackgroundSprite";
    this.m_animator.Clip = this.AppearClip;
    this.m_animator.Length = 0.35f;
    this.m_sprite.MouseEnter += new MouseEventHandler(this.OnMouseEnter);
    this.m_sprite.MouseLeave += new MouseEventHandler(this.OnMouseLeave);
    this.m_sprite.GotFocus += new FocusEventHandler(this.Focus);
    this.m_sprite.LostFocus += new FocusEventHandler(this.Defocus);
    this.m_sprite.Click += new MouseEventHandler(this.SelectThisTab);
    UIKeyControls component = this.GetComponent<UIKeyControls>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.OnRightDown += (System.Action) (() =>
    {
      if ((UnityEngine.Object) AmmonomiconController.Instance.ImpendingLeftPageRenderer != (UnityEngine.Object) null && AmmonomiconController.Instance.ImpendingLeftPageRenderer.LastFocusTarget != null)
      {
        InControlInputAdapter.SkipInputForRestOfFrame = true;
        AmmonomiconController.Instance.ImpendingLeftPageRenderer.LastFocusTarget.Focus();
      }
      else if (AmmonomiconController.Instance.CurrentLeftPageRenderer.LastFocusTarget != null)
      {
        InControlInputAdapter.SkipInputForRestOfFrame = true;
        AmmonomiconController.Instance.CurrentLeftPageRenderer.LastFocusTarget.Focus();
      }
      else
      {
        if (AmmonomiconController.Instance.CurrentRightPageRenderer.LastFocusTarget == null)
          return;
        InControlInputAdapter.SkipInputForRestOfFrame = true;
        AmmonomiconController.Instance.CurrentRightPageRenderer.LastFocusTarget.Focus();
      }
    });
  }

  private void OnMouseLeave(dfControl control, dfMouseEventArgs mouseEvent)
  {
    if (this.m_animator.IsPlaying || this.m_sprite.HasFocus)
      return;
    this.Defocus(control, (dfFocusEventArgs) null);
  }

  private void OnMouseEnter(dfControl control, dfMouseEventArgs mouseEvent)
  {
    if (this.m_animator.IsPlaying)
      return;
    if (this.IsCurrentPage)
      this.m_sprite.BackgroundSprite = this.SelectClip.Sprites[this.SelectClip.Sprites.Count - 1];
    else
      this.m_sprite.BackgroundSprite = this.SelectSpriteName;
    this.m_sprite.Focus(true);
  }

  public void Enable()
  {
    this.m_sprite.Enable();
    this.m_sprite.IsVisible = true;
    this.m_sprite.IsInteractive = true;
    this.m_sprite.Click += new MouseEventHandler(this.SelectThisTab);
  }

  public void Disable()
  {
    this.m_animator.Stop();
    this.m_sprite.Disable();
    this.m_sprite.IsVisible = false;
    this.m_sprite.IsInteractive = false;
    this.m_sprite.Click -= new MouseEventHandler(this.SelectThisTab);
  }

  public void ForceFocus()
  {
    if (!((UnityEngine.Object) this.m_sprite != (UnityEngine.Object) null))
      return;
    this.m_sprite.Focus(true);
  }

  private void SelectThisTab(dfControl control, dfMouseEventArgs mouseEvent)
  {
    this.IsCurrentPage = true;
  }

  private void Defocus(dfControl control, dfFocusEventArgs args)
  {
    this.m_animator.Stop();
    if (this.IsCurrentPage)
      this.m_sprite.BackgroundSprite = this.DeselectSelectedSpriteName;
    else
      this.m_sprite.BackgroundSprite = this.AppearClip.Sprites[this.AppearClip.Sprites.Count - 1];
  }

  public void Focus(dfControl control, dfFocusEventArgs args)
  {
    if (this.IsCurrentPage)
      this.m_sprite.BackgroundSprite = this.SelectClip.Sprites[this.SelectClip.Sprites.Count - 1];
    else
      this.m_sprite.BackgroundSprite = this.SelectSpriteName;
  }

  public void TriggerAppearAnimation()
  {
    this.Enable();
    if (!this.IsCurrentPage)
    {
      this.m_animator.Clip = this.AppearClip;
      this.m_animator.Length = 0.35f;
      this.m_animator.Play();
    }
    else
      this.TriggerSelectedAnimation();
  }

  public void TriggerSelectedAnimation()
  {
    this.m_sprite.IsVisible = true;
    this.m_animator.Clip = this.SelectClip;
    this.m_animator.Length = 0.275f;
    this.m_animator.Play();
  }
}
