using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Set Default Control")]
public class SetDefaultControl : MonoBehaviour
  {
    public dfControl defaultControl;
    private dfControl thisControl;

    public void Awake() => this.thisControl = this.GetComponent<dfControl>();

    public void OnEnable()
    {
      if (!((Object) this.defaultControl != (Object) null) || !this.defaultControl.IsVisible)
        return;
      this.defaultControl.Focus(true);
    }

    [DebuggerHidden]
    public IEnumerator OnIsVisibleChanged(dfControl control, bool value)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new SetDefaultControl__OnIsVisibleChangedc__Iterator0()
      {
        control = control,
        value = value,
        _this = this
      };
    }
  }

