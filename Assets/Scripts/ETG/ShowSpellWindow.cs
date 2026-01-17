// Decompiled with JetBrains decompiler
// Type: ShowSpellWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Actionbar/Show Spell Window")]
public class ShowSpellWindow : MonoBehaviour
{
  private bool busy;
  private bool isVisible;

  private void OnEnable()
  {
    GameObject.Find("Spell Window").GetComponent<dfControl>().IsVisible = false;
  }

  private void OnClick()
  {
    if (this.busy)
      return;
    this.StopAllCoroutines();
    dfControl component = GameObject.Find("Spell Window").GetComponent<dfControl>();
    if (!this.isVisible)
      this.StartCoroutine(this.showWindow(component));
    else
      this.StartCoroutine(this.hideWindow(component));
  }

  [DebuggerHidden]
  private IEnumerator hideWindow(dfControl window)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ShowSpellWindow.\u003ChideWindow\u003Ec__Iterator0()
    {
      window = window,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator showWindow(dfControl window)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ShowSpellWindow.\u003CshowWindow\u003Ec__Iterator1()
    {
      window = window,
      \u0024this = this
    };
  }
}
