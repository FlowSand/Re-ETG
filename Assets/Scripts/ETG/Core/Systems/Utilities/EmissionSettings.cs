// Decompiled with JetBrains decompiler
// Type: EmissionSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class EmissionSettings : MonoBehaviour
  {
    public float EmissivePower;
    public float EmissiveColorPower = 7f;
    private static bool indicesInitialized;
    private static int powerIndex;
    private static int colorPowerIndex;

    private void Start()
    {
      if (!EmissionSettings.indicesInitialized)
      {
        EmissionSettings.indicesInitialized = true;
        EmissionSettings.powerIndex = Shader.PropertyToID("_EmissivePower");
        EmissionSettings.colorPowerIndex = Shader.PropertyToID("_EmissiveColorPower");
      }
      tk2dBaseSprite component = this.GetComponent<tk2dBaseSprite>();
      if ((Object) component != (Object) null)
        component.usesOverrideMaterial = true;
      this.GetComponent<Renderer>().material.SetFloat(EmissionSettings.powerIndex, this.EmissivePower);
      this.GetComponent<Renderer>().material.SetFloat(EmissionSettings.colorPowerIndex, this.EmissiveColorPower);
    }
  }

