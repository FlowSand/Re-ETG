// Decompiled with JetBrains decompiler
// Type: Displace
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[RequireComponent(typeof (WaterBase))]
public class Displace : MonoBehaviour
{
  public void Awake()
  {
    if (this.enabled)
      this.OnEnable();
    else
      this.OnDisable();
  }

  public void OnEnable()
  {
    Shader.EnableKeyword("WATER_VERTEX_DISPLACEMENT_ON");
    Shader.DisableKeyword("WATER_VERTEX_DISPLACEMENT_OFF");
  }

  public void OnDisable()
  {
    Shader.EnableKeyword("WATER_VERTEX_DISPLACEMENT_OFF");
    Shader.DisableKeyword("WATER_VERTEX_DISPLACEMENT_ON");
  }
}
