// Decompiled with JetBrains decompiler
// Type: AmmonomiconInstanceManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class AmmonomiconInstanceManager : MonoBehaviour
{
  public AmmonomiconBookmarkController[] bookmarks;
  private int m_currentlySelectedBookmark;
  private dfGUIManager m_manager;

  public int CurrentlySelectedTabIndex
  {
    get => this.m_currentlySelectedBookmark;
    set => this.m_currentlySelectedBookmark = value;
  }

  public dfGUIManager GuiManager
  {
    get
    {
      if ((Object) this.m_manager == (Object) null)
        this.m_manager = this.GetComponent<dfGUIManager>();
      return this.m_manager;
    }
  }

  public bool BookmarkHasFocus
  {
    get
    {
      for (int index = 0; index < this.bookmarks.Length; ++index)
      {
        if (this.bookmarks[index].IsFocused)
          return true;
      }
      return false;
    }
  }

  public void Open()
  {
    this.m_currentlySelectedBookmark = 0;
    this.StartCoroutine(this.HandleOpenAmmonomicon());
  }

  public void Close()
  {
    for (int index = 0; index < this.bookmarks.Length; ++index)
      this.bookmarks[index].Disable();
  }

  public void LateUpdate()
  {
    if (!((Object) dfGUIManager.ActiveControl == (Object) null) || this.bookmarks == null || !((Object) this.bookmarks[this.m_currentlySelectedBookmark] != (Object) null))
      return;
    this.bookmarks[this.m_currentlySelectedBookmark].ForceFocus();
  }

  public void OpenDeath()
  {
    this.m_currentlySelectedBookmark = this.bookmarks.Length - 1;
    this.StartCoroutine(this.HandleOpenAmmonomiconDeath());
  }

  [DebuggerHidden]
  public IEnumerator InvariantWait(float t)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new AmmonomiconInstanceManager.\u003CInvariantWait\u003Ec__Iterator0()
    {
      t = t
    };
  }

  [DebuggerHidden]
  public IEnumerator HandleOpenAmmonomiconDeath()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new AmmonomiconInstanceManager.\u003CHandleOpenAmmonomiconDeath\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  public IEnumerator HandleOpenAmmonomicon()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new AmmonomiconInstanceManager.\u003CHandleOpenAmmonomicon\u003Ec__Iterator2()
    {
      \u0024this = this
    };
  }
}
