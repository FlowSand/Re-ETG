// Decompiled with JetBrains decompiler
// Type: EmissivePulse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class EmissivePulse : MonoBehaviour
  {
    public float minEmissivePower = 10f;
    public float maxEmissivePower = 20f;
    public float period = 2f;
    private Material m_material;
    private int m_id = -1;

    private void Start()
    {
      this.m_id = Shader.PropertyToID("_EmissivePower");
      this.m_material = this.GetComponent<Renderer>().material;
    }

    private void Update()
    {
      this.m_material.SetFloat(this.m_id, Mathf.Lerp(this.minEmissivePower, this.maxEmissivePower, Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(UnityEngine.Time.timeSinceLevelLoad / this.period, 1f))));
    }
  }

