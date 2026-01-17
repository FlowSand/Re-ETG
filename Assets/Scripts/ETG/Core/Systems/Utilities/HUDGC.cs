// Decompiled with JetBrains decompiler
// Type: HUDGC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Text;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class HUDGC : MonoBehaviour
    {
      public static bool ShowGcData;
      public static bool SkipNextGC;
      private static float B2MB = 9.536743E-07f;
      private static float B2kB = 0.0009765625f;
      private dfLabel m_label;
      private uint m_cachedMemMin;
      private uint m_cachedMemMax;
      private uint m_lastFrameMemSize;
      private bool m_showGcBarLastFrame;
      private float m_avgDuration;
      private float m_avgFrequency;
      private float m_avgMemDelta;
      private float m_lastGcTime;
      private float m_lastGcDuration;
      private float m_lastFrameTime;
      private StringBuilder stringBuilder = new StringBuilder();

      public void Start() => this.m_label = this.GetComponent<dfLabel>();

      public void Update()
      {
        if (HUDGC.ShowGcData)
        {
          if (!this.m_label.IsVisible)
            this.m_label.IsVisible = true;
          if (!this.m_showGcBarLastFrame)
          {
            this.m_cachedMemMin = ProfileUtils.GetMonoUsedHeapSize();
            this.m_cachedMemMax = ProfileUtils.GetMonoHeapSize();
            this.m_lastFrameMemSize = this.m_cachedMemMin;
          }
          uint monoUsedHeapSize = ProfileUtils.GetMonoUsedHeapSize();
          float num1 = UnityEngine.Time.realtimeSinceStartup - this.m_lastFrameTime;
          if (monoUsedHeapSize < this.m_lastFrameMemSize)
          {
            this.m_cachedMemMin = monoUsedHeapSize;
            if (!HUDGC.SkipNextGC)
              this.m_cachedMemMax = this.m_lastFrameMemSize;
            float newValue = UnityEngine.Time.realtimeSinceStartup - this.m_lastGcTime;
            if (!HUDGC.SkipNextGC)
            {
              this.m_lastGcDuration = num1;
              this.m_avgDuration = BraveMathCollege.MovingAverage(this.m_avgDuration, this.m_lastGcDuration, 5);
              if ((double) this.m_lastGcTime > 0.0)
                this.m_avgFrequency = BraveMathCollege.MovingAverage(this.m_avgFrequency, newValue, 5);
            }
            this.m_lastGcTime = UnityEngine.Time.realtimeSinceStartup;
            this.m_avgMemDelta = 0.0f;
          }
          if ((double) num1 > 0.0 && monoUsedHeapSize > this.m_lastFrameMemSize)
            this.m_avgMemDelta = BraveMathCollege.MovingAverage(this.m_avgMemDelta, (float) (monoUsedHeapSize - this.m_lastFrameMemSize) / num1, 90);
          float num2 = Mathf.InverseLerp((float) this.m_cachedMemMin, (float) this.m_cachedMemMax, (float) monoUsedHeapSize);
          this.stringBuilder.Length = 0;
          this.stringBuilder.AppendFormat("Memory Use: {0:00.000}%\n", (object) (float) ((double) num2 * 100.0));
          this.stringBuilder.AppendFormat(" - {0:0.00} MB / {1:0.00} MB\n", (object) (float) ((double) monoUsedHeapSize * (double) HUDGC.B2MB), (object) (float) ((double) this.m_cachedMemMax * (double) HUDGC.B2MB));
          this.stringBuilder.AppendFormat(" - {0:+0.00} MB/sec\n", (object) (float) ((double) this.m_avgMemDelta * (double) HUDGC.B2MB));
          this.stringBuilder.AppendFormat(" - {0:+0.00} kB/frame\n", (object) (float) ((double) this.m_avgMemDelta / 60.0 * (double) HUDGC.B2kB));
          this.stringBuilder.AppendFormat("Last GC Duration: {0:00.00} ms\n", (object) (float) ((double) this.m_lastGcDuration * 1000.0));
          this.stringBuilder.AppendFormat("Time Since Last GC: {0: 00.0} sec\n", (object) (float) ((double) UnityEngine.Time.realtimeSinceStartup - (double) this.m_lastGcTime));
          this.stringBuilder.AppendFormat("Avg Duration: {0:00.00} ms\n", (object) (float) ((double) this.m_avgDuration * 1000.0));
          this.stringBuilder.AppendFormat("Avg Time Between: {0:00.0} sec\n", (object) this.m_avgFrequency);
          this.stringBuilder.AppendFormat("Total Collections: {0}\n", (object) ProfileUtils.GetMonoCollectionCount());
          this.m_label.Text = this.stringBuilder.ToString();
          this.m_lastFrameMemSize = monoUsedHeapSize;
          this.m_lastFrameTime = UnityEngine.Time.realtimeSinceStartup;
          this.m_showGcBarLastFrame = true;
        }
        else
        {
          if (this.m_label.IsVisible)
            this.m_label.IsVisible = false;
          this.m_showGcBarLastFrame = false;
        }
        HUDGC.SkipNextGC = false;
      }
    }

}
