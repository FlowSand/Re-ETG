// Decompiled with JetBrains decompiler
// Type: AnimatePopup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Examples/General/Animate Popup")]
    public class AnimatePopup : MonoBehaviour
    {
      private const float ANIMATION_LENGTH = 0.15f;
      private dfListbox target;

      private void OnDropdownOpen(dfDropdown dropdown, dfListbox popup)
      {
        if ((Object) this.target != (Object) null)
        {
          this.StopCoroutine("animateOpen");
          this.StopCoroutine("animateClose");
          Object.Destroy((Object) this.target.gameObject);
        }
        this.target = popup;
        this.StartCoroutine(this.animateOpen(popup));
      }

      private void OnDropdownClose(dfDropdown dropdown, dfListbox popup)
      {
        this.StartCoroutine(this.animateClose(popup));
      }

      [DebuggerHidden]
      private IEnumerator animateOpen(dfListbox popup)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AnimatePopup.<animateOpen>c__Iterator0()
        {
          popup = popup,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator animateClose(dfListbox popup)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AnimatePopup.<animateClose>c__Iterator1()
        {
          popup = popup,
          _this = this
        };
      }
    }

}
