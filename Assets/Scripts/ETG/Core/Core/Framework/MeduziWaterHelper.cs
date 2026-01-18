using Dungeonator;
using UnityEngine;

#nullable disable

public class MeduziWaterHelper : BraveBehaviour
  {
    public GameObject ReflectionQuadPrefab;
    public Material floorWaterMaterial;
    private Transform m_quadInstance;
    private RoomHandler m_room;
    private bool m_cachedReflectionsEnabled;

    private void Start()
    {
      this.m_room = this.transform.parent.GetComponent<AIActor>().ParentRoom;
      this.transform.parent = this.m_room.hierarchyParent;
      this.m_cachedReflectionsEnabled = GameManager.Options.RealtimeReflections;
      this.ToggleToState(this.m_cachedReflectionsEnabled);
    }

    private void Update()
    {
      if (this.m_cachedReflectionsEnabled == GameManager.Options.RealtimeReflections)
        return;
      this.m_cachedReflectionsEnabled = GameManager.Options.RealtimeReflections;
      this.ToggleToState(this.m_cachedReflectionsEnabled);
    }

    private void ToggleToState(bool refl)
    {
      if (!(bool) (Object) this.m_quadInstance)
      {
        this.m_quadInstance = Object.Instantiate<GameObject>(this.ReflectionQuadPrefab).transform;
        this.m_quadInstance.position = this.m_room.GetCenterCell().ToVector3();
        this.m_quadInstance.position = this.m_quadInstance.position.WithZ(this.m_quadInstance.position.y - 16f);
      }
      Material sharedMaterial = this.m_quadInstance.GetComponent<MeshRenderer>().sharedMaterial;
      if (refl)
      {
        this.m_quadInstance.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));
        sharedMaterial.shader = ShaderCache.Acquire("Brave/ReflectionOnly");
        this.floorWaterMaterial.SetColor("_LightCausticColor", new Color(0.5f, 0.5f, 0.5f));
      }
      else
      {
        this.m_quadInstance.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
        sharedMaterial.shader = ShaderCache.Acquire("Particles/Additive");
        this.floorWaterMaterial.SetColor("_LightCausticColor", new Color(1f, 1f, 1f));
      }
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

