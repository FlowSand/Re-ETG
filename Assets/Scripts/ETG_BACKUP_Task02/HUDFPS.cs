// Decompiled with JetBrains decompiler
// Type: HUDFPS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class HUDFPS : MonoBehaviour
{
  [NonSerialized]
  public float updateInterval = 10f;
  private dfLabel m_label;
  private float accum;
  private int frames;
  private float timeleft;

  private void Start()
  {
    this.updateInterval = 0.5f;
    this.m_label = this.GetComponent<dfLabel>();
    if (!(bool) (UnityEngine.Object) this.m_label)
    {
      Debug.Log((object) "FramesPerSecond needs a dfLabel component!");
      this.enabled = false;
    }
    else
      this.timeleft = this.updateInterval;
  }

  private void Update()
  {
    if (!this.m_label.IsVisible)
      return;
    this.timeleft -= GameManager.INVARIANT_DELTA_TIME;
    this.accum += GameManager.INVARIANT_DELTA_TIME;
    ++this.frames;
    if ((double) this.timeleft > 0.0)
      return;
    float num = (float) this.frames / this.accum;
    this.m_label.Text = $"{num:F2} FPS";
    if ((double) num < 30.0)
      this.m_label.Color = (Color32) Color.yellow;
    else if ((double) num < 10.0)
      this.m_label.Color = (Color32) Color.red;
    else
      this.m_label.Color = (Color32) Color.green;
    this.timeleft = this.updateInterval;
    this.accum = 0.0f;
    this.frames = 0;
  }
}
