// Decompiled with JetBrains decompiler
// Type: SetDefaultControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
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
        return (IEnumerator) new SetDefaultControl.<OnIsVisibleChanged>c__Iterator0()
        {
          control = control,
          value = value,
          _this = this
        };
      }
    }

}
