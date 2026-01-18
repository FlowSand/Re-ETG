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

