// Decompiled with JetBrains decompiler
// Type: DebugRenderInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/General/Debug Render Info")]
public class DebugRenderInfo : MonoBehaviour
{
  public float interval = 0.5f;
  private dfLabel info;
  private dfGUIManager view;
  private float lastUpdate;
  private int frameCount;

  private void Start()
  {
    this.info = this.GetComponent<dfLabel>();
    if ((UnityEngine.Object) this.info == (UnityEngine.Object) null)
    {
      this.enabled = false;
      throw new Exception("No Label component found");
    }
    this.info.Text = string.Empty;
  }

  private void Update()
  {
    if ((UnityEngine.Object) this.view == (UnityEngine.Object) null)
      this.view = this.info.GetManager();
    ++this.frameCount;
    float num1 = UnityEngine.Time.realtimeSinceStartup - this.lastUpdate;
    if ((double) num1 < (double) this.interval)
      return;
    this.lastUpdate = UnityEngine.Time.realtimeSinceStartup;
    float num2 = (float) (1.0 / ((double) num1 / (double) this.frameCount));
    Vector2 vector2 = new Vector2((float) Screen.width, (float) Screen.height);
    string str = $"{(int) vector2.x}x{(int) vector2.y}";
    string format = "Screen : {0}, DrawCalls: {1}, Triangles: {2}, Mem: {3:F0}MB, FPS: {4:F0}";
    float num3 = !Profiler.supported ? (float) GC.GetTotalMemory(false) / 1048576f : (float) Profiler.GetMonoUsedSize() / 1048576f;
    this.info.Text = string.Format(format, (object) str, (object) this.view.TotalDrawCalls, (object) this.view.TotalTriangles, (object) num3, (object) num2).Trim();
    this.frameCount = 0;
  }
}
