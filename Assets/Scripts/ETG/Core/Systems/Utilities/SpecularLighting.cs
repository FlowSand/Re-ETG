// Decompiled with JetBrains decompiler
// Type: SpecularLighting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[RequireComponent(typeof (WaterBase))]
[ExecuteInEditMode]
public class SpecularLighting : MonoBehaviour
  {
    public Transform specularLight;
    private WaterBase waterBase;

    public void Start()
    {
      this.waterBase = (WaterBase) this.gameObject.GetComponent(typeof (WaterBase));
    }

    public void Update()
    {
      if (!(bool) (Object) this.waterBase)
        this.waterBase = (WaterBase) this.gameObject.GetComponent(typeof (WaterBase));
      if (!(bool) (Object) this.specularLight || !(bool) (Object) this.waterBase.sharedMaterial)
        return;
      this.waterBase.sharedMaterial.SetVector("_WorldLightDir", (Vector4) this.specularLight.transform.forward);
    }
  }

