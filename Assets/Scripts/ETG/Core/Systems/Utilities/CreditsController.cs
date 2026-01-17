// Decompiled with JetBrains decompiler
// Type: CreditsController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class CreditsController : MonoBehaviour
    {
      public dfScrollPanel creditsPanel;
      public List<int> scrollThresholds;
      public List<float> scrollDelays;
      public float maxScrollSpeed = 20f;
      private int m_currentThreshold;

      private void Start()
      {
        GameManager.Instance.ClearActiveGameData(false, false);
        Object.Destroy((Object) GameManager.Instance.DungeonMusicController);
        this.StartCoroutine(this.ScrollToNextThreshold());
      }

      [DebuggerHidden]
      private IEnumerator ScrollToNextThreshold()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CreditsController.\u003CScrollToNextThreshold\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator WaitForNextThreshold()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CreditsController.\u003CWaitForNextThreshold\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }

      private void GoToMainMenu()
      {
        Cursor.visible = true;
        GameManager.Instance.LoadCharacterSelect();
      }
    }

}
