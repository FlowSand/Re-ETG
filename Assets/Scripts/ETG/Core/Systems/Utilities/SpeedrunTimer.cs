// Decompiled with JetBrains decompiler
// Type: SpeedrunTimer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class SpeedrunTimer : MonoBehaviour
  {
    public tk2dTextMesh tk2dTarget;
    private Renderer tk2dRenderer;
    private dfLabel m_label;
    private int m_lastPlayedSeconds;
    private char[] m_formattedTimeSpan = new char[11];

    private void Start()
    {
      this.m_label = this.GetComponent<dfLabel>();
      this.m_lastPlayedSeconds = 0;
      if (!(bool) (UnityEngine.Object) this.tk2dTarget)
        return;
      this.tk2dRenderer = this.tk2dTarget.GetComponent<Renderer>();
    }

    private void Update()
    {
      if ((bool) (UnityEngine.Object) this.tk2dTarget)
      {
        if (!this.tk2dRenderer.enabled && GameManager.Options.SpeedrunMode)
        {
          this.m_label.Parent.IsVisible = true;
          this.m_label.IsVisible = false;
          this.tk2dRenderer.enabled = true;
        }
        if (this.tk2dRenderer.enabled && !GameManager.Options.SpeedrunMode)
        {
          this.m_label.Parent.IsVisible = false;
          this.m_label.IsVisible = false;
          this.tk2dRenderer.enabled = false;
        }
        if (!GameManager.HasInstance || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
        {
          this.m_label.Parent.IsVisible = false;
          this.m_label.IsVisible = false;
          this.tk2dRenderer.enabled = false;
        }
        if (!this.tk2dRenderer.enabled)
          return;
      }
      else
      {
        if (!this.m_label.Parent.IsVisible && GameManager.Options.SpeedrunMode)
          this.m_label.Parent.IsVisible = true;
        if (this.m_label.Parent.IsVisible && !GameManager.Options.SpeedrunMode)
          this.m_label.Parent.IsVisible = false;
        if (!this.m_label.IsVisible)
          return;
      }
      this.m_label.Parent.Parent.RelativePosition = this.m_label.Parent.Parent.RelativePosition.WithY((float) ((double) GameUIRoot.Instance.p_playerCoinLabel.Parent.Parent.RelativePosition.y + (double) GameUIRoot.Instance.p_playerCoinLabel.Parent.Parent.Height + 3.0));
      float sessionStatValue = GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED);
      int seconds = Mathf.FloorToInt(sessionStatValue);
      if ((bool) (UnityEngine.Object) this.tk2dTarget)
      {
        int num1 = seconds / 3600;
        int num2 = seconds / 60 % 60;
        int num3 = seconds % 60;
        int num4 = Mathf.FloorToInt((float) (1000.0 * ((double) sessionStatValue % 1.0)));
        int num5 = 48 /*0x30*/;
        this.m_formattedTimeSpan[0] = (char) (num5 + num1 % 10);
        this.m_formattedTimeSpan[1] = ':';
        this.m_formattedTimeSpan[2] = (char) (num5 + num2 / 10 % 10);
        this.m_formattedTimeSpan[3] = (char) (num5 + num2 % 10);
        this.m_formattedTimeSpan[4] = ':';
        this.m_formattedTimeSpan[5] = (char) (num5 + num3 / 10 % 10);
        this.m_formattedTimeSpan[6] = (char) (num5 + num3 % 10);
        this.m_formattedTimeSpan[7] = '.';
        this.m_formattedTimeSpan[8] = (char) (num5 + num4 / 100 % 10);
        this.m_formattedTimeSpan[9] = (char) (num5 + num4 / 10 % 10);
        this.m_formattedTimeSpan[10] = (char) (num5 + num4 % 10);
        this.tk2dTarget.text = new string(this.m_formattedTimeSpan);
        float units = this.m_label.PixelsToUnits();
        this.tk2dTarget.scale = new Vector3(units, units, units) * 16f * 3f;
      }
      else if (!GameManager.Options.DisplaySpeedrunCentiseconds)
      {
        if (seconds == this.m_lastPlayedSeconds && seconds > 0)
          return;
        this.m_lastPlayedSeconds = seconds;
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds);
        this.m_label.Text = $"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
      }
      else
      {
        int milliseconds = Mathf.FloorToInt((float) (1000.0 * ((double) GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED) % 1.0)));
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds, milliseconds);
        this.m_label.Text = $"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00}";
      }
    }
  }

