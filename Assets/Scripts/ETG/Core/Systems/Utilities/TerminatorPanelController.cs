// Decompiled with JetBrains decompiler
// Type: TerminatorPanelController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class TerminatorPanelController : MonoBehaviour
    {
      public dfLabel leftLabel;
      public dfLabel rightLabel;
      public dfLabel bottomLabel;
      [NonSerialized]
      public bool IsActive;
      public float HangTime = 3f;
      private dfPanel m_panel;

      private void Awake()
      {
        this.m_panel = this.GetComponent<dfPanel>();
        this.m_panel.Opacity = 0.0f;
      }

      public void Trigger()
      {
        this.IsActive = true;
        this.StartCoroutine(this.HandleTrigger());
      }

      private int GetNumberOfDigitsNotContainingOne(int digits)
      {
        int notContainingOne = 0;
        for (int p = 0; p < digits; ++p)
        {
          int num = UnityEngine.Random.Range(2, 10);
          notContainingOne += num * (int) Mathf.Pow(10f, (float) p);
        }
        return notContainingOne;
      }

      private string GenerateLeftString(string lsb, string lsfb)
      {
        string str1 = string.Format(lsfb, (object) this.GetNumberOfDigitsNotContainingOne(6), (object) this.GetNumberOfDigitsNotContainingOne(3), (object) this.GetNumberOfDigitsNotContainingOne(2));
        string str2 = string.Format(lsfb, (object) this.GetNumberOfDigitsNotContainingOne(6), (object) this.GetNumberOfDigitsNotContainingOne(3), (object) this.GetNumberOfDigitsNotContainingOne(2));
        string str3 = string.Format(lsfb, (object) this.GetNumberOfDigitsNotContainingOne(6), (object) this.GetNumberOfDigitsNotContainingOne(3), (object) this.GetNumberOfDigitsNotContainingOne(2));
        string str4 = string.Format(lsfb, (object) this.GetNumberOfDigitsNotContainingOne(6), (object) this.GetNumberOfDigitsNotContainingOne(3), (object) this.GetNumberOfDigitsNotContainingOne(2));
        string str5 = string.Format(lsfb, (object) this.GetNumberOfDigitsNotContainingOne(6), (object) this.GetNumberOfDigitsNotContainingOne(3), (object) this.GetNumberOfDigitsNotContainingOne(2));
        string str6 = string.Format(lsfb, (object) this.GetNumberOfDigitsNotContainingOne(6), (object) this.GetNumberOfDigitsNotContainingOne(3), (object) this.GetNumberOfDigitsNotContainingOne(2));
        string str7 = string.Format(lsfb, (object) this.GetNumberOfDigitsNotContainingOne(6), (object) this.GetNumberOfDigitsNotContainingOne(3), (object) this.GetNumberOfDigitsNotContainingOne(2));
        return string.Format(lsb, (object) str1, (object) str2, (object) str3, (object) str4, (object) str5, (object) str6, (object) str7);
      }

      [DebuggerHidden]
      private IEnumerator HandleTrigger()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TerminatorPanelController.\u003CHandleTrigger\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }
    }

}
