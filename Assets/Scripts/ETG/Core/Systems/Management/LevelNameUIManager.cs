// Decompiled with JetBrains decompiler
// Type: LevelNameUIManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Management
{
    public class LevelNameUIManager : MonoBehaviour
    {
      public dfLabel levelNameLabel;
      public dfLabel levelNumberLabel;
      public dfSlicedSprite levelNameShadow;
      public dfSlicedSprite levelNumberShadow;
      public dfSlicedSprite hyphen;
      public float initialDelay = 0.5f;
      public float fadeInTime = 1f;
      public float displayTime = 3f;
      public float fadeOutTime = 1f;
      private const float c_widthBoost = 6f;
      private const float c_minWidth = 40f;
      private dfPanel m_panel;
      private bool m_displaying;

      public void ShowLevelName(Dungeon d) => this.StartCoroutine(this.ShowLevelName_CR(d));

      [DebuggerHidden]
      public IEnumerator ShowLevelName_CR(Dungeon d)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new LevelNameUIManager.\u003CShowLevelName_CR\u003Ec__Iterator0()
        {
          d = d,
          \u0024this = this
        };
      }

      public void ShowCustomLevelName(string name, string levelNumText = "Level 0")
      {
        this.m_panel = this.GetComponent<dfPanel>();
        this.levelNameLabel.Text = name;
        this.levelNameShadow.Width = this.levelNameLabel.GetAutosizeWidth() / 3f;
        this.levelNumberLabel.Text = levelNumText;
        this.levelNumberShadow.Width = this.levelNumberLabel.GetAutosizeWidth() / 3f;
        this.hyphen.Width = (float) (((double) Mathf.Max(this.levelNameLabel.GetAutosizeWidth(), this.levelNumberLabel.GetAutosizeWidth()) + 88.0) / 3.0);
        this.m_displaying = true;
        this.StartCoroutine(this.HandleLevelName());
      }

      public void BanishLevelNameText()
      {
        if (!this.m_displaying)
          return;
        this.m_displaying = false;
      }

      [DebuggerHidden]
      private IEnumerator HandleLevelName()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new LevelNameUIManager.\u003CHandleLevelName\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }
    }

}
