using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Add-Remove Controls/Remove Child Controls")]
public class DemoRemoveAllControls : MonoBehaviour
  {
    public dfControl target;

    public void Start()
    {
      if (!((Object) this.target == (Object) null))
        return;
      this.target = this.GetComponent<dfControl>();
    }

    public void OnClick()
    {
      while (this.target.Controls.Count > 0)
      {
        dfControl control = this.target.Controls[0];
        this.target.RemoveControl(control);
        Object.DestroyImmediate((Object) control.gameObject);
      }
    }
  }

