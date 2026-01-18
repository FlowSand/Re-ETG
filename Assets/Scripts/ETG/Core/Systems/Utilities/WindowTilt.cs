using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Window Tilt")]
public class WindowTilt : MonoBehaviour
  {
    private dfControl control;

    private void Start()
    {
      this.control = this.GetComponent<dfControl>();
      if (!((Object) this.control == (Object) null))
        return;
      this.enabled = false;
    }

    private void Update()
    {
      this.control.transform.localRotation = Quaternion.Euler(0.0f, (float) (((double) this.control.GetCamera().WorldToViewportPoint(this.control.GetCenter()).x * 2.0 - 1.0) * 20.0), 0.0f);
    }
  }

